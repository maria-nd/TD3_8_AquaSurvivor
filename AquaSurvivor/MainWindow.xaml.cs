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

        private UCJeu JeuEnCours = null;
        public static bool RevenirAuJeuDepuisRegles { get; set; } = false;

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
        public void butContinuer_Click(object sender, RoutedEventArgs e)
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
                JeuEnCours = null; 
                AfficherChoixPoisson(sender, e);
            }
          
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
            JeuEnCours = new UCJeu();
            ZoneJeu.Content = JeuEnCours;
            JeuEnCours.butPause.Click += AfficherMenu;

        }

    

        private void AfficherMenu(object sender, RoutedEventArgs e)
        {
            if (ZoneJeu.Content is UCJeu ucJeuActuel)
            {
                ucJeuActuel.MettreEnPause();

                Menu fen = new Menu();
                fen.Owner = this;
                fen.WindowStartupLocation = WindowStartupLocation.CenterOwner;

                fen.butReglesJeu.Click += ReglesDuMenu_Click;

                fen.butReprendre.Click += (s, args) =>
                {
                    fen.Close(); 
                                 
                    if (JeuEnCours != null)
                    {
                        JeuEnCours.ReprendreJeu();
                    }
                };

                fen.ShowDialog();

                
            }
        }

        private void ReglesDuMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.RevenirAuJeuDepuisRegles = true;

            Window menuWindow = Window.GetWindow(sender as DependencyObject);
            menuWindow.Close();

            AfficherReglesJeu(sender, e);
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

        private void QuitterJeu(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }



    }
}