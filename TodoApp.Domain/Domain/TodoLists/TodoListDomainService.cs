namespace TodoApp.Domain.Domain.TodoLists;

public class TodoListDomainService : ITodoListDomainService
{
    private List<string> _validCategories = new();

    public void SetValidCategories(List<string> categories)
    {
        _validCategories = categories ?? throw new ArgumentNullException(nameof(categories));
    }

    public bool ValidateCategory(string category)
    {
        return _validCategories.Contains(category);
    }
}
