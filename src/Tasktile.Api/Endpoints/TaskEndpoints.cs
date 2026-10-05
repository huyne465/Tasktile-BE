using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Tasktile.Api.Auth;
using Tasktile.Api.Contracts;
using Tasktile.Api.Data;
using Tasktile.Api.Domain;

namespace Tasktile.Api.Endpoints;

public static class TaskEndpoints
{
    public static void MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tasks").RequireAuthorization();

        group.MapGet("/", async (ClaimsPrincipal user, AppDbContext db) =>
        {
            var uid = user.GetUserId();
            var tasks = await db.Tasks
                .Where(t => t.OwnerId == uid)
                .OrderBy(t => t.IsDone)
                .ThenByDescending(t => t.Priority)
                .ThenBy(t => t.CreatedAt)
                .ToListAsync();
            return Results.Ok(tasks.Select(t => t.ToDto()));
        });

        group.MapGet("/{id:guid}", async (Guid id, ClaimsPrincipal user, AppDbContext db) =>
        {
            var task = await FindOwned(db, id, user.GetUserId());
            return task is null ? Results.NotFound() : Results.Ok(task.ToDto());
        });

        group.MapPost("/", async (CreateTaskRequest req, ClaimsPrincipal user, AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(req.Title))
                return Results.BadRequest(new { error = "Title không được để trống." });

            var now = DateTimeOffset.UtcNow;
            var task = new TaskEntity
            {
                OwnerId = user.GetUserId(),
                Title = req.Title.Trim(),
                Notes = req.Notes ?? string.Empty,
                Priority = Math.Clamp(req.Priority, 0, 2),
                DueDate = req.DueDate?.ToUniversalTime(),
                CreatedAt = now,
                UpdatedAt = now
            };
            db.Tasks.Add(task);
            await db.SaveChangesAsync();
            return Results.Created($"/api/tasks/{task.Id}", task.ToDto());
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateTaskRequest req, ClaimsPrincipal user, AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(req.Title))
                return Results.BadRequest(new { error = "Title không được để trống." });

            var task = await FindOwned(db, id, user.GetUserId());
            if (task is null) return Results.NotFound();

            var now = DateTimeOffset.UtcNow;
            if (req.IsDone != task.IsDone)
                task.CompletedAt = req.IsDone ? now : null;

            task.Title = req.Title.Trim();
            task.Notes = req.Notes ?? string.Empty;
            task.Priority = Math.Clamp(req.Priority, 0, 2);
            task.IsDone = req.IsDone;
            task.DueDate = req.DueDate?.ToUniversalTime();
            task.UpdatedAt = now;

            await db.SaveChangesAsync();
            return Results.Ok(task.ToDto());
        });

        group.MapPatch("/{id:guid}/toggle", async (Guid id, ClaimsPrincipal user, AppDbContext db) =>
        {
            var task = await FindOwned(db, id, user.GetUserId());
            if (task is null) return Results.NotFound();

            var now = DateTimeOffset.UtcNow;
            task.IsDone = !task.IsDone;
            task.CompletedAt = task.IsDone ? now : null;
            task.UpdatedAt = now;

            await db.SaveChangesAsync();
            return Results.Ok(task.ToDto());
        });

        group.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal user, AppDbContext db) =>
        {
            var task = await FindOwned(db, id, user.GetUserId());
            if (task is null) return Results.NotFound();

            db.Tasks.Remove(task);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }

    private static Task<TaskEntity?> FindOwned(AppDbContext db, Guid id, string ownerId) =>
        db.Tasks.FirstOrDefaultAsync(t => t.Id == id && t.OwnerId == ownerId);
}
