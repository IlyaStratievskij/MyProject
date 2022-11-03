namespace Service;

public static class FileSystemScanMethods
{
    public static async Task ScanAllInDirectoryAsync(string directoryPath, List<Trigger> triggersCounts,
        ScanAdditionalResults scanAdditionalResults)
    {
        try
        {
            // ������ ������������ ������������ ������, ��� �������� ����� �����
            await Parallel.ForEachAsync(Directory.GetFiles(directoryPath),
                async (filePath, cancellationToken) => await ScanFile(filePath, triggersCounts, scanAdditionalResults));

            // ������ ������������ ������������ ������, ��� �������� ����� �������� ����������
            await Parallel.ForEachAsync(Directory.GetDirectories(directoryPath),
                async (subDirectoryPath, cancellationToken) => await ScanAllInDirectoryAsync(subDirectoryPath, triggersCounts, scanAdditionalResults));
        }
        catch (Exception)
        {
            scanAdditionalResults.ErrorsCount++;
#if DEBUG
            throw;
#endif
        }
    }

    // ���� ������������ ������
    public static async Task ScanFile(string filePath, List<Trigger> triggersCounts,
        ScanAdditionalResults scanAdditionalResults)
    {
        try
        {
            
            var needToSearch = triggersCounts.Where(x =>
                x.FileTypes.Length == 0 || x.FileTypes.Contains(Path.GetExtension(filePath)));
            
            char[] buffer = new char[1 << 14];

            int lastBufferLength = needToSearch.Max(x => x.Text.Length) - 1;

            char[] lastBuffer = Array.Empty<char>();

            // ��������� ���� � �����
            using var streamReader = File.OpenText(filePath);

            int blockLength;
            do
            {
                lastBuffer = buffer.TakeLast(lastBufferLength).ToArray();

                blockLength = await streamReader.ReadBlockAsync(buffer, 0, buffer.Length);

                string fullStr = new string(lastBuffer) + new string(buffer);

                foreach (var trigger in needToSearch)
                {
                    if (fullStr.Contains(trigger.Text))
                    {
                        trigger.FoundsCount++;
                        scanAdditionalResults.ProcessedFilesCount++;
                        return;
                    }
                    
                }
            } while (blockLength == buffer.Length); // �� ������������� ������� ������� 16k bit
            scanAdditionalResults.ProcessedFilesCount++;
        }
        catch (Exception)
        {
            scanAdditionalResults.ErrorsCount++;
        }
    }
}