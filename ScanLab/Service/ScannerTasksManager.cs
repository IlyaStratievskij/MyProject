using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Extensions;

namespace Service;

public class ScannerTasksManager
{
    private static Dictionary<int, ScannerTask> Tasks = default!;

    private static ScannerTasksManager? Instanse { get; set; } = null;
    
    public static ScannerTasksManager GetInstanse()
    {
        if (Instanse == null)
        {
            Instanse = new ScannerTasksManager();
        }

        return Instanse;
    }

    private ScannerTasksManager()
    {
        Tasks = new Dictionary<int, ScannerTask>();
    }

    public bool TryToAddAndRun(int id, string directoryPath)
    {
        if(!Directory.Exists(directoryPath))
            return false;

        Func<string, List<Trigger>, ScanAdditionalResults, Task> scanMethod =
            FileSystemScanMethods.ScanAllInDirectoryAsync;

        // задаЄм новое сканирование, отправл€ем искомые триггеры, и путь к директории
        ScannerTask scannerTask = new ScannerTask(TriggersManager.GetNewTriggers(), directoryPath);

        scannerTask.Run(scanMethod);
        
        Tasks.Add(id, scannerTask);
        
        return true;
    }

    public string GetTaskInfoById(int id)
    {
        // проверка наличи€ id
        if (!Tasks.ContainsKey(id))
            return "Task does not exists";
        
        // проверка, выполнилс€ ли запрос
        var task = Tasks[id];
        if (!task.Completed)
            return "Scan task in progress, please wait";

        StringBuilder sb = new();

        // заданные параметры дл€ вывода строки
        sb.AppendLine($"Directory: {task.DirectoryPath}");
        sb.AppendLine($"Processed Files: {task.ProcessedFilesCount}");
        foreach (var detect in task.Detects)
        {
            sb.AppendLine($"{detect.Name} detects: {detect.FoundsCount}");
        }

        sb.AppendLine($"Errors: {task.ErrorsCount}");
        sb.AppendLine($"Execution time: { (task.EndTime - task.StartTime).ToString(@"hh\:mm\:ss") }");
        
        return sb.ToString();
    }
    
}