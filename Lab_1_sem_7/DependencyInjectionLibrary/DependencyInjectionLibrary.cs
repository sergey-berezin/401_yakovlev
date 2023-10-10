using System;
using System.IO;
using System.Net.Http;
using static System.Net.WebRequestMethods;

namespace MyApp
{
    public interface IOInterface
    {
        public string GetTxtFileText(string filepath);
        public void Print(string data);
        public string? GetInput();
    }
    public class IOConsoleImpl : IOInterface
    {
        public string GetTxtFileText(string filepath)
        {
            return System.IO.File.ReadAllText(filepath);
        }
        public void Print(string data)
        {
            Console.WriteLine(data);
        }
        public string? GetInput()
        {
            return Console.ReadLine();
        }
    }
    public class ModelDownload
    {
        public static void DownloadOnnxModel(string filepath = "bert-large-uncased-whole-word-masking-finetuned-squa.onnx", string filelink = "https://storage.yandexcloud.net/dotnet4/bert-large-uncased-whole-word-masking-finetuned-squad.onnx")
        {
            if (!System.IO.File.Exists(filepath))
            {
                int try_counter = 0;
                while (true)
                {
                    try
                    {
                        using (var client = new HttpClient())
                        {
                            using (var s = client.GetStreamAsync(filelink))
                            {
                                using (var fs = new FileStream(filepath, FileMode.OpenOrCreate))
                                {
                                    s.Result.CopyTo(fs);
                                }
                            }
                        }
                        break;
                    }
                    catch (Exception ex)
                    {
                        try_counter++;
                        if (try_counter > 3)
                        {
                            try_counter = -1;
                            break;
                        }
                    }
                }
                if (try_counter == -1)
                {
                    throw new Exception("Error in ONNX model download.");
                }
            }
        }
    }
}