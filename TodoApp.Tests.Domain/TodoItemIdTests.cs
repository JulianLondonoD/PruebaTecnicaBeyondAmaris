using TodoApp.Domain.Domain.ValueObjects;

namespace TodoApp.Tests.Domain;

public class TodoItemIdTests
{
    [Fact]
    public void Create_ValidId_Success()
    {
        var id = new TodoItemId(1);
        Assert.Equal(1, id.Value);
    }

    [Fact]
    public void Create_ZeroId_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new TodoItemId(0));
    }

    [Fact]
    public void Create_NegativeId_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new TodoItemId(-1));
    }

    [Fact]
    public void ImplicitConversion_ToInt_Success()
    {
        TodoItemId id = new TodoItemId(5);
        int value = id;
        Assert.Equal(5, value);
    }

    [Fact]
    public void ImplicitConversion_FromInt_Success()
    {
        TodoItemId id = 10;
        Assert.Equal(10, id.Value);
    }
}
