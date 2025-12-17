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
        
        public static bool perdu { get; set; } = false;

        //public static UCJeu JeuEnCours { get; set; } = null; 
        //private UCJeu sauvegardeJeu;
       //public static bool RevenirAuJeuDepuisRegles { get; set; } = false;

        public static int  NiveauChoisi { get; set; }


        public MainWindow()
        {
            InitializeComponent();
            AfficheDemarrage();          
        }
        public void AfficheDemarrage()
        {
            UCDemarrage uc = new UCDemarrage();

            ZoneJeu.Content = uc;
            uc.but_Play.Click += AfficherReglesJeu;
        }
        
        public void AfficherReglesJeu(object sender, RoutedEventArgs e)
        {
            UCReglesJeu uc = new UCReglesJeu();
            ZoneJeu.Content = uc;
            uc.but_Continuer.Click += butContinuer_Click;

        }
        /*public void butContinuer_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindow.RevenirAuJeuDepuisRegles)
            {
                MainWindow.RevenirAuJeuDepuisRegles = false;

                if (JeuEnCours != null)
                {
                    ZoneJeu.Content = JeuEnCours;

                    JeuEnCours.ReprendreJeu();
                }
            }
            else
            {
                //JeuEnCours = null;
                AfficherChoixPoisson(sender, e);
            }

        }*/

        public void butContinuer_Click(object sender, RoutedEventArgs e)
        {
            AfficherChoixPoisson(sender, e);
        }

        public void AfficherChoixPoisson(object sender, RoutedEventArgs e)
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
            UCJeu jeuActuel = (UCJeu)ZoneJeu.Content;
            jeuActuel.MettreEnPause();

            Menu fen = new Menu();
            fen.Owner = this;
            fen.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            fen.ShowDialog();

            jeuActuel.ReprendreJeu();
        }

        public void AfficherGameOver()
        {
            // clavier pcq quand j'essaye de deplacer ça me déplace le poisson quand même
            // Application.Current.MainWindow.KeyDown -= (ZoneJeu.Content as UCJeu).canvasJeu_KeyDown;

            UCGameOver uc = new UCGameOver();
            ZoneJeu.Content = uc;
            uc.butRetenter.Click += AfficherChoixPoisson;
            uc.butQuitter.Click += QuitterJeu;

        }

        public void AfficherVictoire()
        {
            UCWin uc = new UCWin();

            ZoneJeu.Content = uc;

            uc.butRejouer.Click += AfficherChoixPoisson;
            uc.butQuitter.Click += QuitterJeu;
        }
        private void QuitterJeu(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }



    }
}