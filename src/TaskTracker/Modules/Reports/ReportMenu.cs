using TaskTracker.Services;

namespace TaskTracker.Modules.Reports;

public class ReportMenu
{
    private readonly TaskReportService _reportService;
    private readonly CsvReportExporter _exporter;
    private readonly TaskService _taskService;

    public ReportMenu(TaskService taskService)
    {
        _taskService = taskService;
        _reportService = new TaskReportService(taskService);
        _exporter = new CsvReportExporter();
    }

    public void Run()
    {
        Console.WriteLine("=== Отчёты ===");
        Console.WriteLine("1. Сводка");
        Console.WriteLine("2. Просроченные задачи");
        Console.WriteLine("3. Экспорт CSV");
        Console.Write("Выберите действие: ");

        var command = Console.ReadLine();

        switch (command)
        {
            case "1":
                Console.WriteLine(_reportService.BuildSummary());
                break;
            case "2":
                PrintOverdue();
                break;
            case "3":
                _exporter.Export(_taskService.GetAll());
                break;
            default:
                Console.WriteLine("Неизвестная команда.");
                break;
        }
    }

    private void PrintOverdue()
    {
        var tasks = _reportService.GetOverdueTasks();

        if (tasks.Count == 0)
        {
            Console.WriteLine("Просроченных задач нет.");
            return;
        }

        foreach (var task in tasks)
        {
            Console.WriteLine($"{task.Id} {task.Title} (срок {task.DueDate:dd.MM.yyyy})");
        }
    }
}
