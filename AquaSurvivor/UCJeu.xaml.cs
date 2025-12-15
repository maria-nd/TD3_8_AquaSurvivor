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
        public static int[,] NiveauDifficulte { get; set; } = { { 7, 20, 10, 2, 5, 2 }, { 5, 15, 15, 10, 2, 4 }, { 4, 10, 30, 20, 2, 7 } };
        //déplacement du poisson,     régeneration barre de faim grace à nouritture,      dégât des déchets sur le temps (en seconde ),
        //dégat de la méduse sur la barre de faim,       Boost (en pas) de l'étoile,      déplacemet déchet (en pas)
        private static int tempsRestant = 100;
        private static int score = 0;
        private static int [] objectif = [30, 40, 55, 75];
        private static string dernierePositionHorizontale="";
        private static string [] lesObjets= [ "Nourriture" , "Objets Séciaux", "Déchets"];

        private int pasPoisson;
        private static int[] objectif = [30, 40, 55, 75];
        private static string dernierePositionHorizontale = "";
        private static string[] lesObjets = ["Nourriture", "Objets Séciaux", "Déchets"];
        private static bool booster = false;


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
        private int dureeBoost = 10;

        //REVOIR : timer et boost avec collision

        public UCJeu()
        {
            InitializeComponent();
            this.pasPoisson = NiveauDifficulte[MainWindow.NiveauChoisi, 0];
            ChangerImage("Gauche");

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
                    int index = rnd.Next(obj_Speciaux.Length);
                    nomFichier = obj_Speciaux[index];
                    dossier = "Nourriture";
                    indexSpeciaux[i] = index;
                }
                else if (chance < 75) 
                {
                    type = 0; // Nourriture
                    nomFichier = obj_Nourriture_Basics[rnd.Next(obj_Nourriture_Basics.Length)];
                    dossier = "Nourriture";
                    indexSpeciaux[i] = -1;
                }
                else 
                {
                    type = 2;
                    nomFichier = obj_Dechets[rnd.Next(obj_Dechets.Length)];
                    dossier = "Déchets";
                    indexSpeciaux[i] = -1;

                    type = 1; // Spécial
                    int index = rnd.Next(obj_Speciaux.Length);
                    nomFichier = obj_Speciaux[index];
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

       
        
        private void JeuCompteur(object? sender, EventArgs e)
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
            if (booster)
            {
                barreBoost.Opacity = 1;
                dureeBoost--;
                pasPoisson += NiveauDifficulte[MainWindow.NiveauChoisi, 4];
                barreBoost.Value = dureeBoost;
            }
            barreFaim.Value = Faim;


        }
        private void BoostActif(object sender, EventArgs e)
      

        private void DeplacementObjets()
        {
            barreBoost.Opacity = 1;
            pasPoisson += NiveauDifficulte[MainWindow.NiveauChoisi, 4];


        }

       private void DeplacementObjets()
        {
        //    int pasMouvement = NiveauDifficulte[MainWindow.NiveauChoisi, 5];

            for (int i = 0; i < lesObjetsVisuels.Length; i++)
            {
                Image objet = lesObjetsVisuels[i];

        //        Canvas.SetTop(objet, Canvas.GetTop(objet) + pasMouvement);

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

            if (chance < 60)
            {
                type = 2; // Déchet
                nomFichier = obj_Dechets[rnd.Next(obj_Dechets.Length)];
                dossier = "Déchets";
                indexSpeciaux[index] = -1;
            }
            else if (chance < 85)
            {
                type = 0; // Nourriture
                nomFichier = obj_Nourriture_Basics[rnd.Next(obj_Nourriture_Basics.Length)];
                dossier = "Nourriture";
                indexSpeciaux[index] = -1;
            }
            else
            {
                type = 1; // Spécial
                int indexSpec = rnd.Next(obj_Speciaux.Length);
                nomFichier = obj_Speciaux[indexSpec];
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

        //        Rect rectObjet = new Rect(
        //            Canvas.GetLeft(objet),
        //            Canvas.GetTop(objet),
        //            objet.Width,
        //            objet.Height
        //        );

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

                            score++;
                            labelScore.Content = $"Score : {score} /{objectif[MainWindow.NiveauChoisi]}";
                        }
                        if (indexSpec == 1) 
                        {
                            if (boost <= 0)
                            {
                                NiveauDifficulte[MainWindow.NiveauChoisi, 0] += NiveauDifficulte[MainWindow.NiveauChoisi, 4];
                            }
                            boost = 10;
                            if (timerBoost != null) timerBoost.Start();

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
            for (int i = 0; i < objectif.Length; i++)
            {
                while (score < objectif[i])
                {
                    score += 1;
                    labelScore.Content = $"Score : {score} /{objectif[i]}";
                }
            }

        }

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


        private void ChargerImage(Image cible, string chemin)
        {

            string nomFichierImage = $"pack://application:,,,{chemin}.png";
            cible.Source = new BitmapImage(new Uri(nomFichierImage));

        }
        private void ChangerDeFond(string nomFond)
        {

        }
        // à completer




        private void DeplacementPoisson(object sender, KeyEventArgs e)
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


        private void collision(object sender, RoutedEventArgs e)
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

            }

            //}

            //private string typeObjet(Image objet)
            //{
            //    string tag;
            //    tag = objet.Tag.ToString();
            //    return tag;
            //}



        }
    }
}
