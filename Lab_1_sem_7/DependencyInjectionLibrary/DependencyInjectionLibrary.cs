using System;
using System.IO;
using System.Net.Http;

namespace MyApp
{
    public class IOInterface 
    {
        public string GetTxtFileText(string filepath) 
        {
            string fullText = "", line;
            StreamReader sr = new StreamReader(filepath);
            //Read the first line of text
            line = sr.ReadLine();
            //Continue to read until you reach end of file
            while (line != null)
            {
                fullText += line;
                line = sr.ReadLine();
            }
            sr.Close();
            return fullText;
        }
        public void Print (string data)
        {
            Console.WriteLine(data);
        }
        public string? GetInput() 
        {
            return Console.ReadLine();
        }
    }
    public class DownloadInterface
    {
        public void DownloadOnnxModel(string filepath = "bert_model.onnx", string filelink = "https://storage.yandexcloud.net/dotnet4/bert-large-uncased-whole-word-masking-finetuned-squad.onnx") 
        {
            if (!File.Exists(filepath))
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