using Moq;
using TodoApp.Domain.Domain.Aggregates;
using TodoApp.Domain.Domain.Entities;
using TodoApp.Domain.Domain.Exceptions;
using TodoApp.Domain.Domain.TodoLists;
using TodoApp.Domain.Domain.ValueObjects;

namespace TodoApp.Tests.Domain;

public class TodoListAggregateTests
{
    private readonly Mock<ITodoItemFactory> _mockFactory;
    private readonly Mock<ITodoListDomainService> _mockDomainService;
    private readonly TodoListAggregate _aggregate;

    public TodoListAggregateTests()
    {
        _mockFactory = new Mock<ITodoItemFactory>();
        _mockDomainService = new Mock<ITodoListDomainService>();
        _aggregate = new TodoListAggregate(_mockFactory.Object, _mockDomainService.Object);
    }

    [Fact]
    public void AddItem_ValidCategory_Success()
    {
        _mockDomainService.Setup(x => x.ValidateCategory("Work")).Returns(true);
        _mockFactory.Setup(x => x.Create(It.IsAny<TodoItemId>(), "Test", "Desc", "Work"))
            .Returns(new TodoItem(1, "Test", "Desc", "Work"));

        _aggregate.AddItem(1, "Test", "Desc", "Work");

        Assert.Single(_aggregate.Items);
    }

    [Fact]
    public void AddItem_InvalidCategory_ThrowsException()
    {
        _mockDomainService.Setup(x => x.ValidateCategory("Invalid")).Returns(false);

        Assert.Throws<DomainException>(() => _aggregate.AddItem(1, "Test", "Desc", "Invalid"));
    }

    [Fact]
    public void UpdateItem_ItemExists_Success()
    {
        _mockDomainService.Setup(x => x.ValidateCategory("Work")).Returns(true);
        var item = new TodoItem(1, "Test", "Desc", "Work");
        _mockFactory.Setup(x => x.Create(It.IsAny<TodoItemId>(), "Test", "Desc", "Work"))
            .Returns(item);

        _aggregate.AddItem(1, "Test", "Desc", "Work");
        _aggregate.UpdateItem(1, "New Desc");

        Assert.Equal("New Desc", _aggregate.Items[0].Description);
    }

    [Fact]
    public void UpdateItem_ItemNotFound_ThrowsException()
    {
        Assert.Throws<DomainException>(() => _aggregate.UpdateItem(999, "New Desc"));
    }

    [Fact]
    public void RemoveItem_ProgressUnder50_Success()
    {
        _mockDomainService.Setup(x => x.ValidateCategory("Work")).Returns(true);
        var item = new TodoItem(1, "Test", "Desc", "Work");
        _mockFactory.Setup(x => x.Create(It.IsAny<TodoItemId>(), "Test", "Desc", "Work"))
            .Returns(item);

        _aggregate.AddItem(1, "Test", "Desc", "Work");
        _aggregate.RemoveItem(1);

        Assert.Empty(_aggregate.Items);
    }

    [Fact]
    public void RemoveItem_ProgressOver50_ThrowsException()
    {
        _mockDomainService.Setup(x => x.ValidateCategory("Work")).Returns(true);
        var item = new TodoItem(1, "Test", "Desc", "Work");
        item.AddProgression(DateTime.Now, 60);
        _mockFactory.Setup(x => x.Create(It.IsAny<TodoItemId>(), "Test", "Desc", "Work"))
            .Returns(item);

        _aggregate.AddItem(1, "Test", "Desc", "Work");

        Assert.Throws<TodoItemProgressException>(() => _aggregate.RemoveItem(1));
    }

    [Fact]
    public void RegisterProgression_ValidProgression_Success()
    {
        _mockDomainService.Setup(x => x.ValidateCategory("Work")).Returns(true);
        var item = new TodoItem(1, "Test", "Desc", "Work");
        _mockFactory.Setup(x => x.Create(It.IsAny<TodoItemId>(), "Test", "Desc", "Work"))
            .Returns(item);

        _aggregate.AddItem(1, "Test", "Desc", "Work");
        _aggregate.RegisterProgression(1, DateTime.Now, 50);

        Assert.Equal(50, _aggregate.Items[0].CurrentProgress);
    }
}
