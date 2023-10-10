// See https://aka.ms/new-console-template for more information

using System;
using System.Numerics;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace MyApp
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
            Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e) {
                cts.Cancel();
            };
            IOConsoleImpl iOConsoleImpl = new IOConsoleImpl();
            string fullText = iOConsoleImpl.GetTxtFileText(args[0]);
            NeuralNetwork model = new();
            iOConsoleImpl.Print("Starting model\n");
            model.OnnxModelInit();
            iOConsoleImpl.Print("Model started\n");
            string? question;
            List<Task> taskList = new List<Task>();
            while ((question = iOConsoleImpl.GetInput()) != null && question != "")
            {
                var answer = ProcQuestionAsync(model, fullText, question);
                taskList.Add(answer);
            }
            await Task.WhenAll(taskList);
        }
        static public async Task ProcQuestionAsync(NeuralNetwork model, string fullText, string question)
        {
            var questionProcRes = await Task.Run(() => model.AnswerQuestionAsync(question, fullText, cts.Token));
            Console.Write(question);
            Console.Write(" : ");
            Console.WriteLine(questionProcRes);
        }
    }
}