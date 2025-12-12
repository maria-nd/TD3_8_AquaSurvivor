using System.Reflection;
using System.Text;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string Perso { get; set; } = "";
        public static int PasPoisson { get; set; } = 2;
        public static bool perdu { get; set; } = false;

        public MainWindow()
        {
            InitializeComponent();
            AfficheDemarrage();
           

        }
        private void AfficheDemarrage()
        {
            // crée et charge l'écran de démarrage
            UCDemarrage uc = new UCDemarrage();

            // associe l'écran au conteneur
            ZoneJeu.Content = uc;
            uc.but_Play.Click += AfficherReglesJeu;
        }
        
        private void AfficherReglesJeu(object sender, RoutedEventArgs e)
        {
            UCReglesJeu uc = new UCReglesJeu();
            ZoneJeu.Content = uc;
            uc.but_Continuer.Click += AfficherChoixPoisson;

        }


        private void AfficherChoixPoisson(object sender, RoutedEventArgs e)
        {
            UCChoixPoisson uc = new UCChoixPoisson();
            ZoneJeu.Content = uc;
            uc.butSuivant.Click += AfficherNiveauDifficulte;
        }



        private void AfficherNiveauDifficulte(object sender, RoutedEventArgs e)
        {
            UCNiveauDifficulte uc = new UCNiveauDifficulte();
            ZoneJeu.Content = uc;
            uc.butSuivant.Click += AfficherJeu;
        }
        private void AfficherJeu(object sender, RoutedEventArgs e)
        {
            UCJeu uc = new UCJeu();
            ZoneJeu.Content = uc;
            uc.butPause.Click += AfficherMenu;

        }
        private void AfficherMenu(object sender, RoutedEventArgs e)
        {
            Menu fen = new Menu();
            fen.ShowDialog();
            fen.Left = this.Left + 450; 
            fen.Top = this.Top + 150;
            fen.butReglesJeu.Click += AfficherReglesJeu;
        }

    }
}