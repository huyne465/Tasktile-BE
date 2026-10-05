using Tasktile.Api.Domain;

namespace Tasktile.Api.Contracts;

public sealed record TaskDto(
    Guid Id,
    string Title,
    string Notes,
    int Priority,
    bool IsDone,
    DateTimeOffset? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset? CompletedAt,
    DateTimeOffset UpdatedAt);

public sealed record CreateTaskRequest(
    string Title,
    string? Notes,
    int Priority = 1,
    DateTimeOffset? DueDate = null);

public sealed record UpdateTaskRequest(
    string Title,
    string? Notes,
    int Priority,
    bool IsDone,
    DateTimeOffset? DueDate);

public static class TaskMappings
{
    public static TaskDto ToDto(this TaskEntity t) =>
        new(t.Id, t.Title, t.Notes, t.Priority, t.IsDone, t.DueDate, t.CreatedAt, t.CompletedAt, t.UpdatedAt);
}
