using Microsoft.Extensions.Logging;
using Moq;
using TodoApp.Application.Application.Commands;
using TodoApp.Application.Application.Handlers;
using TodoApp.Domain.Domain.Aggregates;
using TodoApp.Domain.Domain.Entities;
using TodoApp.Domain.Domain.TodoLists;
using TodoApp.Domain.Domain.ValueObjects;

namespace TodoApp.Tests.Application;

public class RegisterProgressionCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_CallsLoadAndSave()
    {
        var mockFactory = new Mock<ITodoItemFactory>();
        var mockDomainService = new Mock<ITodoListDomainService>();
        var aggregate = new TodoListAggregate(mockFactory.Object, mockDomainService.Object);
        
        // Add an existing item to the aggregate so progression registration won't fail
        var item = new TodoItem(new TodoItemId(1), "Test", "Description", "Work");
        aggregate.LoadExistingItem(item);
        
        var mockRepository = new Mock<ITodoListRepository>();
        mockRepository.Setup(x => x.LoadAsync()).ReturnsAsync(aggregate);
        mockRepository.Setup(x => x.SaveAsync(It.IsAny<TodoListAggregate>())).Returns(Task.CompletedTask);
        
        var mockLogger = new Mock<ILogger<RegisterProgressionCommandHandler>>();
        var handler = new RegisterProgressionCommandHandler(mockRepository.Object, mockLogger.Object);
        var dateTime = DateTime.Now;
        var command = new RegisterProgressionCommand(1, dateTime, 50);

        await handler.Handle(command, CancellationToken.None);

        mockRepository.Verify(x => x.LoadAsync(), Times.Once);
        mockRepository.Verify(x => x.SaveAsync(It.IsAny<TodoListAggregate>()), Times.Once);
    }
}
