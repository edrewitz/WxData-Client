/*
This file hosts the functions that work with managing file directories.

(C) Eric J. Drewitz 2025
*/


namespace fileFunctions
{
    public static class clearFiles
    {
        /*
        This class clears the contents of a directory when downloading new data.
        */

        public static void Main(string fullPath, string displayPath)
        {
            if (Directory.Exists(fullPath))
            {
                string[] filePaths = Directory.GetFiles(fullPath);
                foreach (string filePath in filePaths)
                {
                    try
                    {
                        File.Delete(filePath);
                        Console.WriteLine($"Deleted: {displayPath}");
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine($"Error deleting {filePath}: {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Directory not found: {displayPath}");
            }
        }
    }
}