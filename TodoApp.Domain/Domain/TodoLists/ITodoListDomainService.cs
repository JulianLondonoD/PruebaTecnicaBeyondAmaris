using TodoApp.Domain.Domain.ValueObjects;

namespace TodoApp.Domain.Domain.TodoLists;

public interface ITodoListDomainService
{
    bool ValidateCategory(string category);
    void SetValidCategories(List<string> categories);
}
