namespace TodoApp.Tests.Application.Tests.Common.Builders;

public class CategoryBuilder
{
    private string _category = "Work";

    public static class CommonCategories
    {
        public const string Work = "Work";
        public const string Personal = "Personal";
        public const string Study = "Study";
        public const string Health = "Health";
        public const string Finance = "Finance";
        public const string Shopping = "Shopping";
        public const string Home = "Home";
    }

    public CategoryBuilder Work()
    {
        _category = CommonCategories.Work;
        return this;
    }

    public CategoryBuilder Personal()
    {
        _category = CommonCategories.Personal;
        return this;
    }

    public CategoryBuilder Study()
    {
        _category = CommonCategories.Study;
        return this;
    }

    public CategoryBuilder Health()
    {
        _category = CommonCategories.Health;
        return this;
    }

    public CategoryBuilder Finance()
    {
        _category = CommonCategories.Finance;
        return this;
    }

    public CategoryBuilder Shopping()
    {
        _category = CommonCategories.Shopping;
        return this;
    }

    public CategoryBuilder Home()
    {
        _category = CommonCategories.Home;
        return this;
    }

    public CategoryBuilder Custom(string category)
    {
        _category = category;
        return this;
    }

    public string Build()
    {
        return _category;
    }

    public static CategoryBuilder Default() => new();
}
