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
using System.Xml.Serialization;

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
            tabNum = 0;
            network = new NeuralNetwork();
            var cts = new CancellationTokenSource();
            _ = network.OnnxModelInit(cts.Token);
            List<(string, string?, string?, int)> allTabsInfo = UtilsForBD.getAllRecordToRestore(db);
            foreach ((string, string?, string?, int) cur_elem in allTabsInfo) 
            {
                createTab(cur_elem.Item1, cur_elem.Item2, cur_elem.Item3, cur_elem.Item4);
            }
        }

        private void CreateTab(object sender, RoutedEventArgs e)
        {
            createTab();
        }
        private void createTab(string? text = null, string? question = null, string? answer = null, int QueryNum = -1)
        {
            TabItem newTab = new TabItem();
            var tabHead = new ClosableTabHeader($"Tab {tabNum}");
            newTab.Header = tabHead;
            var curTabPage = new TabPage(network, db);
            curTabPage.setTabPage(text, question, answer, QueryNum);
            newTab.Content = curTabPage;
            tabHead.button_close.Click +=
                (sender, e) =>
                {
                    ((TabPage)newTab.Content).CancelOp();
                    TextAnswerTab.Items.Remove(newTab);
                    curTabPage.decreaseDBRecordCounter();
                };
            tabNum++;
            TextAnswerTab.Items.Add(newTab);
        }
        private void ButtonClickDropTables(object sender, RoutedEventArgs e)
        {
            UtilsForBD.DropDatabase(db);
        }
    }
}
