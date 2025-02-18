using GeneratorDataProcessor.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;
using System.Threading.Tasks.Dataflow;

namespace GeneratorDataProcessor.Services
{
    public class FileWatcherSerivce : IHostedService, IDisposable
    {
        private readonly ILogger<FileWatcherSerivce> _logger;
        private readonly IConfiguration _configuration;
        private readonly IFileProcesssing _fileProcessingService;
        private FileSystemWatcher _watcher;
        private ActionBlock<string> _fileProcessingBlock;
        private readonly string _inputFolder;
        private readonly string _outputFolder;

        public FileWatcherSerivce(ILogger<FileWatcherSerivce> logger, IConfiguration configuration, IFileProcesssing fileProcessingService)
        {
            _logger = logger;
            _configuration = configuration;
            _fileProcessingService = fileProcessingService;
            _inputFolder = _configuration["Settings:InputFolderPath"] ?? throw new ArgumentNullException("Settings:InputFolderPath");
            _outputFolder = _configuration["Settings:OutputFolderPath"] ?? throw new ArgumentNullException("Settings:OutputFolderPath");
           
            if (!Directory.Exists(_inputFolder))
                Directory.CreateDirectory(_inputFolder);

            if (!Directory.Exists(_outputFolder))
                Directory.CreateDirectory(_outputFolder);

            _fileProcessingBlock = new ActionBlock<string>(async filePath =>
            {
                await ProcessFileAsync(filePath);
            },
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = 1
            });

            _watcher = new FileSystemWatcher
            {
                Path = _inputFolder,
                Filter = "*.xml",
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                EnableRaisingEvents = false,
            };

            _watcher.Created += OnCreated;

        }
        public void Dispose()
        {
            _watcher.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("File Watcher Service started");
            var existingFiles = Directory.GetFiles(_inputFolder, "*.xml");

            foreach (var file in existingFiles)
            {
                Console.WriteLine($"Found existing file: {file}");
                _fileProcessingService.ProcessGenerationReport(file, _outputFolder);
            }
            _watcher.EnableRaisingEvents = true;
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("File Watcher Service stopped");
            _watcher.EnableRaisingEvents = false;
            _watcher.Dispose();
            _fileProcessingBlock.Complete();
            await _fileProcessingBlock.Completion;
        }
        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation($"File created: {e.FullPath}");
            _fileProcessingBlock.Post(e.FullPath);
        }
        private  async Task ProcessFileAsync(string filePath)
        {
            var policy = Policy.Handle<IOException>()
                        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        (exception, timeSpan, retryCount, context) =>
                        {
                            _logger.LogWarning($"Error processing file: {filePath}. Retry count: {retryCount}");
                        });
            await policy.ExecuteAsync(() =>
            {
              using (var openFile = new FileStream(filePath,FileMode.Open , FileAccess.Read,FileShare.None))
              {
                  
              }
              return Task.CompletedTask;
            });
            try
            {
                await Task.Run(() =>
                {
                    _fileProcessingService?.ProcessGenerationReport(filePath, _outputFolder);
                    _logger.LogInformation($"File processed: {filePath}");
                });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error processing file: {filePath}");
            }
        }
    }
}
