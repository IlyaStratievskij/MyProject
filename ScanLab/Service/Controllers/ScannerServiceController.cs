using Microsoft.AspNetCore.Mvc;

namespace Service.Controllers;

[ApiController]
[Route("[controller]")]
public class ScannerServiceController : ControllerBase
{
    
    // ������ id ��� ������ �������
    [HttpPost("Scan")]
    public int Scan([FromBody] string fileSystemPath)
    {
        int id = Random.Shared.Next();

        if (ScannerTasksManager.GetInstanse().TryToAddAndRun(id, fileSystemPath))
        {
            return id;
        }

        return -1;
    }

    // ������� ������ � ����������� �� ScannerTasksManager
    [HttpGet("Status")]
    public string Status([FromQuery] int taskId)
    {
        return ScannerTasksManager.GetInstanse().GetTaskInfoById(taskId);
    }
}