namespace Service;

public class ScanAdditionalResults
{

    private readonly object _forPFCLock = new();
    // всего просмотренных файлов
    private int _processedFilesCount;
    public int ProcessedFilesCount
    {
        get => _processedFilesCount;
        set
        {
            // компилятор ругается, лучше заблокировать заданный объект на время присваивания
            lock (_forPFCLock)
            {
                _processedFilesCount = value;
            }
        }
    }
    
    private readonly object _forECLock = new();
    private int _errorsCount;
    public int ErrorsCount
    {
        get => _errorsCount;
        set
        {
            lock (_forECLock)
            {
                _errorsCount = value;
            }
        }
    }
}