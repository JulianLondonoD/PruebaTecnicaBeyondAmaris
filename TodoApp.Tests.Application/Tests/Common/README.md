# Test Data Builders

This directory contains test data builders for the TodoApp test project. These builders follow the Builder pattern and provide a fluent API to create test data easily and consistently.

## Files

### Builders/TodoItemBuilder.cs
A fluent builder for creating `TodoItem` entities with customizable properties and progressions.

**Features:**
- Set custom Id, Title, Description, and Category
- Add single or multiple progressions
- Shortcuts for common scenarios (Completed, InProgress)
- Integrates with `TodoItemFactory`
- Automatic progression ordering by date

**Usage:**
```csharp
// Create with defaults
var item = new TodoItemBuilder().Build();

// Create with custom values
var item = new TodoItemBuilder()
    .WithId(42)
    .WithTitle("My Task")
    .WithDescription("Task description")
    .WithCategory("Work")
    .Build();

// Create completed task
var item = new TodoItemBuilder()
    .WithTitle("Completed Task")
    .Completed()
    .Build();

// Create task with specific progress
var item = new TodoItemBuilder()
    .WithTitle("In Progress Task")
    .InProgress(50)
    .Build();

// Create task with multiple progressions
var baseDate = DateTime.Now;
var item = new TodoItemBuilder()
    .WithProgression(baseDate, 25)
    .WithProgression(baseDate.AddDays(1), 25)
    .WithProgression(baseDate.AddDays(2), 50)
    .Build();
```

### Builders/CategoryBuilder.cs
A simple builder for creating category strings with predefined common categories.

**Features:**
- Predefined categories (Work, Personal, Study, Health, Finance, Shopping, Home)
- Support for custom categories
- Fluent API

**Usage:**
```csharp
// Use predefined category
var category = new CategoryBuilder().Work().Build();

// Use custom category
var category = new CategoryBuilder().Custom("MyCategory").Build();

// Access common categories directly
var workCategory = CategoryBuilder.CommonCategories.Work;
```

### TestDataGenerator.cs
A static class providing helper methods to generate random test data using AutoFixture.

**Features:**
- Generate random TodoItems
- Generate TodoItems with specific number of progressions
- Generate completed/in-progress/not-started TodoItems
- Generate random progressions
- Generate multiple TodoItems at once

**Usage:**
```csharp
// Generate random TodoItem
var item = TestDataGenerator.GenerateRandomTodoItem();

// Generate TodoItem with 3 progressions
var item = TestDataGenerator.GenerateTodoItemWithProgress(3);

// Generate completed TodoItem
var item = TestDataGenerator.GenerateCompletedTodoItem();

// Generate in-progress TodoItem with 50% progress
var item = TestDataGenerator.GenerateInProgressTodoItem(50);

// Generate not-started TodoItem
var item = TestDataGenerator.GenerateNotStartedTodoItem();

// Generate random Progression
var progression = TestDataGenerator.GenerateRandomProgression();

// Generate multiple TodoItems
var items = TestDataGenerator.GenerateMultipleTodoItems(10);

// Get random category
var category = TestDataGenerator.GetRandomCategory();

// Generate random TodoItemId
var id = TestDataGenerator.GenerateRandomId();
```

## Design Patterns

### Fluent Builder Pattern
The builders use the fluent builder pattern, allowing method chaining for readable test setup:
```csharp
var item = new TodoItemBuilder()
    .WithId(1)
    .WithTitle("Task")
    .WithCategory("Work")
    .InProgress(50)
    .Build();
```

### Test Data Builder Pattern
Builders provide sensible defaults so you only need to specify what matters for your test:
```csharp
// Only specify what's important for this test
var item = new TodoItemBuilder()
    .WithId(42)
    .Build();
```

### Integration with AutoFixture
`TestDataGenerator` uses AutoFixture to generate random but valid test data, reducing test maintenance and improving coverage.

## Examples

See `Examples/BuilderExampleTests.cs` for comprehensive examples of all builder features.

## Best Practices

1. **Use Builders for Arrange Phase**: Builders are ideal for the "Arrange" phase of your tests
2. **Specify Only What Matters**: Only set values that are relevant to your specific test
3. **Use Shortcuts When Available**: Use `Completed()`, `InProgress()` instead of manually adding progressions
4. **Leverage TestDataGenerator for Random Data**: When exact values don't matter, use `TestDataGenerator`
5. **Keep Tests Readable**: Builder chains should read like a sentence describing the test scenario

## Contributing

When adding new domain entities, consider creating corresponding builders to maintain consistency across the test suite.
