namespace Service;

// класс со всеми данными, о заданном сканировании
class ScannerTask
{
    // состояние запроса
    public bool Completed { get; private set; }
    // каталог
    public string DirectoryPath { get; }
    // кол-во просмотренных файлов
    public int ProcessedFilesCount { get; private set; }
    // ошибки
    public int ErrorsCount { get; private set; }
    // список из заданных обнаружений "подозрительного" типа
    public List<Trigger> Detects { get; }

    // для определения времени работы
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public ScannerTask(List<Trigger> targets, string directoryPath)
    {
        Completed = false;
        
        DirectoryPath = directoryPath;
        Detects = targets;
    }

    public async void Run(Func<string, List<Trigger>, ScanAdditionalResults, Task> scannerMethod)
    {
        StartTime = DateTime.Now;
        try
        {
            if (Completed)
                return;
            var scanAdditionalResults = new ScanAdditionalResults();
            await scannerMethod(DirectoryPath, Detects, scanAdditionalResults);

            ProcessedFilesCount = scanAdditionalResults.ProcessedFilesCount;
            ErrorsCount = scanAdditionalResults.ErrorsCount;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        
        EndTime = DateTime.Now;
        Completed = true;
    }
}