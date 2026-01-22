using TodoApp.Domain.Domain.ValueObjects;

namespace TodoApp.Tests.Domain;

public class ProgressionTests
{
    [Fact]
    public void Create_ValidProgression_Success()
    {
        var dateTime = DateTime.Now;
        var progression = new Progression(dateTime, 50);
        
        Assert.Equal(dateTime, progression.DateTime);
        Assert.Equal(50, progression.Percent);
    }

    [Fact]
    public void Create_ZeroPercent_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new Progression(DateTime.Now, 0));
    }

    [Fact]
    public void Create_NegativePercent_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new Progression(DateTime.Now, -10));
    }

    [Fact]
    public void Create_PercentOver100_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => new Progression(DateTime.Now, 101));
    }

    [Fact]
    public void Create_Percent100_Success()
    {
        var progression = new Progression(DateTime.Now, 100);
        Assert.Equal(100, progression.Percent);
    }
}
