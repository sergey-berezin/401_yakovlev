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
using DB;

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для TabPage.xaml
    /// </summary>
    public partial class TabPage : UserControl
    {
        CancellationTokenSource cts;
        NeuralNetwork network;
        TextsAnswers db;
        int QueryID;

        public TabPage(NeuralNetwork network, TextsAnswers db)
        { 
            InitializeComponent();
            this.network = network;
            this.db = db;
            cts = new CancellationTokenSource();
            ButtonAnswer.IsEnabled = false;
            ButtonCancel.IsEnabled = false;
            this.QueryID = -1;
        }
        public void decreaseDBRecordCounter() 
        {
            UtilsForBD.decOpenCount(db, this.QueryID);
        }
        public void setTabPage(string text, string? question, string? answer, int QueryID)
        {
            TextBlockFile.Text = text;
            TextBoxQuestion.Text = question;
            TextBlockAnswer.Text = answer;
            this.QueryID = QueryID;
            ButtonAnswer.IsEnabled = true;
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
                QueryID = UtilsForBD.addQuestionAnswertoBD(db, fileContent, null, null);
            }
        }
        private async void ButtonClickGetAnswer(object sender, RoutedEventArgs e)
        {
            cts = new CancellationTokenSource();
            ButtonAnswer.IsEnabled = false;
            ButtonCancel.IsEnabled = true;
            UtilsForBD.decOpenCount(db, QueryID);
            if (TextBoxQuestion.Text == null || TextBlockFile.Text == null || network.isDownloaded == false ||
                TextBlockFile.Text == "") 
            {
                MessageBox.Show("Question or file is empty or network doesn't downloaded.");
                ButtonAnswer.IsEnabled = true;
                ButtonCancel.IsEnabled = false;
                return;
            }
            try
            {
                string? answer = UtilsForBD.getAnswerBD(db, TextBlockFile.Text, TextBoxQuestion.Text);
                UtilsForBD.decOpenCount(db, QueryID);
                if (answer != null)
                {
                    this.QueryID = UtilsForBD.addQuestionAnswertoBD(db, TextBlockFile.Text, TextBoxQuestion.Text, answer, QueryID);
                    //MessageBox.Show($"Result for {TextBoxQuestion.Text} from BD.");
                }
                else 
                {    
                    answer = await network.AnswerQuestionAsync(TextBlockFile.Text, TextBoxQuestion.Text, cts.Token);
                    this.QueryID = UtilsForBD.addQuestionAnswertoBD(db, TextBlockFile.Text, TextBoxQuestion.Text, answer, QueryID);
                }
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
