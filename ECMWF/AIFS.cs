/*
This script scans for the latest available ECMWF data and downloads it to your computer.

(C) Eric J. Drewitz 2025
*/

using WxData_Client.Utilities;


namespace WxData_Client.ECMWF
{


    public class FileDownloader
    {

        /*
        This class downloads the ECMWF AIFS data and saves the data to a folder at path

        f:ECMWF/AIFS

        */

        private static async Task DownloadFileAsync(string fileUrl, string localFilePath)
        {

            /*
            HTTPS Client that downloads the data and saves the data to the folder.

            Required Arguments:

            1) fileUrl (String) - The URL of the data file

            2) localFilePath (String) - The local file path on the computer where the data will be stored. 
            */

            using HttpClient client = new();
            try
            {

                // Get the file stream from the URL
                using (Stream contentStream = await client.GetStreamAsync(fileUrl))
                {
                    // Create a FileStream to save the content to the local path
                    using (FileStream fileStream = new(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        // Copy the content stream to the file stream
                        await contentStream.CopyToAsync(fileStream);
                    }
                }
                // Prints success message to the user
                Console.WriteLine($"File downloaded successfully to: {localFilePath}");
            }
            catch (HttpRequestException e)
            {
                // Prints HTTPS error to the user.
                Console.WriteLine($"Error downloading file: {e.Message}");
                Console.WriteLine("Waiting 30 seconds...");
                Thread.Sleep(30000);

                for (int i = 0; i < 6; i++)
                {
                    try
                    {
                        using Stream contentStream = await client.GetStreamAsync(fileUrl);
                        // Create a FileStream to save the content to the local path
                        using FileStream fileStream = new(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                        // Copy the content stream to the file stream
                        await contentStream.CopyToAsync(fileStream);
                        Console.WriteLine($"File downloaded successfully to: {localFilePath}");
                        break;
                    }
                    catch
                    {
                        // Prints success message to the user
                        Console.WriteLine("Unable to reconnect.\nIncreasing time between reconnect attempts to 60 seconds...");
                        Console.WriteLine($"Retries Remaining: {5 - i}");
                        Thread.Sleep(60000);
                    }

                }

            }
            catch (Exception e)
            {
                // Prints error to the user. 
                Console.WriteLine($"An unexpected error occurred: {e.Message}");
            }
        }

        public static async Task DownloadECMWFAIFS(int finalForecastHour)
        {
            /*
            In this section of code, we make our lists of URLs and fileNames. 

            Next we loop through the URL list with our HTTPS Client we created above. 

            Files save to f:ECMWF/AIFS/{fileName}
            */

            DateTime utcNow = DateTime.UtcNow;
            DateTime yDay = utcNow.AddDays(-1);
            int hour = utcNow.Hour;
            string run = "";
            DateTime time = utcNow;

            if ((hour >= 6) && (hour < 12))
            {
                run = "00";
            }
            else if ((hour >= 12) && (hour < 18))
            {
                run = "06";
            }
            else if ((hour >= 18) && (hour < 24))
            {
                run = "12";
            }
            else
            {
                run = "18";
                time = yDay;
            }

            List<string> url_list = [];

            for (int i = 0; i < (finalForecastHour + 6); i += 6)
            {
                string u = $"https://data.ecmwf.int/forecasts/{time.ToString("yyyyMMdd")}/{run}z/aifs-single/0p25/oper/{time.ToString("yyyyMMdd")}{run}0000-{i}h-oper-fc.grib2";

                url_list.Add(u);
            }

            List<string> file_list = [];

            for (int i = 0; i < (finalForecastHour + 6); i += 6)
            {
                string f = $"{time.ToString("yyyyMMdd")}{run}0000-{i}h-oper-fc.grib2";
                file_list.Add(f);
            }

            string directoryPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string folderName = $"Weather Data/ECMWF/AIFS/";
            string displayFolderName = $@"Weather Data\ECMWF\AIFS\";
            string fullPath = Path.Combine(directoryPath, folderName);
            string displayPath = Path.Combine(directoryPath, displayFolderName);

            List<string> paths = [];
            for (int i = 0; i < file_list.Count; i++)
            {
                string path = $"{fullPath}{file_list[i]}";
                paths.Add(path);
            }

            if (!Directory.Exists($"{fullPath}"))
            {
                Directory.CreateDirectory($"{fullPath}");
                Console.WriteLine($"Folder {displayPath}' created");
            }
            else
            {
                Console.WriteLine($"Folder '{displayPath}' already exists");
            }

            ClearFiles.DeleteFiles(fullPath, displayPath);

            for (int a = 0; a < url_list.Count; a++)
            {
                try
                {
                    await DownloadFileAsync(url_list[a], paths[a]);
                }
                catch
                {
                    try
                    {
                        for (int r = 0; r < 6; r++)
                        {

                            Console.WriteLine($"Network Connection disrupted\nWaiting 30 seconds and reconnecting.\nRetries left: {5 - r}");
                            Thread.Sleep(30000);
                            await DownloadFileAsync(url_list[a], paths[a]);
                            bool success = true;
                            if (success == true)
                            {
                                break;
                            }
                            else
                            {
                                
                            }

                        }

                    }
                    catch
                    {
                        Console.WriteLine("Cannot reconnect. Exiting....");
                        
                    }

                }

            }
        }

    }
    
}
