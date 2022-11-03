// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Net.Http.Json;

string serviceControllerAddress = "https://localhost:5001/ScannerService";

string pathToService = @"C:\Users\strat\OneDrive\Рабочий стол\ScanLab1\Service\bin\Debug\net6.0\Service.exe"; // путь к сервису
bool isServiceStarted = false; // запущен ли сервис

string? input; // допускает null 

while(true)
{
    input = Console.ReadLine();
    if (input == null || input == "") // если нажата клавиша Enter то выполнение заканчивается
        break;
    
    string[] inputWords = input.Split(" ");
    switch (inputWords[0])
    {
        case "scan_service":
            if (isServiceStarted)
            {
                Console.WriteLine("Scan service started yet");
                break;
            }
            
            ProcessStartInfo processStartInfo = new ProcessStartInfo(pathToService);
            // запуск самого процесса
            Process.Start(processStartInfo);
            
            Console.WriteLine("Scan service was started");
            
            break;
        case "scan_util":
            switch (inputWords[1])
            {
                case "scan":

                    // новый клиент для отправки запросов
                    using (HttpClient httpClient = new HttpClient())
                    {
                        // отправляем post запрос ( отправка кода ресурса и предоставления содержимого в json )
                        using var responce = await httpClient.PostAsync(new Uri($"{serviceControllerAddress}/Scan"),
                            JsonContent.Create(inputWords[2]));
                        
                        Console.WriteLine(await responce.Content.ReadAsStringAsync());
                        
                    }
                    
                    break;
                case "status":
                    
                    using (HttpClient httpClient = new HttpClient())
                    {
                        // отправляем get запрос для получения данных о статусе по id
                        using var responce = await httpClient.GetAsync(new Uri($"{serviceControllerAddress}/Status?taskId={inputWords[2]}"));
                        
                        Console.WriteLine(await responce.Content.ReadAsStringAsync());
                        
                    }
                    
                    break;
            }
            break;
    }
    
    Console.WriteLine("Press <Enter> to exit");
}