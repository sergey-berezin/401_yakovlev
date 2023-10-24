﻿using MyApp;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int tabNum = 0;
        NeuralNetwork network;
        public MainWindow()
        {
            InitializeComponent();
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
            newTab.Content = new TabPage(network);
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
