using backend_microondas.App_Start;
using backend_microondas.Data;
using Microsoft.Owin.Hosting;
using System;
using System.Configuration;

namespace backend_microondas
{
    class Program
    {
        static void Main()
        {
            try
            {
                var config = new ServerConfiguration();

                Console.WriteLine("=== MICROONDAS API ===");
                Console.WriteLine($"Servidor iniciando em: {config.BaseUrl}");
                Console.WriteLine($"CORS habilitado para: {config.AllowedOrigins}");
                Console.WriteLine("======================");

                using (WebApp.Start<Startup>(url: config.BaseUrl))
                {
                    Console.WriteLine("Servidor rodando com sucesso!");
                    Console.WriteLine("Pressione ENTER para parar o servidor...");
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao iniciar servidor: {ex.Message}");
                Console.WriteLine("Pressione ENTER para sair...");
                Console.ReadLine();
            }
        }
    }

    public class ServerConfiguration
    {
        public string BaseUrl { get; }
        public string AllowedOrigins { get; }

        public ServerConfiguration()
        {
            BaseUrl = ConfigurationManager.AppSettings["BaseUrl"] ?? "http://localhost:7000/";
            AllowedOrigins = ConfigurationManager.AppSettings["AllowedOrigins"] ?? "http://localhost:4200";
        }
    }
}