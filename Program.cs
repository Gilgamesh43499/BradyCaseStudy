using GeneratorDataProcessor.Interfaces;
using GeneratorDataProcessor.Services;
using GeneratorDataProcessor.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace GeneratorDataProcessor
{
    public class Program
    {

        public static async Task Main(string[] args)
        {
            using var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                }).
                ConfigureServices((context, services) =>
                {
                    var configuration = context.Configuration;
                    string referenceDataFile = configuration["Settings:ReferenceDataFilePath"] ?? throw new ArgumentNullException("Settings:ReferenceDataFilePath");
                    referenceDataFile = Path.Combine(Directory.GetCurrentDirectory(), referenceDataFile);
                    var referenceData = XMLHelper.ParseReferenceData(referenceDataFile);
                    services.AddSingleton<IFileProcesssing>(new FilePRocessingService(referenceData));
                    services.AddHostedService<FileWatcherSerivce>();
                }).
                ConfigureLogging(logging =>
                {
                    logging.AddConsole();
                })
                .Build();
            await host.RunAsync();
        }

        //moved to the FileWatcherService
        private static void OnFileCreated(object sender, FileSystemEventArgs e)
        {
        }

        //moved to the FileWatcherService
        private static void ProcessExistingFiles(string inputFolder, IFileProcesssing processingService, string outputFolder)
        {
        }

    }
}
