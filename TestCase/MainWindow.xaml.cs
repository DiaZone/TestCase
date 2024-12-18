﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace TestCase
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void StartButton_Click(object sender, RoutedEventArgs e)
        {
            ExitWindow exitWindow = new ExitWindow();
            exitWindow.Show();
            this.Close();
        }

        public void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
