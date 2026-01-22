using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoApp.Application.Common.Interfaces;
using TodoApp.Domain.Domain.Aggregates;
using TodoApp.Domain.Domain.Entities;
using TodoApp.Domain.Domain.TodoLists;
using TodoApp.Infrastructure.Data;
using TodoApp.Infrastructure.Data.Entities;

namespace TodoApp.Infrastructure.Repositories;

public class TodoListRepository : ITodoListRepository
{
    private readonly TodoDbContext _context;
    private readonly ITodoItemFactory _factory;
    private readonly ITodoListDomainService _domainService;
    private readonly ILogger<TodoListRepository> _logger;
    private readonly IResiliencePolicyProvider _resiliencePolicyProvider;

    public TodoListRepository(
        TodoDbContext context,
        ITodoItemFactory factory,
        ITodoListDomainService domainService,
        ILogger<TodoListRepository> logger,
        IResiliencePolicyProvider resiliencePolicyProvider)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _resiliencePolicyProvider = resiliencePolicyProvider ?? throw new ArgumentNullException(nameof(resiliencePolicyProvider));
    }

    public async Task<TodoListAggregate> LoadAsync()
    {
        _logger.LogDebug("Loading TodoList aggregate from database");
        
        var policy = _resiliencePolicyProvider.GetDatabasePolicy<TodoListAggregate>();
        
        return await policy.ExecuteAsync(async cancellationToken =>
        {
            // Initialize domain service with valid categories
            var categories = await _context.Categories.Select(c => c.Name).ToListAsync(cancellationToken);
            _domainService.SetValidCategories(categories);
            
            var entities = await _context.TodoItems
                .Include(x => x.Progressions.OrderBy(p => p.DateTime))
                .OrderBy(x => x.Id)
                .ToListAsync(cancellationToken);

            var aggregate = new TodoListAggregate(_factory, _domainService);

            foreach (var entity in entities)
            {
                try
                {
                    var domainItem = _factory.Create(entity.Id, entity.Title, entity.Description, entity.Category);
                    
                    // Add progressions in order
                    foreach (var progression in entity.Progressions.OrderBy(p => p.DateTime))
                    {
                        domainItem.AddProgression(progression.DateTime, progression.Percent);
                    }
                    
                    // Load item into aggregate
                    aggregate.LoadExistingItem(domainItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading TodoItem {Id} from database", entity.Id);
                    throw;
                }
            }

            _logger.LogDebug("Loaded {Count} TodoItems from database", entities.Count);
            return aggregate;
        }, CancellationToken.None);
    }

    public async Task SaveAsync(TodoListAggregate aggregate)
    {
        _logger.LogDebug("Saving TodoList aggregate to database");
        
        var policy = _resiliencePolicyProvider.GetDatabasePolicy<bool>();
        
        await policy.ExecuteAsync(async cancellationToken =>
        {
            // Only use transactions if not using in-memory database
            var providerName = _context.Database.ProviderName ?? "";
            var useTransaction = !providerName.Contains("InMemory", StringComparison.OrdinalIgnoreCase);
            
            if (useTransaction)
            {
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                
                try
                {
                    // Delete items that are no longer in the aggregate
                    await DeleteRemovedItemsAsync(aggregate, cancellationToken);
                    
                    foreach (var domainItem in aggregate.Items)
                    {
                        await SaveTodoItemAsync(domainItem, cancellationToken);
                    }
                    
                    await _context.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                    
                    _logger.LogDebug("Successfully saved TodoList aggregate");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex, "Error saving TodoList aggregate");
                    throw;
                }
            }
            else
            {
                try
                {
                    // Delete items that are no longer in the aggregate
                    await DeleteRemovedItemsAsync(aggregate, cancellationToken);
                    
                    foreach (var domainItem in aggregate.Items)
                    {
                        await SaveTodoItemAsync(domainItem, cancellationToken);
                    }
                    
                    await _context.SaveChangesAsync(cancellationToken);
                    
                    _logger.LogDebug("Successfully saved TodoList aggregate");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving TodoList aggregate");
                    throw;
                }
            }
            
            return true;
        }, CancellationToken.None);
    }

    private async Task DeleteRemovedItemsAsync(TodoListAggregate aggregate, CancellationToken cancellationToken = default)
    {
        // Get current item IDs from aggregate
        var aggregateIds = aggregate.Items.Select(x => x.Id.Value).ToHashSet();
        
        // Find items that need to be deleted (exist in DB but not in aggregate)
        var itemsToDelete = await _context.TodoItems
            .Where(x => !aggregateIds.Contains(x.Id))
            .ToListAsync(cancellationToken);
        
        if (itemsToDelete.Any())
        {
            _logger.LogInformation("Deleting {Count} removed TodoItems from database", itemsToDelete.Count);
            
            // Remove the items (CASCADE delete will handle progressions)
            _context.TodoItems.RemoveRange(itemsToDelete);
        }
    }

    private async Task SaveTodoItemAsync(TodoItem domainItem, CancellationToken cancellationToken = default)
    {
        var existingEntity = await _context.TodoItems
            .Include(x => x.Progressions)
            .FirstOrDefaultAsync(x => x.Id == domainItem.Id.Value, cancellationToken);

        if (existingEntity == null)
        {
            // Create new TodoItem
            var newEntity = new TodoItemEntity
            {
                Id = domainItem.Id.Value,
                Title = domainItem.Title,
                Description = domainItem.Description,
                Category = domainItem.Category,
                CreatedAt = DateTime.UtcNow,
                Progressions = domainItem.Progressions.Select(progression => new ProgressionEntity
                {
                    DateTime = progression.DateTime,
                    Percent = progression.Percent,
                    TodoItemId = domainItem.Id.Value,
                    CreatedAt = DateTime.UtcNow
                }).ToList()
            };

            _context.TodoItems.Add(newEntity);
            _logger.LogDebug("Created new TodoItem entity {Id}", domainItem.Id.Value);
        }
        else
        {
            // Update existing TodoItem
            existingEntity.Title = domainItem.Title;
            existingEntity.Description = domainItem.Description;
            existingEntity.Category = domainItem.Category;
            existingEntity.UpdatedAt = DateTime.UtcNow;

            // Update progressions (remove all and re-add)
            _context.Progressions.RemoveRange(existingEntity.Progressions);
            
            existingEntity.Progressions = domainItem.Progressions.Select(progression => new ProgressionEntity
            {
                DateTime = progression.DateTime,
                Percent = progression.Percent,
                TodoItemId = domainItem.Id.Value,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            _logger.LogDebug("Updated TodoItem entity {Id}", domainItem.Id.Value);
        }
    }

    public async Task<int> GetNextIdAsync()
    {
        var policy = _resiliencePolicyProvider.GetDatabasePolicy<int>();
        
        return await policy.ExecuteAsync(async cancellationToken =>
        {
            var maxId = await _context.TodoItems.AnyAsync(cancellationToken) 
                ? await _context.TodoItems.MaxAsync(x => x.Id, cancellationToken)
                : 0;
                
            return maxId + 1;
        }, CancellationToken.None);
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        var policy = _resiliencePolicyProvider.GetDatabasePolicy<List<string>>();
        
        return await policy.ExecuteAsync(async cancellationToken =>
        {
            return await _context.Categories
                .Select(c => c.Name)
                .OrderBy(name => name)
                .ToListAsync(cancellationToken);
        }, CancellationToken.None);
    }
}
