using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MyApp;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для TabPage.xaml
    /// </summary>
    public partial class TabPage : UserControl
    {
        CancellationTokenSource cts;
        NeuralNetwork network;

        public TabPage(NeuralNetwork network)
        { 
            InitializeComponent();
            this.network = network;
            cts = new CancellationTokenSource();
            ButtonAnswer.IsEnabled = false;
            ButtonCancel.IsEnabled = false;
        }
        private void ButtonClickOpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "txt files (*.txt)|*.txt";
            string filePath, fileContent;
            if (openFileDialog.ShowDialog() == true)
            {
                filePath = openFileDialog.FileName;
                fileContent = System.IO.File.ReadAllText(filePath);
                TextBlockFile.Text = fileContent;
                ButtonAnswer.IsEnabled = true;
            }
        }
        private async void ButtonClickGetAnswer(object sender, RoutedEventArgs e)
        {
            cts = new CancellationTokenSource();
            ButtonAnswer.IsEnabled = false;
            ButtonCancel.IsEnabled = true;
            if (TextBoxQuestion.Text == null || TextBlockFile.Text == null || network.isDownloaded == false) 
            {
                MessageBox.Show("Question or file is empty or network doesn't downloaded.");
                return;
            }
            try
            {
                string answer = await network.AnswerQuestionAsync(TextBlockFile.Text, TextBoxQuestion.Text, cts.Token);
                TextBlockAnswer.Text = answer;
            }
            catch (NotSupportedException ex) 
            {
                MessageBox.Show("The model was not loaded earlier.");
            }
            catch (ObjectDisposedException ex) 
            {
                MessageBox.Show("Operation canceled.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Somethink wrong.");
            }
            ButtonAnswer.IsEnabled = true;
            ButtonCancel.IsEnabled = false;
        }
        public void CancelOp()
        {
            cts.Cancel();
        }
        private void ButtonClickCancel(object sender, RoutedEventArgs e) 
        {
            cts.Cancel();
        }
    }
}
