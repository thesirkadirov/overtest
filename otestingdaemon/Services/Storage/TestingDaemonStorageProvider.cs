using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NLog;
using Sirkadirov.Overtest.Libraries.Shared.Database;
using Sirkadirov.Overtest.Libraries.Shared.Methods;

namespace Sirkadirov.Overtest.TestingDaemon.Services.Storage
{
    
    public class TestingDaemonStorageProvider
    {
        
        private readonly IConfiguration _configuration;
        private readonly OvertestDatabaseContext _databaseContext;
        private readonly ILogger _logger;

        private static readonly object TestingDataActualizationLocker;

        static TestingDaemonStorageProvider()
        {
            TestingDataActualizationLocker = new object();
        }
        
        public TestingDaemonStorageProvider(IConfiguration configuration, OvertestDatabaseContext databaseContext)
        {
            _configuration = configuration;
            _databaseContext = databaseContext;
            _logger = LogManager.GetCurrentClassLogger();
        }
        
        public async Task<bool> ActualizeTestingData()
        {
            
            _logger.Info("Got a request to actualize testing data for all tasks.");
            
            var programmingTasksList = await _databaseContext.ProgrammingTasks.Select(t => t.Id).ToListAsync();

            var updatedCount = 0;
            
            foreach (var task in programmingTasksList)
            {
                if (await ActualizeTestingData(task))
                {
                    _logger.Info($"Task's {task} testing data was out of date, now successfully updated!");
                    updatedCount++;
                }
            }
            
            _logger.Info($"Programming tasks in the database: {programmingTasksList.Count}, updated: {updatedCount}.");
            
            return updatedCount > 0;

        }

        public async Task<bool> ActualizeTestingData(Guid programmingTaskId)
        {
            
            // No need to actualize testing data
            if (await IsActualTestingData(programmingTaskId))
                return false;
            
            // Get local testing data path
            var testingDataLocalPath = GetTestingDataPathByProgrammingTaskId(programmingTaskId);
            
            // Delete old and create new directory for testing data
            FileSystemSharedMethods.SecureRecreateDirectory(testingDataLocalPath);
            
            // Query actual testing data and it's hash code
            var queryResult = await _databaseContext.ProgrammingTaskTestingDatas
                .Where(d => d.ProgrammingTaskId == programmingTaskId)
                .Select(d => new {d.DataPackageFile, d.DataPackageHash})
                .FirstOrDefaultAsync();
            
            // ReSharper disable once ConvertToUsingDeclaration
            await using (var fileStream = new MemoryStream(queryResult.DataPackageFile))
            {
                
                // Testing data is stored as ZIP archive, we need to read it
                using var zipArchive = new ZipArchive(fileStream, ZipArchiveMode.Read, false, Encoding.UTF8);
                
                // Extract archive contents to newly created directory
                zipArchive.ExtractToDirectory(testingDataLocalPath, true);
                
            }
            
            // Save testing data hash into a file
            await File.WriteAllTextAsync(
                Path.Combine(
                    testingDataLocalPath,
                    _configuration.GetValue<string>("general:storage:custom_naming:hash_file")
                ),
                queryResult.DataPackageHash,
                Encoding.UTF8
            );
            
            // Data actualized
            return true;
            
        }
        
        public async Task<bool> IsActualTestingData(Guid programmingTaskId)
        {

            try
            {
                
                _logger.Debug($"Executing {nameof(IsActualTestingData)} for programming task {programmingTaskId}...");
                
                var externalHash = await _databaseContext.ProgrammingTaskTestingDatas
                    .Where(d => d.ProgrammingTaskId == programmingTaskId)
                    .Select(d => d.DataPackageHash)
                    .FirstOrDefaultAsync();

                var testingDataPath = GetTestingDataPathByProgrammingTaskId(programmingTaskId);

                if (!Directory.Exists(testingDataPath))
                {
                    _logger.Debug($"{nameof(IsActualTestingData)}: No cache directory found for task {programmingTaskId}! Returning {false.ToString()}.");
                    return false;
                }
            
                var localHash = await File.ReadAllTextAsync(
                    Path.Combine(
                        testingDataPath,
                        _configuration.GetValue<string>("general:storage:custom_naming:hash_file")
                    )
                );

                var isActual = localHash == externalHash;
                
                _logger.Debug($"{nameof(IsActualTestingData)}: programming task {programmingTaskId} local hash: {localHash}, external hash: {externalHash}. {(isActual ? "Actual" : "Out-of-date")}.");

                return isActual;

            }
            catch (Exception ex)
            {
                
                _logger.Warn(
                    ex,
                    $"An exception catched during {nameof(IsActualTestingData)} execution for programming task {programmingTaskId}! Marking as out-of-date."
                );
                
                return false;
            }

        }

        public TempDirectoryAccessPoint GetTestingDataAccessPoint(Guid programmingTaskId)
        {
            
            var dataDirectoryConfiguration = _configuration.GetSection("general:storage");
            
            var testingDataPath = GetTestingDataPathByProgrammingTaskId(programmingTaskId);
            
            var temporaryAccessPointPath = Path.Combine(
                dataDirectoryConfiguration.GetValue<string>("path"),
                dataDirectoryConfiguration.GetValue<string>("custom_naming:temp_directory"),
                Guid.NewGuid().ToString()
            );
            
            FileSystemSharedMethods.SecureCopyDirectory(testingDataPath, temporaryAccessPointPath);
            
            return new TempDirectoryAccessPoint(temporaryAccessPointPath);
            
        }

        private string GetTestingDataPathByProgrammingTaskId(Guid programmingTaskId, bool throwIfNotExist = false)
        {
            
            var dataDirectoryConfiguration = _configuration.GetSection("general:storage");
            
            var path = Path.Combine(
                dataDirectoryConfiguration.GetValue<string>("path"),
                dataDirectoryConfiguration.GetValue<string>("custom_naming:testing_data_directory"),
                programmingTaskId.ToString()
            );

            if (!throwIfNotExist)
                return path;

            return Directory.Exists(path) ? path : throw new DirectoryNotFoundException();

        }
        
    }
    
}