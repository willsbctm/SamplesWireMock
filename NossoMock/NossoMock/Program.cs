using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace NossoMock
{
    class Program
    {
        static void Main(string[] args)
        {

            while(true)
            {
                Console.WriteLine("Informe o path");
                var path = Console.ReadLine();
                var server = WireMock.Server.WireMockServer.Start(8086);
                var logEntries = server.LogEntries;

                var taskLog = Task.Run(() =>
                {
                    var log = 0;
                    while (true)
                    {
                        var logs = server.LogEntries.ToList();
                        if (logs.Count > 0)
                        {
                            server.DeleteLogEntry(logs[log].Guid);
                        }
                    };
                });

                Console.WriteLine("Informe a chave");
                var chave = Console.ReadLine();

                Console.WriteLine("Fazendo request");
                server
                .Given(Request.Create()
                .WithPath($"/api/{path}")
                .WithParam($"parametro", "1234")
                .UsingGet())
                .RespondWith(Response.Create()
                .WithStatusCode(StatusCodes.Status200OK)
                .WithBody(request =>
                {
                    Console.WriteLine("Deu match");
                    return "oi";
                }));

                var url = $"http://localhost:8086/api/{chave}?parametro=1234";
                var httpClient = new HttpClient();
                var resultado = httpClient.GetAsync(url).GetAwaiter().GetResult();
                Console.WriteLine($"{resultado.StatusCode}: {resultado.Content.ReadAsStringAsync().GetAwaiter().GetResult()}");
                server.Dispose();
            }
        }
    }
}
