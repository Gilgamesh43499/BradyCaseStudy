using GeneratorDataProcessor.Interfaces;
using GeneratorDataProcessor.Services;
using GeneratorDataProcessor.Utilities;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;

namespace GeneratorDataProcessor
{
    public class Program
    {
        private static IConfiguration? _configuration;
        private static FileSystemWatcher? _watcher;

        // Using the interface from your SOLID approach:
        private static IFileProcesssing? _processingService;
        private static readonly object _lockObj = new object();

        public static void Main(string[] args)
        {
            Console.WriteLine("=== .NET 8 SOLID/Strategy Console App ===");


            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();


            string inputFolder = _configuration["Settings:InputFolderPath"] ?? throw new ArgumentNullException("Settings:InputFolderPath");
            string outputFolder = _configuration["Settings:OutputFolderPath"] ?? throw new ArgumentNullException("Settings:OutputFolderPath");
            string referenceDataFile = _configuration["Settings:ReferenceDataFilePath"] ?? throw new ArgumentNullException("Settings:ReferenceDataFilePath");
            referenceDataFile = Path.Combine(Directory.GetCurrentDirectory(), referenceDataFile);
            Console.WriteLine("Configuration Loaded:");
            Console.WriteLine($"  Input Folder: {inputFolder}");
            Console.WriteLine($"  Output Folder: {outputFolder}");
            Console.WriteLine($"  Reference Data File: {referenceDataFile}");
            Console.WriteLine();

            var referenceData = XMLHelper.ParseReferenceData(referenceDataFile);
            _processingService = new FilePRocessingService(referenceData);

            if (string.IsNullOrEmpty(referenceDataFile))
            {
                throw new ArgumentNullException(nameof(referenceDataFile), "Reference data file path cannot be null or empty.");
            }

            if (!Directory.Exists(inputFolder))
                Directory.CreateDirectory(inputFolder);

            if (!Directory.Exists(outputFolder))
                Directory.CreateDirectory(outputFolder);

            ProcessExistingFiles(inputFolder, _processingService, outputFolder);

            _watcher = new FileSystemWatcher
            {
                Path = inputFolder,
                Filter = "*.xml",
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
            };

            _watcher.Created += OnFileCreated;
            _watcher.EnableRaisingEvents = true;

            Console.WriteLine($"Watching '{inputFolder}' for new XML files...");
            Console.WriteLine("Press [Enter] to quit.");
            Console.ReadLine();

            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
        }

        private static void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            // Some files may still be in the process of being written.
            // We add a short delay. In production, consider more robust approaches.
            Thread.Sleep(1000);

            lock (_lockObj)
            {
                Console.WriteLine($"New file detected: {e.FullPath}");
                lock (_lockObj)
                {
                    if (_processingService == null)
                    {
                        throw new InvalidOperationException("Processing service is not initialized.");
                    }

                    string? outputFolderPath = _configuration?["Settings:OutputFolderPath"];
                    if (string.IsNullOrEmpty(outputFolderPath))
                    {
                        throw new InvalidOperationException("Output folder path is not configured.");
                    }

                    _processingService.ProcessGenerationReport(e.FullPath, outputFolderPath);
                }
            }
        }
        private static void ProcessExistingFiles(string inputFolder, IFileProcesssing processingService, string outputFolder)
        {
            var existingFiles = Directory.GetFiles(inputFolder, "*.xml");

            foreach (var file in existingFiles)
            {
                Console.WriteLine($"Found existing file: {file}");
                processingService.ProcessGenerationReport(file, outputFolder);
            }
        }

    }
}
