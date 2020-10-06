using System;
using System.IO;
using Sirkadirov.Overtest.Libraries.Shared.Methods;

namespace Sirkadirov.Overtest.TestingDaemon.Services.Storage
{
    
    public class TempDirectoryAccessPoint : IDisposable
    {
        
        public string DirectoryFullName { get; }

        public TempDirectoryAccessPoint(string path)
        {
            DirectoryFullName = path;
            Directory.CreateDirectory(path);
        }
        
        public void Dispose()
        {
            
            FileSystemSharedMethods.SecureDeleteDirectory(DirectoryFullName);
            
        }
        
    }
    
}