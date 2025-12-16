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
        public static int[,] NiveauDifficulte { get; set; } = { { 7, 20, 10, 2, 5, 2, 1 }, { 5, 15, 15, 10, 2, 4, 2 }, { 4, 10, 30, 20, 2, 7, 5 } };
        // 0 déplacement du poisson,      1 régeneration barre de faim grace à nouritture,      2 dégât des déchets sur le temps (en seconde ),
        // 3 dégat de la méduse sur la barre de faim,       4 Boost (en pas) de l'étoile,      5 déplacemet déchet (en pas), 6 diminution barre de faim
        private static int tempsRestant = 300;
        private static int score = 0;
        private static int [] objectif = [3, 5, 7, 10];
        private static string dernierePositionHorizontale="";
        private static string [] lesObjets= [ "Nourriture" , "Objets Séciaux", "Déchets"];
        private static bool booster;
        private static bool estEmpoisonne;
        private static int dureePoisonRestante = 10;
        /*private int pasPoisson;
        private static int[] objectif = [30, 40, 55, 75];
        private static string dernierePositionHorizontale = "";
        private static string[] lesObjets = ["Nourriture", "Objets Séciaux", "Déchets"];
        private static bool booster = false;*/


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
        private string[] fond = { "imgAquarium.png", "imgRiviere.jpg", "imgLac.jpg", "imgMer.jpg", "imgOcean.jpg" };
        private int indexFond = 0;
        public  int pasPoisson = NiveauDifficulte[MainWindow.NiveauChoisi, 0];
        private int dureeBoost = 10;
        private int indexObjectif = 0;

        //REVOIR : timer et boost avec collision

        /*public UCJeu()
        {
            InitializeComponent();
            this.pasPoisson = NiveauDifficulte[MainWindow.NiveauChoisi, 0];
            //ChangerImage("Gauche");

            ChargerImage(imgPoisson, $"/img/Poissons/{MainWindow.Perso}Gauche");

            timerJeuPrincipal = new DispatcherTimer();
            timerJeuPrincipal.Interval = TimeSpan.FromSeconds(1);
            timerJeuPrincipal.Tick += JeuCompteur;
            timerJeuPrincipal.Start();

            //jsp si je laisse comme ça
            timerBoost = new DispatcherTimer();
            timerBoost.Interval = TimeSpan.FromSeconds(1);
            //timerBoost.Tick += BoostVitesse;

            //InitObjets();

           // Perletoucher();
        }*/
        public UCJeu()
        {
            InitializeComponent();
            this.pasPoisson = NiveauDifficulte[MainWindow.NiveauChoisi, 0];
            ChangerImage("Gauche");

            timerJeuPrincipal = new DispatcherTimer();
            timerJeuPrincipal.Interval = TimeSpan.FromMilliseconds(50);
            timerJeuPrincipal.Tick += JeuCompteur;
            timerJeuPrincipal.Start();
            //jsp si je laisse comme ça
            timerBoost = new DispatcherTimer();
            timerBoost.Interval = TimeSpan.FromSeconds(1);
            timerBoost.Tick += BoostCompteur;

            timerPoison = new DispatcherTimer();
            timerPoison.Interval = TimeSpan.FromSeconds(1);
            timerPoison.Tick +=
            //InitObjets();

            // Perletoucher();
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





        private void InitObjets()
        {
            lesObjetsVisuels = new Image[NB_OBJETS];
            typeObjets = new int[NB_OBJETS];
            indexSpeciaux = new int[NB_OBJETS]; 

            for (int i = 0; i < NB_OBJETS; i++)
            {
                int chance = rnd.Next(0, 100);
                int type;
                string nomFichier;
                string dossier;

                if (chance < 60) // 60% Déchets
                {
                    type = 1; // Spécial
                    int index = rnd.Next(objetSpeciaux.Length);
                    nomFichier = objetSpeciaux[index];
                    dossier = "Nourriture";
                    indexSpeciaux[i] = index;
                }
                else if (chance < 75) 
                {
                    type = 0; // Nourriture
                    nomFichier = nourriture[rnd.Next(nourriture.Length)];
                    dossier = "Nourriture";
                    indexSpeciaux[i] = -1;
                }
                else 
                {
                    type = 2;
                    nomFichier = dechets[rnd.Next(dechets.Length)];
                    dossier = "Déchets";
                    indexSpeciaux[i] = -1;

                    type = 1; // Spécial
                    int index = rnd.Next(objetSpeciaux.Length);
                    nomFichier = objetSpeciaux[index];
                    dossier = "Nourriture";
                    indexSpeciaux[i] = index;
                  
                }

                typeObjets[i] = type; 

                lesObjetsVisuels[i] = new Image
                {
                    Source = new BitmapImage(new Uri($"pack://application:,,,/img/{dossier}/{nomFichier}")),
                    Width = 50,
                    Height = 50,
                };

                canvasJeu.Children.Add(lesObjetsVisuels[i]);
                //essayer de gerer le repositionnement

                double maxLeft = canvasJeu.ActualWidth - lesObjetsVisuels[i].Width;
                Canvas.SetLeft(lesObjetsVisuels[i], rnd.Next(0, (int)maxLeft) );
                Canvas.SetTop(lesObjetsVisuels[i], -rnd.Next(200, 800) * (i + 1));
            }
        }



        /* private void JeuCompteur(object? sender, EventArgs e)
         {

             Faim -= NiveauDifficulte[MainWindow.NiveauChoisi,1];
             if (tempsRestant > 0)
             {
                 int baisseFaim = NiveauDifficulte[MainWindow.NiveauChoisi, 0];
                 if (Faim > 0)
                 {
                     Faim -= baisseFaim; ;
                     if (Faim < 0) 
                         Faim = 0;
                     barreFaim.Value = Faim;
                 }

                 if (tempsRestant > 0)
                 {
                     tempsRestant--;
                     labelTemps.Content = $"Temps : {tempsRestant}s";
                 }

                 compteur = 0;
                 tempsRestant--;
                 labelTemps.Content = $"Temps : {tempsRestant}s";
             }
             if (Faim <= 0 || tempsRestant <= 0)
             {
                 timerJeuPrincipal.Stop();
                 if (timerBoost != null) timerBoost.Stop();
                 MainWindow.perdu = true;
                 MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                 mainWindow.AfficherGameOver();
             }
             *//*if (BoostVitesse)
             {
                 barreBoost.Opacity = 1;
                 dureeBoost--;
                 pasPoisson += NiveauDifficulte[MainWindow.NiveauChoisi, 4];
                 barreBoost.Value = dureeBoost;
             }*//*
             barreFaim.Value = Faim;


         }*/

        private void JeuCompteur(object? sender, EventArgs e)
        {
            compteur++;
            if (compteur * 50 >= 1000)
            {
                int baisseFaim = NiveauDifficulte[MainWindow.NiveauChoisi, 6];
                if (Faim > 0)
                {
                    Faim -= baisseFaim; ;
                    if (Faim < 0)
                        Faim = 0;
                    barreFaim.Value = Faim;
                }

                if (tempsRestant > 0)
                {
                    tempsRestant--;
                    labelTemps.Content = $"Temps : {tempsRestant}s";
                }

                compteur = 0;
            }

            DeplacementObjets();
            VerifierCollisions();

            if (Faim <= 0 || tempsRestant <= 0)
            {
                timerJeuPrincipal.Stop();
                if (timerBoost != null) timerBoost.Stop();
                MainWindow.perdu = true;

                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.AfficherGameOver();
            }
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
            int pasMouvement = NiveauDifficulte[MainWindow.NiveauChoisi, 5];

            for (int i = 0; i < lesObjetsVisuels.Length; i++)
            {
                Image objet = lesObjetsVisuels[i];

                   Canvas.SetTop(objet, Canvas.GetTop(objet) + pasMouvement);

                if (Canvas.GetTop(objet) > canvasJeu.ActualHeight)
                {
                    int type = typeObjets[i];

                    if (type != 2)
                    {
                        Faim -= 5;
                        if (Faim < 0) Faim = 0;
                        barreFaim.Value = Faim;
                    }

                    ResetObjet(i);
                }
            }
        }

        private void ResetObjet(int index)
        {
            Image objet = lesObjetsVisuels[index];

            int chance = rnd.Next(0, 100);
            int type;
            string nomFichier;
            string dossier;

            if (chance < 50)
            {
                type = 2; // Déchet
                nomFichier = dechets[rnd.Next(dechets.Length)];
                dossier = "Déchets";
                indexSpeciaux[index] = -1;
            }
            else if (chance < 65)
            {
                type = 0; // Nourriture
                nomFichier = nourriture[rnd.Next(nourriture.Length)];
                dossier = "Nourriture";
                indexSpeciaux[index] = -1;
            }
            else
            {
                type = 1; // Spécial
                int indexSpec = rnd.Next(objetSpeciaux.Length);
                nomFichier = objetSpeciaux[indexSpec];
                dossier = "Nourriture";
                indexSpeciaux[index] = indexSpec;
            }

            typeObjets[index] = type; 

            objet.Source = new BitmapImage(new Uri($"pack://application:,,,/img/{dossier}/{nomFichier}"));

            double maxLeft = canvasJeu.ActualWidth - objet.Width;
            Canvas.SetLeft(objet, rnd.Next(0, (int)maxLeft));
            Canvas.SetTop(objet, -rnd.Next(100, 500));
        }

        

        private void VerifierCollisions()
        {
            Rect rectPoisson = new Rect(
                Canvas.GetLeft(imgPoisson),
                Canvas.GetTop(imgPoisson),
                imgPoisson.Width,
                imgPoisson.Height
            );

            for (int i = 0; i < lesObjetsVisuels.Length; i++)
            {
                Image objet = lesObjetsVisuels[i];

                Rect rectObjet = new Rect(
                    Canvas.GetLeft(objet),
                    Canvas.GetTop(objet),
                   objet.Width,
                    objet.Height
              );

                if (rectPoisson.IntersectsWith(rectObjet))
                {
                    int type = typeObjets[i]; 
                    int indexSpec = indexSpeciaux[i]; 

                    if (type == 0)
                    {
                        Faim += NiveauDifficulte[MainWindow.NiveauChoisi, 1];
                        if (Faim > 100) Faim = 100;
                        barreFaim.Value = Faim;
                    }
                    else if (type == 2) 
                    {
                        tempsRestant -= NiveauDifficulte[MainWindow.NiveauChoisi, 2];
                    }
                    else if (type == 1) // Spécial
                    {
                        if (indexSpec == 0) 
                        {
                            Faim += NiveauDifficulte[MainWindow.NiveauChoisi, 1]; // Nourriture
                            if (Faim > 100) Faim = 100;
                            barreFaim.Value = Faim;

                            Perletoucher();
                        }
                        if (indexSpec == 1) 
                        {
                            
                            ActiverBoost();
                            //if (boost <= 0)
                            //{
                            //    NiveauDifficulte[MainWindow.NiveauChoisi, 0] += NiveauDifficulte[MainWindow.NiveauChoisi, 4];
                            //}
                            //boost = 10;
                            //if (timerBoost != null) timerBoost.Start();

                          /*  NiveauDifficulte[MainWindow.NiveauChoisi, 0] += NiveauDifficulte[MainWindow.NiveauChoisi, 4];
                            boost = 10;
                            if (timerBoost != null) timerBoost.Start();*/
                        }
                        else if (indexSpec == 2) 
                        {
                            Faim -= NiveauDifficulte[MainWindow.NiveauChoisi, 3];
                            if (Faim < 0) Faim = 0;
                            barreFaim.Value = Faim;
                        }
                    }

                    ResetObjet(i);
                }
            }
        }

        private void Perletoucher()
        {
            int objectifActuel = objectif[indexObjectif];
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




        /* public void FaimDiminue(object? sender, EventArgs e)
          {
              if (tempsRestant > 0)
              {
                  tempsRestant--;
                  labelTemps.Content = $"Temps : {tempsRestant}s";
              }
              if (Faim == 0 || tempsRestant == 0)
              {
                  timerJeuPrincipal.Stop();
                  //timerBoost.Stop();
                  MainWindow.perdu = true;

                  // Appel de la méthode pour afficher l'écran Game Over
                  MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                  mainWindow.AfficherGameOver();
              }


                  if (Faim > 0)
              {
                  Faim--;
                  barreFaim.Value = Faim;
              }
              else
              {
                  timerJeuPrincipal.Stop();
                  MainWindow.perdu = true;

              }

          }*/

        public static void ReinitialiserJeu()
        {
            Faim = 100;
            tempsRestant = 60;
            score = 0;
            MainWindow.perdu = false;
        }


      /*  private void ChargerImage(Image cible, string chemin)
        {

            string nomFichierImage = $"pack://application:,,,{chemin}.png";
            cible.Source = new BitmapImage(new Uri(nomFichierImage));

        }*/
        private void ChangerImage(string direction)
        {

            string nomFichierImage = $"pack://application:,,,/img/Poissons/{MainWindow.Perso}{direction}.png";
            imgPoisson.Source = new BitmapImage(new Uri(nomFichierImage));

        }


        // à completer




        /*private void DeplacementPoisson(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right && (Canvas.GetLeft(imgPoisson) + pasPoisson) + imgPoisson.Width < canvasJeu.ActualWidth)
            {
                ChargerImage(imgPoisson, $"/img/Poissons/{MainWindow.Perso}Droite");
                dernierePositionHorizontale = "Droite";
                Canvas.SetLeft(imgPoisson, Canvas.GetLeft(imgPoisson) + pasPoisson);
            }
            // à completer
#if DEBUG
#endif
            if (e.Key == Key.Left && Canvas.GetLeft(imgPoisson) - pasPoisson > 0)
            {
                ChargerImage(imgPoisson, $"/img/Poissons/{MainWindow.Perso}Gauche");
                dernierePositionHorizontale = "Gauche";
                Canvas.SetLeft(imgPoisson, Canvas.GetLeft(imgPoisson) - pasPoisson);
            }
            // à completer
#if DEBUG
#endif
            if (e.Key == Key.Down && (Canvas.GetTop(imgPoisson) + pasPoisson) + imgPoisson.Height < canvasJeu.ActualHeight)
                if (dernierePositionHorizontale == "Droite")
                {

                    ChargerImage(imgPoisson, $"/img/Poissons/{MainWindow.Perso}BasDroite");
                    Canvas.SetTop(imgPoisson, Canvas.GetTop(imgPoisson) + pasPoisson);
                }
                else
                {

                    ChargerImage(imgPoisson, $"/img/Poissons/{MainWindow.Perso}BasGauche");
                    Canvas.SetTop(imgPoisson, Canvas.GetTop(imgPoisson) + pasPoisson);

                }

#if DEBUG
#endif
            if (e.Key == Key.Up && Canvas.GetTop(imgPoisson) - pasPoisson > 0)
                if (dernierePositionHorizontale == "Droite")
                {
                    ChargerImage(imgPoisson, $"/img/Poissons/{MainWindow.Perso}HautDroite");
                    Canvas.SetTop(imgPoisson, Canvas.GetTop(imgPoisson) - pasPoisson);
                }
                else
                {
                    ChargerImage(imgPoisson, $"/img/Poissons/{MainWindow.Perso}HautGauche");
                    Canvas.SetTop(imgPoisson, Canvas.GetTop(imgPoisson) - pasPoisson);

                }

#if DEBUG
#endif
        }*/
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


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyDown += DeplacementPoisson;
            //Application.Current.MainWindow.KeyUp += canvasJeu_KeyUp;

            InitObjets();

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


       /* private void collision(object sender, RoutedEventArgs e)
        {
            Rect rectPoisson = new Rect(Canvas.GetLeft(imgPoisson), Canvas.GetTop(imgPoisson), imgPoisson.Width, imgPoisson.Height);
            for (int i = 0; i < lesObjets.Length; i++)
            {
                Image objet = new Image();
                Rect rectobjet = new Rect();
                if (lesObjets[i] == "Nourriture")
                {
                    for (int j = 0; j < nourriture.Length; j++)
                    {
                        ChargerImage(objet, $"/img/Nourriture/{nourriture[j]}");
                        rectobjet = new Rect(Canvas.GetLeft(objet), Canvas.GetTop(objet), objet.Width, objet.Height);
                        if (rectPoisson.IntersectsWith(rectobjet))
                            Faim += NiveauDifficulte[MainWindow.NiveauChoisi, 1]; //On ajoute des "points" à la faim selon le niveau choisis
                    }
                }
                else if (lesObjets[i] == "Objets Spéciaux")
                {
                    for (int k = 0; k < objetSpeciaux.Length; k++)
                    {
                        ChargerImage(objet, $"/img/Nourriture/{nourriture[k]}");
                        rectobjet = new Rect(Canvas.GetLeft(objet), Canvas.GetTop(objet), objet.Width, objet.Height);
                        if (rectPoisson.IntersectsWith(rectobjet))
                        {
                            if (i == 0) // étoile de mer
                                Perletoucher(); // On ajoute au score +1
                            else if (i == 1) // boost de vitesse
                            {
                                booster = true; // On active le boost de vitesse le poisson va plus vite selon le niveau choisis et on a la barreBoost qui aparraît à l'écran
                            }
                            else // Méduse
                            {
                                Empoisonner();
                            }
                        }
                    }
                }
                else
                {
                    for (int l = 0; l < objetSpeciaux.Length; l++)
                    {
                        ChargerImage(objet, $"/img/Nourriture/{nourriture[l]}");
                        rectobjet = new Rect(Canvas.GetLeft(objet), Canvas.GetTop(objet), objet.Width, objet.Height);
                        if (rectPoisson.IntersectsWith(rectobjet)) ;
                            //timerJeuPrincipal -= NiveauDifficulte[MainWindow.NiveauChoisi, 3]; //On retire du temps selon le niveau choisis

                    }

                }

            }*/

            //}

            //private string typeObjet(Image objet)
            //{
            //    string tag;
            //    tag = objet.Tag.ToString();
            //    return tag;
            //}



        }
    }

