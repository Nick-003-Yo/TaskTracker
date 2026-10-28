using System.Text;
using TaskTracker.Models;

namespace TaskTracker.Modules.Reports;

public class CsvReportExporter
{
    public void Export(IEnumerable<TaskItem> tasks)
    {
        var directory = Path.Combine(Directory.GetCurrentDirectory(), "reports");
        var path = Path.Combine(directory, "tasks.csv");

        try
        {
            Directory.CreateDirectory(directory);

            var lines = new List<string>
            {
                "Id,Title,Status,Priority,Assignee,DueDate"
            };

            foreach (var task in tasks)
            {
                lines.Add(string.Join(",",
                    task.Id,
                    Escape(task.Title),
                    task.Status,
                    task.Priority,
                    Escape(task.Assignee),
                    task.DueDate.HasValue ? task.DueDate.Value.ToString("yyyy-MM-dd") : string.Empty));
            }

            File.WriteAllLines(path, lines, new UTF8Encoding(true));
            Console.WriteLine("Отчёт сохранён: " + path);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка экспорта: " + ex.Message);
        }
    }

    private static string Escape(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            return "\"" + value.Replace("\"", "\"\"") + "\"";

        return value;
    }
}
