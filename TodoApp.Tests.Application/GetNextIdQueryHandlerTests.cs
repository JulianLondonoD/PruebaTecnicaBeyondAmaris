using Moq;
using TodoApp.Application.Application.Handlers;
using TodoApp.Application.Application.Queries;
using TodoApp.Domain.Domain.TodoLists;

namespace TodoApp.Tests.Application;

public class GetNextIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsNextId()
    {
        var mockRepository = new Mock<ITodoListRepository>();
        mockRepository.Setup(x => x.GetNextIdAsync()).ReturnsAsync(5);
        var handler = new GetNextIdQueryHandler(mockRepository.Object);
        var query = new GetNextIdQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(5, result);
    }
}
