using System.Text.Json;
using TaskTracker.Models;

namespace TaskTracker.Services;

public class TaskStorage
{
    private readonly string filePath;

    public TaskStorage(string filePath)
    {
        this.filePath = filePath;
    }

    public List<TaskItem> Load()
    {
        if (!File.Exists(filePath))
            return new List<TaskItem>();

        var json = File.ReadAllText(filePath);

        if (string.IsNullOrWhiteSpace(json))
            return new List<TaskItem>();

        return JsonSerializer.Deserialize<List<TaskItem>>(json)
               ?? new List<TaskItem>();
    }

    public void Save(List<TaskItem> tasks)
    {
        var directory = Path.GetDirectoryName(filePath);

        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(
            tasks,
            new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

        File.WriteAllText(filePath, json);
    }
}
