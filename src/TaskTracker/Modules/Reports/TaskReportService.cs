using System.Text;
using TaskTracker.Models;
using TaskTracker.Services;

namespace TaskTracker.Modules.Reports;

public class TaskReportService
{
    private readonly TaskService _taskService;

    public TaskReportService(TaskService taskService)
    {
        _taskService = taskService;
    }

    public string BuildSummary()
    {
        var tasks = _taskService.GetAll().ToList();

        var total = tasks.Count;
        var newTasks = tasks.Count(t => t.Status == TaskState.New);
        var inProgress = tasks.Count(t => t.Status == TaskState.InProgress);
        var completed = tasks.Count(t => t.Status == TaskState.Completed);
        var completionPercent = total == 0
            ? 0
            : (int)Math.Round(completed * 100.0 / total);
        var overdue = tasks.Count(IsOverdue);
        var highPriority = tasks.Count(t => t.Priority == TaskPriority.High);
        var withoutAssignee = tasks.Count(t => string.IsNullOrWhiteSpace(t.Assignee));

        var mostLoadedAssignee = tasks
            .Where(t => !string.IsNullOrWhiteSpace(t.Assignee))
            .GroupBy(t => t.Assignee.Trim(), StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault()?.Key ?? "нет";

        var builder = new StringBuilder();
        builder.AppendLine("=== Отчёт по задачам ===");
        builder.AppendLine("Всего задач: " + total);
        builder.AppendLine("Новых: " + newTasks);
        builder.AppendLine("В работе: " + inProgress);
        builder.AppendLine("Выполнено: " + completed);
        builder.AppendLine("Процент выполнения: " + completionPercent + "%");
        builder.AppendLine("Просрочено: " + overdue);
        builder.AppendLine("Высокий приоритет: " + highPriority);
        builder.AppendLine("Без исполнителя: " + withoutAssignee);
        builder.AppendLine("Самый загруженный исполнитель: " + mostLoadedAssignee);
        return builder.ToString();
    }

    public List<TaskItem> GetOverdueTasks()
    {
        return _taskService.GetAll()
            .Where(IsOverdue)
            .ToList();
    }

    private static bool IsOverdue(TaskItem task)
    {
        return task.DueDate.HasValue
            && task.Status != TaskState.Completed
            && task.DueDate.Value < DateTime.Now;
    }
}
