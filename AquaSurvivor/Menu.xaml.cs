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
using System.Windows.Shapes;

namespace AquaSurvivor
{
    /// <summary>
    /// Logique d'interaction pour Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public Menu()
        {
            InitializeComponent();
            

        }

        private void butReprendre_Click(object sender, RoutedEventArgs e)
        {
            // la je ferme juste la fenêtre et normalement la MainWindow reprendra toute seule
            this.Close();
        }

        private void butRejouer_Click(object sender, RoutedEventArgs e)
        {
            UCJeu.ReinitialiserJeu();
            MainWindow maFenetre = (MainWindow)Application.Current.MainWindow;
            this.Close();
            maFenetre.AfficherChoixPoisson(sender, e);
        }

        private void butQuitterPartie_Click(object sender, RoutedEventArgs e)
        {
            //UCJeu.ReinitialiserJeu();
            MainWindow maFenetre = (MainWindow)Application.Current.MainWindow;
            this.Close();
            maFenetre.AfficheDemarrage();
        }

     
    }
}
