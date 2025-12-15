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
        //déplacement du poisson, régeneration barre de faim grace à nouritture, dégât des déchets sur le temps (en seconde ), dégat de la méduse sur la barre de faim, Boost (en pas) de l'étoile, déplacemet déchet (en pas)
        private static int tempsRestant = 100;
        private static int score = 0;
        private static int [] objectif = [30, 40, 55, 75];
        private static string dernierePositionHorizontale="";
        private static string [] lesObjets= [ "Nourriture" , "Objets Séciaux", "Déchets"];



        private Random rnd = new Random();
        private readonly int NB_OBJETS = 45;
        private Image[] lesObjets;
        //private string[,] lesObjets = { { "imgCalamar.png", "imgCrevette.png", "imgPoissonJaune.png", "imgSardine.png", "imgSardine.png", }, { "imgPerle.png", "imgEtoileDeMer.png", "imgMeduse.png", "imgMeduse.png", "imgMeduse.png" }, { "imgBouteille.png", "imgCigare.png", "imgCigarette.png", "imgPoubelle.png", "imgSacPlastique.png" } };

        private int compteur = 0;

        private string[] obj_Nourriture_Basics = { "imgCalamar.png", "imgCrevette.png", "imgPoissonJaune.png", "imgSardine.png" };
        private string[] obj_Speciaux = { "imgPerle.png", "imgEtoileDeMer.png", "imgMeduse.png" };
        private string[] obj_Dechets = { "imgBouteille.png", "imgCigare.png", "imgCigarette.png", "imgPoubelle.png", "imgSacPlastique.png" };






        public UCJeu()
        {
            InitializeComponent();
            ChangerImage("Gauche");

            timerJeuPrincipal = new DispatcherTimer();
            timerJeuPrincipal.Interval = TimeSpan.FromMilliseconds(50);
            timerJeuPrincipal.Tick += JeuCompteur;
            timerJeuPrincipal.Start();

            //jsp si je laisse comme ça
            timerBoost = new DispatcherTimer();
            timerBoost.Interval = TimeSpan.FromSeconds(1);
            timerBoost.Tick += BoostVitesse;

            InitObjets();

            // Perletoucher();
        }
        private (string nom, string img) GetRandomImageProperties()
        {
            //  60% Déchet, 25% Nourriture, 15% Spécial
            int chance = rnd.Next(0, 100);
            string nom;
            string img;

            if (chance < 50) // 60% Déchets
            {
                nom = obj_Dechets[rnd.Next(obj_Dechets.Length)];
                img = "Déchets";
            }
            else if (chance < 75) // 25% Nourriture basique
            {
                nom = obj_Nourriture_Basics[rnd.Next(obj_Nourriture_Basics.Length)];
                img = "Nourriture";
            }
            else 
    {
                nom = obj_Speciaux[rnd.Next(obj_Speciaux.Length)];
                img = "Nourriture";
            }
            return (nom, img);
        }
        private void GenererObjetAleatoire()
        {
            // 1. Choisir un type au hasard
            string[] types = { "Nourriture", "Dechet", "Boost" };
            string typeChoisi = types[rnd.Next(types.Length)];
            // 2. Choisir un fichier selon le type
            string dossier = "";
            switch (typeChoisi)
            {
                case "Nourriture":
                    dossier = "Images/Nourriture";
                    break;
                case "Dechet":
                    dossier = "Images/Dechets";
                    break;
                case "Boost":
                    dossier = "Images/Boosts";
                    break;
            }
            string[] fichiers = Directory.GetFiles(dossier, "*.png");
            if (fichiers.Length == 0) return;
            string chemin = fichiers[rnd.Next(fichiers.Length)];
            Image img = new Image();
            img.Width = 40;
            img.Height = 40;
            img.Source = new BitmapImage(new Uri(chemin, UriKind.RelativeOrAbsolute));
            // 4. Position aléatoire sur le Canvas
            double x = rnd.Next(0, (int)(canvasJeu.ActualWidth - img.Width));
            double y = rnd.Next(0, (int)(canvasJeu.ActualHeight - img.Height));
            Canvas.SetLeft(img, x);
            Canvas.SetTop(img, y);
            // 5. Tag = type de l’objet
            img.Tag = typeChoisi;
            // 6. Ajouter au Canvas et à la liste
            canvasJeu.Children.Add(img);
            lesObjets =  [img];

        }

        private void InitObjets()
        {
            lesObjets = new Image[NB_OBJETS];

            for (int i = 0; i < lesObjets.Length; i++)
            {
                var props = GetRandomImageProperties();

                lesObjets[i] = new Image
                {
                    Source = new BitmapImage(new Uri($"pack://application:,,,/img/{props.img}/{props.nom}")),
                    Width = 50,
                    Height = 50,
                };

                canvasJeu.Children.Add(lesObjets[i]);

                double maxLeft = canvasJeu.ActualWidth - lesObjets[i].Width;
                Canvas.SetLeft(lesObjets[i], rnd.Next(400,800));
                Canvas.SetTop(lesObjets[i], -rnd.Next(200, 800) * (i + 1));
            }
        }
        private void JeuCompteur(object? sender, EventArgs e)
        {
            compteur++;
            if (compteur * 50 >= 1000) //1sec
            {
                int baisseFaim = NiveauDifficulte[MainWindow.NiveauChoisi, 0];
                if (Faim > 0)
                {
                    Faim -= 1;
                    if (Faim < 0) Faim = 0;
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

        private void DeplacementObjets()
        {
            int pasMouvement = NiveauDifficulte[MainWindow.NiveauChoisi, 5];

            for (int i = 0; i < lesObjets.Length; i++)
            {
                Image objet = lesObjets[i];

                Canvas.SetTop(objet, Canvas.GetTop(objet) + pasMouvement);

                if (Canvas.GetTop(objet) > canvasJeu.ActualHeight)
                {
                    string source = objet.Source.ToString();

                    if (!source.Contains("/Déchets/"))
                    {
                        Faim -= 5;
                        if (Faim < 0) Faim = 0;
                        barreFaim.Value = Faim;
                    }

                   ResetObjet(objet);
                }
            }
        }

        private void ResetObjet(Image objet)
        {
            var props = GetRandomImageProperties();

            objet.Source = new BitmapImage(new Uri($"pack://application:,,,/img/{props.img}/{props.nom}"));

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

            for (int i = 0; i < lesObjets.Length; i++)
            {
                Image objet = lesObjets[i];

                Rect rectObjet = new Rect(
                    Canvas.GetLeft(objet),
                    Canvas.GetTop(objet),
                    objet.Width,
                    objet.Height
                );

                if (rectPoisson.IntersectsWith(rectObjet))
                {
                    string source = objet.Source.ToString();
                    if (source.Contains("imgPerle"))
                    {
                        Faim += NiveauDifficulte[MainWindow.NiveauChoisi, 1];
                        if (Faim > 100) Faim = 100;
                        barreFaim.Value = Faim;

                        score++;
                        // labelScore.Content = $"Score : {score} / {objectif[MainWindow.NiveauChoisi]}"; 
                    }
                    else if (source.Contains("imgEtoileDeMer"))
                    {
                        NiveauDifficulte[MainWindow.NiveauChoisi, 0] += NiveauDifficulte[MainWindow.NiveauChoisi, 4];
                        boost = 10;
                        if (timerBoost != null) timerBoost.Start();
                    }
                    else if (source.Contains("imgMeduse"))
                    {
                        Faim -= NiveauDifficulte[MainWindow.NiveauChoisi, 3];
                        if (Faim < 0) Faim = 0;
                        barreFaim.Value = Faim;
                    }
                    else if (source.Contains("/Déchets/"))
                    {
                        // C'est un déchet standard (vérification par dossier)
                        tempsRestant -= NiveauDifficulte[MainWindow.NiveauChoisi, 2];
                    }
                    else if (source.Contains("/Nourriture/"))
                    {
                        // C'est une nourriture standard
                        Faim += NiveauDifficulte[MainWindow.NiveauChoisi, 1];
                        if (Faim > 100) Faim = 100;
                        barreFaim.Value = Faim;
                    }

                   ResetObjet(objet);
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
                    labelScore.Content = $"Score : {score} /{objectif}";
                }
            }
            
        }

        private void Empoisonner()
        {
            Faim -= NiveauDifficulte[MainWindow.NiveauChoisi, 3];
            barreFaim.Value = Faim;
        }
        private void BoostVitesse() // J'ai retirer object sender, EventArgs e à  remettre si jamais ca pose problème
        {
            barreBoost.Opacity = 1;
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


        private void ChangerImage(string direction)
        {

            string nomFichierImage = $"pack://application:,,,/img/Poissons/{MainWindow.Perso}{direction}.png";
            imgPoisson.Source = new BitmapImage(new Uri(nomFichierImage));

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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyDown += DeplacementPoisson;
            //Application.Current.MainWindow.KeyUp += canvasJeu_KeyUp;
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

        // Méthode pour reprendre le jeu (appelée après la fermeture du Menu/Règles)
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


        //private void collision(object sender, RoutedEventArgs e)
        //{
        //    Rect rectPoisson = new Rect(Canvas.GetLeft(imgPoisson), Canvas.GetTop(imgPoisson), imgPoisson.Width, imgPoisson.Height);
        //    for (int j = 0; j < lesObjets.Length; j++)
        //    {
        //            for (int i = 0; i < objetNourriture.GetLength(1); i++)
        //            {
        //                Image objet = lesObjets[i];
        //                Rect rectobjet = new Rect(Canvas.GetLeft(objet), Canvas.GetTop(objet), objet.Width, objet.Height);
        //                if (rectPoisson.IntersectsWith(rectobjet))
        //                    {
        //                if (quelObjet == 0) // 0 = nourriture
        //                {
        //                    Faim += NiveauDifficulte[MainWindow.NiveauChoisi, 1]; //On ajoute des "points" à la faim selon le niveau choisis

        //                }
        //                else if (quelObjet == 1) // 1= objets Spéciaux
        //                {
        //                    if (i == 0) // étoile de mer
        //                    {
        //                        Perletoucher(); // On ajoute au score +1
        //                    }
        //                    else if (i == 1) // boost de vitesse
        //                    {
        //                        BoostVitesse(); // On active le boost de vitesse le poisson va plus vite selon le niveau choisis et on a la barreBoost qui aparraît à l'écran
        //                    }
        //                    else // Méduse
        //                    {
        //                        Empoisonner();
        //                    }
        //                }


        //                else
        //                {
        //                    //timerJeuPrincipal-= NiveauDifficulte[MainWindow.NiveauChoisi, 3]; //On retire du temps selon le niveau choisis

        //                }


        //                }
        //            }
        //            quelObjet++;

        //    }

        //}

        //private string typeObjet(Image objet)
        //{
        //    string tag;
        //    tag = objet.Tag.ToString();
        //    return tag;
        //}



    }
}
