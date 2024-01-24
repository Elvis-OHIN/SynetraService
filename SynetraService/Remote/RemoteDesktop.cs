using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using SynetraService.Models;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;

namespace SynetraService.Remote
{
    public class RemoteDesktop
    {
        private TcpClient Client = new TcpClient();
        private NetworkStream NetworkStream;
        private int port;

        private static Bitmap CaptureDesktopImage()
        {
            using var screenshot = new Bitmap(1920, 1080);
            using (var g = Graphics.FromImage(screenshot))
            {
                g.CopyFromScreen(0, 0, 0, 0,
                screenshot.Size, CopyPixelOperation.SourceCopy);
            }
            return screenshot;
        }
        public static async Task SendImageAsync()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Remplacez cette URL par l'URL de votre service .NET
                    string serviceUrl = $"https://localhost:7082/api/ShareScreen/{SystemInfo.GetMotherboardInfo()}";
                    byte[] imageBytes;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        CaptureDesktopImage().Save(ms, ImageFormat.Png);
                        imageBytes = ms.ToArray();
                    }
                    var content = new ByteArrayContent(imageBytes);
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");

                    var response = await httpClient.PostAsync(serviceUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("L'image a été envoyée avec succès.");
                    }
                    else
                    {
                        Console.WriteLine("Échec de l'envoi de l'image : " + response.ReasonPhrase);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de l'envoi de l'image : " + ex.Message);
            }
        }
    }
}
