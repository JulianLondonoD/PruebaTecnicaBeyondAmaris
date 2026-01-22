using Microsoft.Extensions.Logging;
using Moq;
using TodoApp.Application.Application.Commands;
using TodoApp.Application.Application.Handlers;
using TodoApp.Domain.Domain.Aggregates;
using TodoApp.Domain.Domain.TodoLists;

namespace TodoApp.Tests.Application;

public class AddTodoItemCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_CallsLoadAndSave()
    {
        var mockFactory = new Mock<ITodoItemFactory>();
        var mockDomainService = new Mock<ITodoListDomainService>();
        mockDomainService.Setup(x => x.ValidateCategory(It.IsAny<string>())).Returns(true);
        var aggregate = new TodoListAggregate(mockFactory.Object, mockDomainService.Object);
        
        var mockRepository = new Mock<ITodoListRepository>();
        mockRepository.Setup(x => x.LoadAsync()).ReturnsAsync(aggregate);
        mockRepository.Setup(x => x.SaveAsync(It.IsAny<TodoListAggregate>())).Returns(Task.CompletedTask);
        
        var mockLogger = new Mock<ILogger<AddTodoItemCommandHandler>>();
        var handler = new AddTodoItemCommandHandler(mockRepository.Object, mockLogger.Object);
        var command = new AddTodoItemCommand(1, "Test", "Description", "Work");

        await handler.Handle(command, CancellationToken.None);

        mockRepository.Verify(x => x.LoadAsync(), Times.Once);
        mockRepository.Verify(x => x.SaveAsync(It.IsAny<TodoListAggregate>()), Times.Once);
    }
}
