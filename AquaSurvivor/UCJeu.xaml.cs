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
        public static int boost { get; set; } = 10;
        DispatcherTimer timerJeuPrincipal;
        DispatcherTimer timerBoost;
        public static int[,] NiveauDifficulte { get; set; } = { { 7, 20, 10, 2, 5, 2, 1, 2 }, { 5, 15, 15, 10, 2, 4, 2, 3 }, { 4, 10, 30, 20, 2, 7, 5, 5 } };

        //déplacement du poisson,     régeneration barre de faim grace à nouritture,      dégât des déchets sur le temps (en seconde ),
        //dégat de la méduse sur la barre de faim,       Boost (en pas) de l'étoile,      déplacemet déchet (en pas)
        private static int tempsRestant = 100;
        private static int score = 0;
        private static int [] objectif = [30, 40, 55, 75];
        private static string dernierePositionHorizontale="";

      


        private Random rnd = new Random();
        private readonly int NB_OBJETS = 15;
        //private string[,] lesObjets = { { "imgCalamar.png", "imgCrevette.png", "imgPoissonJaune.png", "imgSardine.png", "imgSardine.png", }, { "imgPerle.png", "imgEtoileDeMer.png", "imgMeduse.png", "imgMeduse.png", "imgMeduse.png" }, { "imgBouteille.png", "imgCigare.png", "imgCigarette.png", "imgPoubelle.png", "imgSacPlastique.png" } };

        
        private Image[] lesObjetsVisuels;
        private int[] typeObjets;
        private int[] indexSpeciaux;

        private int compteur = 0;

        private string[] nourriture = { "imgCalamar.png", "imgCrevette.png", "imgPoissonJaune.png", "imgSardine.png" };
        private string[] objetSpeciaux = { "imgPerle.png", "imgEtoileDeMer.png", "imgMeduse.png" };
        private string[] dechets = { "imgBouteille.png", "imgCigare.png", "imgCigarette.png", "imgPoubelle.png", "imgSacPlastique.png" };
        private string[] fond = { "imgAquarium.png", "imgRiviere.png", "imgLac.png", "imgMer.png", "imgOcean.png" };
        public  int pasPoisson = NiveauDifficulte[MainWindow.NiveauChoisi, 0];

        
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
            timerBoost.Tick += BoostVitesse;

           
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
            int chance = rnd.Next(100);
            int type;
            string fichier;
            string dossier;

            if (chance < 60)
            {
                type = 2; // Déchet
                fichier = dechets[rnd.Next(dechets.Length)];
                dossier = "Déchets";
                indexSpeciaux[index] = -1;
            }
            else if (chance < 85)
            {
                type = 0; // Nourriture
                fichier = nourriture[rnd.Next(nourriture.Length)];
                dossier = "Nourriture";
                indexSpeciaux[index] = -1;
            }
            else
            {
                type = 1; // Spécial
                int spec = rnd.Next(objetSpeciaux.Length);
                fichier = objetSpeciaux[spec];
                dossier = "Nourriture";
                indexSpeciaux[index] = spec;
            }

            typeObjets[index] = type;

            lesObjetsVisuels[index].Source =
                new BitmapImage(new Uri($"pack://application:,,,/img/{dossier}/{fichier}"));

            double maxLeft = canvasJeu.ActualWidth - 50;
            Canvas.SetLeft(lesObjetsVisuels[index], rnd.Next(0, (int)maxLeft));
            Canvas.SetTop(lesObjetsVisuels[index], -rnd.Next(100, 800));
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
            double pLeft = Canvas.GetLeft(imgPoisson);
            double pTop = Canvas.GetTop(imgPoisson);

            if (double.IsNaN(pLeft) || double.IsNaN(pTop)) return;

            Rect rectPoisson = new Rect(pLeft, pTop, imgPoisson.Width, imgPoisson.Height);

            for (int i = 0; i < lesObjetsVisuels.Length; i++)
            {
                Image obj = lesObjetsVisuels[i];
                if (obj == null) continue;

                double oLeft = Canvas.GetLeft(obj);
                double oTop = Canvas.GetTop(obj);

                if (double.IsNaN(oLeft) || double.IsNaN(oTop)) continue;

                Rect rectObj = new Rect(oLeft, oTop, obj.Width, obj.Height);

                if (!rectPoisson.IntersectsWith(rectObj)) continue;

                if (typeObjets[i] == 0)
                {
                    Faim += NiveauDifficulte[MainWindow.NiveauChoisi, 1];
                    if (Faim > 100) Faim = 100;
                    barreFaim.Value = Faim;
                }
                else if (typeObjets[i] == 2)
                {
                    tempsRestant -= NiveauDifficulte[MainWindow.NiveauChoisi, 2];
                }
                else
                {
                    if (indexSpeciaux[i] == 0)
                    {
                        score++;
                        labelScore.Content = $"Score : {score}/{objectif[MainWindow.NiveauChoisi]}";
                    }
                    else if (indexSpeciaux[i] == 1)
                    {
                        boost = 10;
                        barreBoost.Value = boost;
                        barreBoost.Opacity = 1;
                        timerBoost.Start();
                    }
                    else if (indexSpeciaux[i] == 2)
                    {
                        Faim -= NiveauDifficulte[MainWindow.NiveauChoisi, 3];
                        if (Faim < 0) Faim = 0;
                        barreFaim.Value = Faim;
                    }
                }

                ResetObjet(i);
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


     

        /* private void Perletoucher()
         {
             for (int i = 0; i < objectif.Length; i++)
             {
                 while (score < objectif[i])
                 {
                     score += 1;
                     labelScore.Content = $"Score : {score} /{objectif[i]}";
                 }
             }

         }*/

        private void Empoisonner()
        {
            Faim -= NiveauDifficulte[MainWindow.NiveauChoisi, 3];
            barreFaim.Value = Faim;
        }
        private void BoostVitesse(bool booster) // J'ai retirer object sender, EventArgs e à  remettre si jamais ca pose problème
        {
            barreBoost.Opacity = 1;
            //NiveauDifficulte[MainWindow.NiveauChoisi, 0] += NiveauDifficulte[MainWindow.NiveauChoisi, 4];
            if (boost > 0) // Une while serait + approprié ? 
            while (!booster)
            {
                
                NiveauDifficulte[MainWindow.NiveauChoisi, 0] += NiveauDifficulte[MainWindow.NiveauChoisi, 4];
                if (boost > 0) // Une while serait + approprié ? 
                {
                    boost--;
                    barreFaim.Value = boost;
                }

                else
                {
                    timerBoost.Stop();
                    barreBoost.Opacity = 0;
                    booster = true;

                }
            }

            else
            {
                timerBoost.Stop();
                barreBoost.Opacity = 0;
                NiveauDifficulte[MainWindow.NiveauChoisi, 0] = pasPoisson;
            }
            

        }
        private void BoostVitesse(object? sender, EventArgs e) // J'ai retirer object sender, EventArgs e à  remettre si jamais ca pose problème
        {
            barreBoost.Opacity = 1;
            //NiveauDifficulte[MainWindow.NiveauChoisi, 0] += NiveauDifficulte[MainWindow.NiveauChoisi, 4];
            if (boost > 0) // Une while serait + approprié ? 
            {
                boost--;
                barreFaim.Value = boost;
            }

            else
            {
                timerBoost.Stop();
                barreBoost.Opacity = 0;
                NiveauDifficulte[MainWindow.NiveauChoisi, 0] = pasPoisson;
            }

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
            tempsRestant = 60;
            score = 0;
            MainWindow.perdu = false;
        }


     
        private void ChangerImage(string direction)
        {

            string nomFichierImage = $"pack://application:,,,/img/Poissons/{MainWindow.Perso}{direction}.png";
            imgPoisson.Source = new BitmapImage(new Uri(nomFichierImage));

        }

        private void ChangerDeFond(string nomFond)
        {

        }
        // à completer




       
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
            if (barreFaim != null)
            {
                barreFaim.Value = Faim;
            }
            if (timerJeuPrincipal != null)
            {
                timerJeuPrincipal.Start();
            }
            if (timerBoost != null)
            {
                timerBoost.Start();
            }
            Application.Current.MainWindow.KeyDown += DeplacementPoisson;
        }
        private void ChangementFond()
        {

        }



        }
    }

