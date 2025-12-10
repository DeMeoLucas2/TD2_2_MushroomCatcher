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

namespace MushroomCatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AfficheDemarrage();
        }

        private void AfficheDemarrage()
        {
            Ecran_affichage  uc = new Ecran_affichage(); // crée et charge l'écran de démarrage
            ZoneJeu.Content = uc; // associe l'écran au conteneur
            uc.ButJouer.Click += AfficherJeu; // affiche l'écran de jeu
            uc.ButCredit.Click += AfficherCredit; // affiche l'écran de crédit
            uc.ButQuitter.Click += AfficherQuitter; // affiche l'écran pour quitter
        }

        private void AfficherJeu(object sender, RoutedEventArgs e)
        {
            EcranRegle uc = new EcranRegle();
            ZoneJeu.Content = uc;
        }

        private void AfficherCredit(object sender, RoutedEventArgs e)
        {
            EcranCredit uc = new EcranCredit();
            ZoneJeu.Content = uc;
        }

        private void AfficherQuitter(object sender, RoutedEventArgs e)
        {
            EcranQuitter uc = new EcranQuitter();
            ZoneJeu.Content = uc;
        }
    }
}