using System.Threading.Tasks;
using WxData_Client.ECMWF;

#pragma warning disable CA1050 // Declare types in namespaces
public class GetData
#pragma warning restore CA1050 // Declare types in namespaces
{
    public static async Task Main(string[] args)
    {

        await FileDownloader.DownloadECMWFAIFS(72);
        

    }
}