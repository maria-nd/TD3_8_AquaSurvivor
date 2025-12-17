using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public static int dureeBoostRestante { get; set; } = 10;
        DispatcherTimer timerJeuPrincipal;
        DispatcherTimer timerBoost;
        DispatcherTimer timerPoison;
        public static int[,] NiveauDifficulte { get; set; } = { { 7, 20, 10, 2, 5, 2, 1, 2 }, { 5, 15, 15, 10, 2, 4, 2, 3 }, { 4, 10, 30, 20, 2, 7, 5, 5 } };

        // 0 déplacement du poisson,      1 régeneration barre de faim grace à nouritture,      2 dégât des déchets sur le temps (en seconde ),
        // 3 dégat de la méduse sur la barre de faim,       4 Boost (en pas) de l'étoile,      5 déplacemet déchet (en pas), 6 diminution barre de faim
        private static int tempsRestant = 300;
        private static int score = 0;
        private static int [] objectif = [3, 5, 7, 10];
        private static string dernierePositionHorizontale=""; 
        private static bool booster;
        private static bool estEmpoisonne;
        private static int dureePoisonRestante = 10;
        private Random rnd = new Random();
        private readonly int NB_OBJETS = 15;

        
        private Image[] lesObjetsVisuels;
        private int[] typeObjets;
        private int[] indexSpeciaux;

        private int compteur = 0;

        private string[] nourriture = { "imgCalamar.png", "imgCrevette.png", "imgPoissonJaune.png", "imgSardine.png" };
        private string[] objetSpeciaux = { "imgPerle.png", "imgEtoileDeMer.png", "imgMeduse.png" };
        private string[] dechets = { "imgBouteille.png", "imgCigare.png", "imgCigarette.png", "imgPoubelle.png", "imgSacPlastique.png" };
        private string[] fond = { "imgAquarium.png", "imgRiviere.jpg", "imgLac.jpg", "imgMer.jpg", "imgOcean.jpg" };
        private int indexFond = 0;
        public  int pasPoisson = NiveauDifficulte[MainWindow.NiveauChoisi, 0];
        private int dureeBoost = 10;
        private int indexObjectif = 0;

        
        public UCJeu()
        {
            InitializeComponent();
            this.pasPoisson = NiveauDifficulte[MainWindow.NiveauChoisi, 0];
            ChangerImage("Gauche");

            timerJeuPrincipal = new DispatcherTimer();
            timerJeuPrincipal.Interval = TimeSpan.FromMilliseconds(50);
            timerJeuPrincipal.Tick += JeuCompteur;
           


            //jsp si je laisse comme ça
            timerBoost = new DispatcherTimer();
            timerBoost.Interval = TimeSpan.FromSeconds(1);
            timerBoost.Tick += BoostCompteur;

            timerPoison = new DispatcherTimer();
            timerPoison.Interval = TimeSpan.FromSeconds(1);
            timerPoison.Tick += PoisonCompteur;
           
        }
        private void PoisonCompteur(object? sender, EventArgs e)
        {

            if (!estEmpoisonne)
            {
                pasPoisson = NiveauDifficulte[MainWindow.NiveauChoisi, 0];
                timerPoison.Stop();
                return; // sortir de la méthode et pas éxecuter le reste (pas pertinent de diminuer la dureer si y'a plus de poison)
            }
            dureePoisonRestante--;
            
            if (dureePoisonRestante <= 0)
            {
                estEmpoisonne = false;
                timerPoison.Stop();
                barreFaim.Foreground = Brushes.Green; 
            }
        }

        private void BoostCompteur(object? sender, EventArgs e)
        {

            if (!booster)
            {
                pasPoisson = NiveauDifficulte[MainWindow.NiveauChoisi, 0];
                timerBoost.Stop();
                return;
            }
            dureeBoostRestante--;
            barreBoost.Value = dureeBoostRestante;
            if (dureeBoostRestante <= 0)
            {
                booster = false;
                timerBoost.Stop();

                pasPoisson = NiveauDifficulte[MainWindow.NiveauChoisi, 0] ; // revenir à la vitesse normale
                
                barreBoost.Opacity = 0; // cacher la barre de boost
            }
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyDown += DeplacementPoisson;
            //Application.Current.MainWindow.KeyUp += canvasJeu_KeyUp;

            InitObjets();
            timerJeuPrincipal.Start();
        }


        private void InitObjets()
        {
            lesObjetsVisuels = new Image[NB_OBJETS];
            typeObjets = new int[NB_OBJETS];
            indexSpeciaux = new int[NB_OBJETS];

            for (int i = 0; i < NB_OBJETS; i++)
            {
                lesObjetsVisuels[i] = new Image
                {
                    Width = 50,
                    Height = 50
                };

                canvasJeu.Children.Add(lesObjetsVisuels[i]);
                ResetObjet(i);
            }
        }
        private void ResetObjet(int index)
        {
            int chance = rnd.Next(0, 100);

            if (chance < 60)
            {
                // CAS DECHET
                typeObjets[index] = 2;
                int i = rnd.Next(0, dechets.Length);

                string chemin = "pack://application:,,,/img/Déchets/" + dechets[i];
                lesObjetsVisuels[index].Source = new BitmapImage(new Uri(chemin));
            }
            else if (chance < 85)
            {
                // CAS NOURRITURE
                typeObjets[index] = 0;
                int i = rnd.Next(0, nourriture.Length);

                string chemin = "pack://application:,,,/img/Nourriture/" + nourriture[i];
                lesObjetsVisuels[index].Source = new BitmapImage(new Uri(chemin));
            }
            else
            {
                // CAS SPECIAL
                typeObjets[index] = 1;
                int i = rnd.Next(0, objetSpeciaux.Length);
                indexSpeciaux[index] = i;

                string chemin = "pack://application:,,,/img/Nourriture/" + objetSpeciaux[i];
                lesObjetsVisuels[index].Source = new BitmapImage(new Uri(chemin));
            }

            double maxLeft = canvasJeu.ActualWidth - 50;
            Canvas.SetLeft(lesObjetsVisuels[index], rnd.Next(0, (int)maxLeft));
            Canvas.SetTop(lesObjetsVisuels[index], -rnd.Next(100, 800));
        }


        private void ActiverBoost()
        {
            booster = true;
            dureeBoostRestante = 10;
            pasPoisson += NiveauDifficulte[MainWindow.NiveauChoisi, 4]; // augmenter la vitesse du poisson
            barreBoost.Opacity = 1;// afficher la barre de boost
            barreBoost.Maximum = 10;
            barreBoost.Value = 10;
            timerBoost.Start();
        }

        private void DeplacementObjets()
        {
            if (lesObjetsVisuels == null) return;

            int pas = NiveauDifficulte[MainWindow.NiveauChoisi, 5];

            for (int i = 0; i < lesObjetsVisuels.Length; i++)
            {
                Image obj = lesObjetsVisuels[i];
                if (obj == null) continue;

                Canvas.SetTop(obj, Canvas.GetTop(obj) + pas);

                if (Canvas.GetTop(obj) > canvasJeu.ActualHeight)
                {
                    if (typeObjets[i] == 2)
                    {
                        Faim -= NiveauDifficulte[MainWindow.NiveauChoisi, 6];
                        if (Faim < 0) Faim = 0;
                        barreFaim.Value = Faim;
                    }

                    ResetObjet(i);
                }
            }
        }
        private void VerifierCollisions()
        {
            Rect rectPoisson = new Rect(Canvas.GetLeft(imgPoisson), Canvas.GetTop(imgPoisson), imgPoisson.Width, imgPoisson.Height);

            for (int i = 0; i < lesObjetsVisuels.Length; i++)
            {
                Image objetActuel = lesObjetsVisuels[i];

                Rect rectObjet = new Rect(Canvas.GetLeft(objetActuel), Canvas.GetTop(objetActuel), objetActuel.Width, objetActuel.Height);

                if (rectPoisson.IntersectsWith(rectObjet))
                {
                    
                    if (typeObjets[i] == 0)
                    {
                        Faim = Faim + NiveauDifficulte[MainWindow.NiveauChoisi, 1];
                        if (Faim > 100)
                        {
                            Faim = 100;
                        }
                        barreFaim.Value = Faim;
                    }

                    else if (typeObjets[i] == 2)
                    {
                        tempsRestant = tempsRestant - NiveauDifficulte[MainWindow.NiveauChoisi, 2];
                    }

                    else if (typeObjets[i] == 1)
                    {
                        if (indexSpeciaux[i] == 0)
                        {
                            Perletoucher();
                        }
                        else if (indexSpeciaux[i] == 1)
                        {
                            ActiverBoost();
                        }
                        else if (indexSpeciaux[i] == 2)
                        {
                            Empoisonner();
                        }
                    }

                    ResetObjet(i);

                }
            }
        }
        private void JeuCompteur(object sender, EventArgs e)
        {
            compteur++;

            if (compteur * 50 >= 1000)
            {
                Faim--;
                if (Faim < 0) Faim = 0;
                barreFaim.Value = Faim;

                tempsRestant--;
                labelTemps.Content = $"Temps : {tempsRestant}s";

                compteur = 0;
            }

            DeplacementObjets();
            VerifierCollisions();

            if (Faim <= 0 || tempsRestant <= 0)
            {
                timerJeuPrincipal.Stop();
                timerBoost.Stop();
                MainWindow.perdu = true;
                ((MainWindow)Application.Current.MainWindow).AfficherGameOver();
            }
        }

        private void Perletoucher()
        {
            int objectifActuel = objectif[indexObjectif];
            int objectifFinal = objectif[objectif.Length - 1];
            score += 1;
            // sécurise l’index
            if (indexObjectif >= objectif.Length)
                indexObjectif = objectif.Length - 1;
            if (score >= objectifActuel && indexObjectif < objectif.Length - 1) // objectif atteint ?
            {
                indexObjectif++;              // passe à l’objectif suivant
                objectifActuel = objectif[indexObjectif];
                labelScore.Content = $"Score : {score} / {objectifActuel}";
                tempsRestant += 30; // On ajoute 30sec à chaque fois qu'il atteint un niveau
                ChangerFond();
            }
            if (score >= objectifFinal)
            {
                timerJeuPrincipal.Stop();
          
                MainWindow FenetrePrincipale = (MainWindow)Application.Current.MainWindow;
                FenetrePrincipale.AfficherVictoire();
            }
            labelScore.Content = $"Score : {score} / {objectifActuel}";
        }
        private void ChangerFond()
        {
            indexFond++;
            if (indexFond == fond.Length -1) indexFond = fond.Length-1; //sécurise l’index (au cas où il dépasserait la taille du tableau)
            var fondAMettre = new ImageBrush { ImageSource= new BitmapImage(new Uri($"pack://application:,,,/img/Fonds/{fond[indexFond]}")), Stretch = Stretch.Fill };
            canvasJeu.Background = fondAMettre;
        }

        private void Empoisonner()
        {
            estEmpoisonne = true;
            dureePoisonRestante = 10; // 10 secondes de poison
            barreFaim.Foreground = Brushes.GreenYellow; // On modifie la couleur de la barre
            timerPoison.Start();
        }


        //public void ObjectifAtteint()       a mette dans menu 
        //{


        //public void ObjectifAtteint()       a mette dans menu 
        //{

        //}

        //public void Endroitactuel()       a mette dans menu 
        //{

        //}


        public static void ReinitialiserJeu()
        {
            Faim = 100;
            tempsRestant = 300;
            score = 0;
            MainWindow.perdu = false;

        }
             
        private void ChangerImage(string direction)
        {

            string nomFichierImage = $"pack://application:,,,/img/Poissons/{MainWindow.Perso}{direction}.png";
            imgPoisson.Source = new BitmapImage(new Uri(nomFichierImage));

        }
               
        private void DeplacementPoisson(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right && (Canvas.GetLeft(imgPoisson) + NiveauDifficulte[MainWindow.NiveauChoisi, 0]) + imgPoisson.Width < canvasJeu.ActualWidth)
            {
                ChangerImage("Droite");
                dernierePositionHorizontale = "Droite";
                Canvas.SetLeft(imgPoisson, Canvas.GetLeft(imgPoisson) + NiveauDifficulte[MainWindow.NiveauChoisi, 0]);
            }
            // à completer
