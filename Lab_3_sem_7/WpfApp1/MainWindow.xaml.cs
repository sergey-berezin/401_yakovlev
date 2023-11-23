using MyApp;
using System;
using System.Collections.Generic;
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
using DB;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int tabNum = 0;
        NeuralNetwork network;
        TextsAnswers db;
        public MainWindow()
        {
            InitializeComponent();
            db = new TextsAnswers();
            //db.Add(new TextID { ID = 5, Text = "some text 2" });
            //db.SaveChanges();
            /*
            db.Add(new TextIDQuestion { ID = 5, Question = "sdfasdf 123", Answer = "some text 2" });
            db.SaveChanges();
            foreach (var temp in db.IDtoText) {
                MessageBox.Show(temp.Text + " " + temp.ID);
            }
            foreach (var temp in db.textIDQuestions)
            {
                MessageBox.Show(temp.Question + " " + temp.ID + " " + temp.Answer);
            }
            */
            tabNum = 0;
            network = new NeuralNetwork();
            var cts = new CancellationTokenSource();
            _ = network.OnnxModelInit(cts.Token);
        }

        private void CreateTab(object sender, RoutedEventArgs e)
        {
            TabItem newTab = new TabItem();
            var tabHead = new ClosableTabHeader($"Tab {tabNum}");
            newTab.Header = tabHead;
            newTab.Content = new TabPage(network, db);
            tabHead.button_close.Click +=
                (sender, e) =>
                {
                    ((TabPage)newTab.Content).CancelOp();
                    TextAnswerTab.Items.Remove(newTab);
                };
            tabNum++;
            TextAnswerTab.Items.Add(newTab);
        }
    }
}
