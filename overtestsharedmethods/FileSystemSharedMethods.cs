using System.IO;

namespace Sirkadirov.Overtest.Libraries.Shared.Methods
{
    
    public static class FileSystemSharedMethods
    {

        public static void SecureRecreateDirectory(string path)
        {
            
            SecureDeleteDirectory(path);

            Directory.CreateDirectory(path);

        }
        
        public static void SecureDeleteDirectory(string path)
        {

            if (!Directory.Exists(path))
                return;
            
            foreach (var directoryPath in Directory.GetDirectories(path))
            {
                SecureDeleteDirectory(directoryPath);
            }
            
            foreach (var filePath in Directory.GetFiles(path))
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
            }
            
            Directory.Delete(path);
            
        }
        
        public static void SecureCopyDirectory(string sourceDirectoryPath, string destinationDirectoryPath)
        {
            
            // Get information about the source directory
            var sourceDirectoryInfo = new DirectoryInfo(sourceDirectoryPath);
            
            // Check whether source directory exists
            if (!sourceDirectoryInfo.Exists)
                throw new DirectoryNotFoundException();
            
            // Try to create destination directory
            Directory.CreateDirectory(destinationDirectoryPath);
            
            // Copy files to a new destination
            foreach (var directoryFile in sourceDirectoryInfo.GetFiles())
            {
                directoryFile.CopyTo(Path.Combine(destinationDirectoryPath, directoryFile.Name), true);
            }
            
            // Recursive copy subdirectories
            foreach (var subDirectory in sourceDirectoryInfo.GetDirectories())
            {
                SecureCopyDirectory(subDirectory.FullName, Path.Combine(destinationDirectoryPath, subDirectory.Name));
            }
            
        }

    }
    
}