#if DEBUG
#endif
            if (e.Key == Key.Left && Canvas.GetLeft(imgPoisson) - NiveauDifficulte[MainWindow.NiveauChoisi, 0] > 0)
            {
                ChangerImage("Gauche");
                dernierePositionHorizontale = "Gauche";
                Canvas.SetLeft(imgPoisson, Canvas.GetLeft(imgPoisson) - NiveauDifficulte[MainWindow.NiveauChoisi, 0]);
            }
            // à completer
#if DEBUG
#endif
            if (e.Key == Key.Down && (Canvas.GetTop(imgPoisson) + NiveauDifficulte[MainWindow.NiveauChoisi, 0]) + imgPoisson.Height < canvasJeu.ActualHeight)
                if (dernierePositionHorizontale == "Droite")
                {

                    ChangerImage("BasDroite");
                    Canvas.SetTop(imgPoisson, Canvas.GetTop(imgPoisson) + NiveauDifficulte[MainWindow.NiveauChoisi, 0]);
                }
                else
                {

                    ChangerImage("BasGauche");
                    Canvas.SetTop(imgPoisson, Canvas.GetTop(imgPoisson) + NiveauDifficulte[MainWindow.NiveauChoisi, 0]);

                }

#if DEBUG
#endif
            if (e.Key == Key.Up && Canvas.GetTop(imgPoisson) - NiveauDifficulte[MainWindow.NiveauChoisi, 0] > 0)
                if (dernierePositionHorizontale == "Droite")
                {
                    ChangerImage("HautDroite");
                    Canvas.SetTop(imgPoisson, Canvas.GetTop(imgPoisson) - NiveauDifficulte[MainWindow.NiveauChoisi, 0]);
                }
                else
                {
                    ChangerImage("HautGauche");
                    Canvas.SetTop(imgPoisson, Canvas.GetTop(imgPoisson) - NiveauDifficulte[MainWindow.NiveauChoisi, 0]);

                }

#if DEBUG
#endif
        }


       

        private void butPause_Click(object sender, RoutedEventArgs e)
        {

        }
        public void MettreEnPause()
        {
            if (timerJeuPrincipal != null)
            {
                timerJeuPrincipal.Stop();
            }

            if (timerBoost != null)
            {
                timerBoost.Stop();
            }

            Application.Current.MainWindow.KeyDown -= DeplacementPoisson;
        }

        public void ReprendreJeu()
        {
            if (timerJeuPrincipal != null)
            {
                timerJeuPrincipal.Start();
            }

            if (timerBoost != null && booster == true)
            {
                timerBoost.Start();
            }

            if (timerPoison != null && estEmpoisonne == true)
            {
                timerPoison.Start();
            }

            this.Focus();

            // On ré-attache le clavier par sécurité
            Application.Current.MainWindow.KeyDown -= DeplacementPoisson;
            Application.Current.MainWindow.KeyDown += DeplacementPoisson;
        }

       
  
    }
    }

