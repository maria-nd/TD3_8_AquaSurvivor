using System;
using System.Collections.Generic;
using System.ComponentModel;
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

       
        public void AfficherEndroit(int indexFond)
        {
            string texte;
            switch (indexFond)
            {
                case 1:
                    texte = "la Rivière";
                    break;
                case 2:
                    texte = "le Lac";
                    break;
                case 3:
                    texte = "la Mer";
                    break;
                case 4:
                    texte = " l'Océan";
                    break;
                default:
                    texte = "Inconnu";
                    break;
            }
            labelEndroit.Content = $"Vous êtes dans {texte}";
        }
        public void AfficherObjectifAtteint(int numero)
        {
            Label lbl= null;
            switch (numero)
            {
                case 1: lbl = labelObjectif1; break;
                case 2: lbl = labelObjectif2; break;
                case 3: lbl = labelObjectif3; break;
                case 4: lbl = labelObjectif4; break;
            }
            if (lbl != null)
            {
                lbl.Content = $"Objectif {numero} : atteint";
                lbl.Foreground = Brushes.Green;
            }
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
