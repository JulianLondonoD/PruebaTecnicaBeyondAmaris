using TodoApp.Domain.Domain.Entities;
using TodoApp.Domain.Domain.Exceptions;

namespace TodoApp.Tests.Domain;

public class TodoItemTests
{
    [Fact]
    public void Create_ValidTodoItem_Success()
    {
        var item = new TodoItem(1, "Test", "Description", "Work");
        
        Assert.Equal(1, item.Id.Value);
        Assert.Equal("Test", item.Title);
        Assert.Equal("Description", item.Description);
        Assert.Equal("Work", item.Category);
        Assert.Empty(item.Progressions);
        Assert.False(item.IsCompleted);
    }

    [Fact]
    public void AddProgression_ValidProgression_Success()
    {
        var item = new TodoItem(1, "Test", "Description", "Work");
        item.AddProgression(DateTime.Now, 30);
        
        Assert.Single(item.Progressions);
        Assert.Equal(30, item.CurrentProgress);
        Assert.False(item.IsCompleted);
    }

    [Fact]
    public void AddProgression_Multiple_Success()
    {
        var item = new TodoItem(1, "Test", "Description", "Work");
        var date1 = new DateTime(2025, 1, 1);
        var date2 = new DateTime(2025, 1, 2);
        
        item.AddProgression(date1, 30);
        item.AddProgression(date2, 40);
        
        Assert.Equal(2, item.Progressions.Count);
        Assert.Equal(70, item.CurrentProgress);
        Assert.False(item.IsCompleted);
    }

    [Fact]
    public void AddProgression_CompleteItem_IsCompletedTrue()
    {
        var item = new TodoItem(1, "Test", "Description", "Work");
        item.AddProgression(DateTime.Now, 100);
        
        Assert.True(item.IsCompleted);
    }

    [Fact]
    public void AddProgression_SumEquals100_IsCompletedTrue()
    {
        var item = new TodoItem(1, "Test", "Description", "Work");
        var date1 = new DateTime(2025, 1, 1);
        var date2 = new DateTime(2025, 1, 2);
        
        item.AddProgression(date1, 60);
        item.AddProgression(date2, 40);
        
        Assert.True(item.IsCompleted);
    }

    [Fact]
    public void AddProgression_DateNotGreater_ThrowsException()
    {
        var item = new TodoItem(1, "Test", "Description", "Work");
        var date = DateTime.Now;
        
        item.AddProgression(date, 30);
        
        Assert.Throws<ProgressionDateException>(() => item.AddProgression(date, 20));
    }

    [Fact]
    public void AddProgression_ExceedsTotal_ThrowsException()
    {
        var item = new TodoItem(1, "Test", "Description", "Work");
        item.AddProgression(DateTime.Now, 60);
        
        Assert.Throws<ProgressionPercentException>(() => item.AddProgression(DateTime.Now.AddDays(1), 50));
    }

    [Fact]
    public void Update_ProgressUnder50_Success()
    {
        var item = new TodoItem(1, "Test", "Description", "Work");
        item.AddProgression(DateTime.Now, 40);
        item.Update("New Description");
        
        Assert.Equal("New Description", item.Description);
    }

    [Fact]
    public void Update_ProgressOver50_ThrowsException()
    {
        var item = new TodoItem(1, "Test", "Description", "Work");
        item.AddProgression(DateTime.Now, 60);
        
        Assert.Throws<TodoItemProgressException>(() => item.Update("New Description"));
    }
}
