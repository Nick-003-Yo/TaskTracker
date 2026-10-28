using System.Text;
using TaskTracker.Models;

namespace TaskTracker.Modules.Reports;

public class CsvReportExporter
{
    public void Export(IEnumerable<TaskItem> tasks)
    {
        var path = @"C:\TaskTracker\reports\tasks.csv";

        try
        {
            var lines = new List<string>
            {
                "Id,Title,Status,Priority,Assignee,DueDate"
            };

            foreach (var task in tasks)
            {
                lines.Add(string.Join(",",
                    task.Id,
                    task.Title,
                    task.Status,
                    task.Priority,
                    task.Assignee,
                    task.DueDate));
            }

            File.WriteAllLines(path, lines, Encoding.UTF8);
            Console.WriteLine("Отчёт сохранён: " + path);
        }
        catch
        {
        }
    }
}
