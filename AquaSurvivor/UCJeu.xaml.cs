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
using System.Windows.Threading;

namespace AquaSurvivor
{
    /// <summary>
    /// Logique d'interaction pour UCJeu.xaml
    /// </summary>
    public partial class UCJeu : UserControl
    {
        public static int Faim { get; set; } = 100;
        public static int boost { get; set; } = 10;
        DispatcherTimer timerFaim;
        DispatcherTimer timerBoost;
        public static int[,] NiveauDifficulte { get; set; } = { { 7, 20, 10, 2, 5, 2 }, { 5, 15, 15, 10, 2, 4 }, { 4, 10, 30, 20, 2, 7 } };
        //déplacement du poisson, régeneration barre de faim grace à nouritture, dégât des déchets sur le temps (en seconde ), dégat de la méduse sur la barre de faim, Boost (en pas) de l'étoile, déplacemet déchet (en pas)
        private static int score = 0;
        private static int [] objectif = [30, 40, 55, 75];
        public UCJeu()
        {
            InitializeComponent();
            ChangerImage("Gauche");
            timerFaim = new DispatcherTimer();
            timerFaim.Interval = TimeSpan.FromSeconds(1);
            timerFaim.Tick += FaimDiminue;
            timerFaim.Start();
            Perletoucher();
            timerBoost = new DispatcherTimer();
            timerBoost.Interval = TimeSpan.FromSeconds(1);
            timerBoost.Tick += BoostVitesse;
        }
        private void Perletoucher()
        {
            for (int i = 0; i < objectif.Length; i++)
            {
                while (score < objectif[i])
                {
                    score += 1;
                    labelScore.Content = $"Score : {score} /{objectif}";
                }
            }
            
        }
        private void BoostVitesse(object? sender, EventArgs e)
        {
            barreBoost.Opacity = 1;
            boost--;
            barreFaim.Value = boost;

            if (boost == 0) 
            {
                timerBoost.Stop();
                barreBoost.Opacity = 0;

            }

            timerBoost.Stop();
            MainWindow.PasPoisson += NiveauDifficulte[MainWindow.NiveauChoisi, 4];
        }
        //public void ObjectifAtteint()       a mette dans menu 
        //{

        //}

        //public void Endroitactuel()       a mette dans menu 
        //{

        //}




        public void FaimDiminue(object? sender, EventArgs e)
        {
            if (Faim > 0)
            {
                Faim--;
                barreFaim.Value = Faim;
            }
            else
            {
                timerFaim.Stop();
                MainWindow.perdu = true;

            }

        }
        private void ChangerImage(string direction)
        {
            string nomFichierImage = $"pack://application:,,,/img/Poissons/{MainWindow.Perso}{direction}.png";
            imgPoisson.Source = new BitmapImage(new Uri(nomFichierImage));
        }
            // à completer

        private void canvasJeu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right && (Canvas.GetLeft(imgPoisson) + MainWindow.PasPoisson) + imgPoisson.Width < canvasJeu.ActualWidth)
            {
                ChangerImage("Droite");
                Canvas.SetLeft(imgPoisson, Canvas.GetLeft(imgPoisson) + MainWindow.PasPoisson);
            }
            // à completer
#if DEBUG
#endif
            if (e.Key == Key.Left && Canvas.GetLeft(imgPoisson) - MainWindow.PasPoisson > 0)
            {
                ChangerImage("Gauche");
                Canvas.SetLeft(imgPoisson, Canvas.GetLeft(imgPoisson) - MainWindow.PasPoisson);
            }
            // à completer
#if DEBUG
#endif
            if (e.Key == Key.Down && (Canvas.GetTop(imgPoisson) + MainWindow.PasPoisson) + imgPoisson.Height < canvasJeu.ActualHeight)
                Canvas.SetTop(imgPoisson, Canvas.GetTop(imgPoisson) + MainWindow.PasPoisson);
#if DEBUG
#endif
            if (e.Key == Key.Up && Canvas.GetTop(imgPoisson) - MainWindow.PasPoisson > 0)
                Canvas.SetTop(imgPoisson, Canvas.GetTop(imgPoisson) - MainWindow.PasPoisson);

#if DEBUG
#endif
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyDown += canvasJeu_KeyDown;
            //Application.Current.MainWindow.KeyUp += canvasJeu_KeyUp;
        }

        private void butPause_Click(object sender, RoutedEventArgs e)
        {
           
        }

    }
}
