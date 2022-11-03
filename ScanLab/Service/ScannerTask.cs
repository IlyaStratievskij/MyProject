namespace Service;

// ����� �� ����� �������, � �������� ������������
class ScannerTask
{
    // ��������� �������
    public bool Completed { get; private set; }
    // �������
    public string DirectoryPath { get; }
    // ���-�� ������������� ������
    public int ProcessedFilesCount { get; private set; }
    // ������
    public int ErrorsCount { get; private set; }
    // ������ �� �������� ����������� "���������������" ����
    public List<Trigger> Detects { get; }

    // ��� ����������� ������� ������
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