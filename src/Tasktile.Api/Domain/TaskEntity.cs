namespace Tasktile.Api.Domain;

public sealed class TaskEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OwnerId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    /// 0 = thấp, 1 = vừa, 2 = cao (khớp TaskPriority bên iOS).
    public int Priority { get; set; } = 1;
    public bool IsDone { get; set; }
    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? CompletedAt { get; set; }
    /// Dùng cho đồng bộ sau này (last-write-wins).
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
