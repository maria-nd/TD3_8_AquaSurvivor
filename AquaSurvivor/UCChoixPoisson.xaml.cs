using System;
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

namespace AquaSurvivor
{
    /// <summary>
    /// Logique d'interaction pour UCChoixPoisson.xaml
    /// </summary>
    public partial class UCChoixPoisson : UserControl
    {
        public UCChoixPoisson()
        {
            InitializeComponent();
        }

        private void butSuivant_Click(object sender, RoutedEventArgs e)
        {

        }

        private void rb1_Click(object sender, RoutedEventArgs e)
        {
            butSuivant.IsEnabled = true;
            MainWindow.Perso = "imgPoisson1";

        }

        private void rb2_Click(object sender, RoutedEventArgs e)
        {
            butSuivant.IsEnabled = true;
            MainWindow.Perso = "imgPoisson2";

        }

        private void rb3_Click(object sender, RoutedEventArgs e)
        {
            butSuivant.IsEnabled = true;
            MainWindow.Perso = "imgPoisson3";

        }

        private void rb4_Click(object sender, RoutedEventArgs e)
        {
            butSuivant.IsEnabled = true;
            MainWindow.Perso = "imgPoisson4";

        }

        private void rb5_Click(object sender, RoutedEventArgs e)
        {
            butSuivant.IsEnabled = true;
            MainWindow.Perso = "imgPoisson5";

        }

        private void rb6_Click(object sender, RoutedEventArgs e)
        {
            butSuivant.IsEnabled = true;
            MainWindow.Perso = "imgPoisson6";

        }

        private void rb7_Click(object sender, RoutedEventArgs e)
        {
            butSuivant.IsEnabled = true;
            MainWindow.Perso = "imgPoisson7";

        }

        private void rb8_Click(object sender, RoutedEventArgs e)
        {
            butSuivant.IsEnabled = true;
            MainWindow.Perso = "imgPoisson8";

        }

        private void rb9_Click(object sender, RoutedEventArgs e)
        {
            butSuivant.IsEnabled = true;
            MainWindow.Perso = "imgPoisson9";

        }
    }
}
