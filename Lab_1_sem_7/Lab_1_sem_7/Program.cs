// See https://aka.ms/new-console-template for more information

using System;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace MyApp // Note: actual namespace depends on the project name.
{
    class Program
    {
        static Semaphore printLock = new Semaphore(1, 1);
        static CancellationTokenSource cts = new CancellationTokenSource();
        static async Task Main(string[] args)
        {
            if (args.Length == 0)  
            {
                return;
            }
            IOInterface iOInterface = new IOInterface();
            NeuralNetwork model = new(iOInterface.GetTxtFileText(args[0]));
            Console.WriteLine("Starting model\n");
            model.OnnxModelInit();
            Console.WriteLine("Model started\n");
            string question;
            while ((question = Console.ReadLine()) != "")
            {
                var answer = ProcQuestionAsync(model, question);
            }
            Task.WhenAll();
        }
        static public async Task ProcQuestionAsync(NeuralNetwork model, string question) 
        {
            var questionProcRes = await Task.Run(() => model.AnswerQuestionAsync(question, cts.Token));
            Console.Write(question);
            Console.Write(" : "); 
            Console.WriteLine(questionProcRes);
        }
    }
}