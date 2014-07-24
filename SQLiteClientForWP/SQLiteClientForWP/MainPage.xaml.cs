using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using SQLiteClientForWP.Classes;

namespace SQLiteClientForWP
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            DatabaseWrapper dbw = new DatabaseWrapper();
            double d =(double) dbw.ExecuteScalar("SELECT distanceGPSInKM(1,2,3,4) FROM customer");

            List<Customer> lstCustomes =  dbw.ExecuteQueryObject<Customer>("SELECT * FROM customer");

            dbw.Dispose();
        }
    }
}