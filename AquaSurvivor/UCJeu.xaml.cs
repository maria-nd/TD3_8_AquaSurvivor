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
        DispatcherTimer timerfaim;
        public UCJeu()
        {
            InitializeComponent();
            string nomFichierImage = $"pack://application:,,,/img/Poissons/{MainWindow.Perso}Gauche.png";
            imgPoisson.Source = new BitmapImage(new Uri(nomFichierImage));
 
            timerfaim = new DispatcherTimer();
            timerfaim.Interval = TimeSpan.FromSeconds(1);
            timerfaim.Tick += FaimDiminue;
            timerfaim.Start();
        }

        public void FaimDiminue(object? sender, EventArgs e)
        {
            if (Faim > 0)
            {
                Faim--;
                barreFaim.Value = Faim;
            }
            else
            {
                timerfaim.Stop();
                MainWindow.perdu = true;

            }

        }

        private void canvasJeu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Right && (Canvas.GetLeft(imgPoisson) + MainWindow.PasPoisson) + imgPoisson.Width < canvasJeu.ActualWidth)
                Canvas.SetLeft(imgPoisson, Canvas.GetLeft(imgPoisson) + MainWindow.PasPoisson);
            // à completer
#if DEBUG
#endif
            if (e.Key == Key.Left && Canvas.GetLeft(imgPoisson) - MainWindow.PasPoisson > 0)
                Canvas.SetLeft(imgPoisson, Canvas.GetLeft(imgPoisson) - MainWindow.PasPoisson);
            // à completer
#if DEBUG
#endif
            if (e.Key == Key.Up && (Canvas.GetTop(imgPoisson) + MainWindow.PasPoisson) - imgPoisson.Height < canvasJeu.ActualHeight)
                Canvas.SetTop(imgPoisson, Canvas.GetTop(imgPoisson) - MainWindow.PasPoisson);
#if DEBUG
#endif
            if (e.Key == Key.Down && Canvas.GetTop(imgPoisson) + MainWindow.PasPoisson > 0)
                Canvas.SetTop(imgPoisson, Canvas.GetTop(imgPoisson) + MainWindow.PasPoisson);

#if DEBUG
#endif
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.KeyDown += canvasJeu_KeyDown;
            //Application.Current.MainWindow.KeyUp += canvasJeu_KeyUp;
        }
    }
}
