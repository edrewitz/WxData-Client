/*
This script scans for the latest available ECMWF data and downloads it to your computer.

(C) Eric J. Drewitz 2025
*/

using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using Microsoft.VisualBasic;


public class FileDownloader
{

    /*
    This class downloads the ECMWF AIFS data and saves the data to a folder at path

    f:ECMWF/AIFS

    */

    public static async Task DownloadFileAsync(string fileUrl, string localFilePath)
    {

        /*
        HTTPS Client that downloads the data and saves the data to the folder.

        Required Arguments:

        1) fileUrl (String) - The URL of the data file

        2) localFilePath (String) - The local file path on the computer where the data will be stored. 
        */

        using (HttpClient client = new HttpClient())
        {
            try
            {

                // Get the file stream from the URL
                using (Stream contentStream = await client.GetStreamAsync(fileUrl))
                {
                    // Create a FileStream to save the content to the local path
                    using (FileStream fileStream = new FileStream(localFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
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
            }
            catch (Exception e)
            {
                // Prints error to the user. 
                Console.WriteLine($"An unexpected error occurred: {e.Message}");
            }
        }
    }

    public static async Task Main(string[] args)
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

        List<string> url_list = new List<string>();

        for (int i = 0; i < 366; i += 6)
        {
            string u = $"https://data.ecmwf.int/forecasts/{time.ToString("yyyyMMdd")}/{run}z/aifs-single/0p25/oper/{time.ToString("yyyyMMdd")}{run}0000-{i}h-oper-fc.grib2";

            url_list.Add(u);
        }

        List<string> file_list = new List<string>();

        for (int i = 0; i < 366; i += 6)
        {
            string f = $"{time.ToString("yyyyMMdd")}{run}0000-{i}h-oper-fc.grib2";

            file_list.Add(f);
        }


        List<string> paths = new List<string>();
        for (int i = 0; i < file_list.Count; i++)
        {
            string path = $"ECMWF/AIFS/{file_list[i]}";
            paths.Add(path);
        }

        if (!Directory.Exists($"ECMWF/AIFS"))
        {
            Directory.CreateDirectory($"ECMWF/AIFS");
            Console.WriteLine($"Folder 'ECMWF/AIFS' created");
        }
        else
        {
            Console.WriteLine($"Folder 'ECMWF/AIFS' already exists");
        }

        for (int i = 0; i < url_list.Count; i++)
        {
            await DownloadFileAsync(url_list[i], paths[i]);
        }

    }
}
    
