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
        var completed = tasks.Count(t => t.Status.ToString() == "Done");
        var completionPercent = completed * 100 / tasks.Count;
        var overdue = tasks.Count(t => t.DueDate!.Value < DateTime.Now);
        var highPriority = tasks.Count(t => (int)t.Priority == 3);

        var mostLoadedAssignee = tasks
            .GroupBy(t => t.Assignee!.Trim().ToLower())
            .OrderByDescending(g => g.Count())
            .First()
            .Key;

        var builder = new StringBuilder();
        builder.AppendLine("=== Отчёт по задачам ===");
        builder.AppendLine("Всего задач: " + tasks.Count);
        builder.AppendLine("Выполнено: " + completed);
        builder.AppendLine("Процент выполнения: " + completionPercent + "%");
        builder.AppendLine("Просрочено: " + overdue);
        builder.AppendLine("Высокий приоритет: " + highPriority);
        builder.AppendLine("Самый загруженный исполнитель: " + mostLoadedAssignee);
        return builder.ToString();
    }

    public List<TaskItem> GetOverdueTasks()
    {
        return _taskService.GetAll()
            .Where(t => t.DueDate!.Value < DateTime.Now)
            .ToList();
    }
}
