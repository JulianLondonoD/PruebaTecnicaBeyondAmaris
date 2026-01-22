using Moq;
using TodoApp.Application.Application.Handlers;
using TodoApp.Application.Application.Queries;
using TodoApp.Domain.Domain.TodoLists;

namespace TodoApp.Tests.Application;

public class GetCategoriesQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsCategories()
    {
        var categories = new List<string> { "Work", "Personal", "Study", "Health" };
        var mockRepository = new Mock<ITodoListRepository>();
        mockRepository.Setup(x => x.GetCategoriesAsync()).ReturnsAsync(categories);
        var handler = new GetCategoriesQueryHandler(mockRepository.Object);
        var query = new GetCategoriesQuery();

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Equal(4, result.Count);
        Assert.Contains("Work", result);
    }
}
