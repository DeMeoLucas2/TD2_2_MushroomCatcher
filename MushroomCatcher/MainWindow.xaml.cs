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
        JEU jeuWindow;
        public MainWindow()
        {
            InitializeComponent();
            AfficheDemarrage();
        }

        private void AfficheDemarrage()
        {
            Ecran_affichage uc = new Ecran_affichage(); // crée et charge l'écran de démarrage
            ZoneJeu.Content = uc; // associe l'écran au conteneur
            uc.ButJouer.Click += AfficherRegle; // affiche l'écran des règles
            uc.ButCredit.Click += AfficherCredit; // affiche l'écran de crédit
            uc.ButQuitter.Click += AfficherQuitter; // affiche l'écran pour quitter
        }

        private void AfficherRegle(object sender, RoutedEventArgs e)
        {
            EcranRegle uc = new EcranRegle(); // crée et charge l'écran de règles
            ZoneJeu.Content = uc; // associe l'écran au conteneur
            uc.ButRetourRegle.Click += AfficheRetour; // retourne à l'écran de démarrage
            uc.ButOuiRegle.Click += AfficheJeu; // affiche l'écran de jeu
        }

        private void AfficherCredit(object sender, RoutedEventArgs e)
        {
            EcranCredit uc = new EcranCredit(); // crée et charge l'écran de crédit
            ZoneJeu.Content = uc; // associe l'écran au conteneur
            uc.ButRetourCredit.Click += AfficheRetour; // retourne à l'écran de démarrage
        }

        private void AfficherQuitter(object sender, RoutedEventArgs e)
        {
            EcranQuitter uc = new EcranQuitter(); // crée et charge l'écran pour quitter
            ZoneJeu.Content = uc; // associe l'écran au conteneur
            uc.ButRetourQuitter.Click += AfficheRetour; // retourne à l'écran de démarrage
        }

        private void AfficheRetour(object sender, RoutedEventArgs e)
        {
            if (ZoneJeu.Content == jeuWindow)
            {
                jeuWindow.Hide();
            }
            Ecran_affichage uc = new Ecran_affichage(); // crée et charge l'écran de démarrage
            ZoneJeu.Content = uc; // associe l'écran au conteneur
            uc.ButJouer.Click += AfficherRegle; // affiche l'écran des règles
            uc.ButCredit.Click += AfficherCredit; // affiche l'écran de crédit
            uc.ButQuitter.Click += AfficherQuitter; // affiche l'écran pour quitter
        }

        private void AfficheJeu(object sender, RoutedEventArgs e)
        {
            jeuWindow = new JEU(); // crée et charge l'écran de démarrage
            jeuWindow.Show(); // associe l'écran au conteneur
            jeuWindow.ButBoutique.Click += AfficheBoutique; // affiche l'écran de la boutique
            jeuWindow.ButRetourJeu.Click += AfficheRetour; // affiche l'écran de démmarrage
        }

        private void AfficheRetourAuJeu(object sender, RoutedEventArgs e)
        {
            jeuWindow.RelancerLeJeu();
            jeuWindow.Show(); // associe l'écran au conteneur
        }

        public void AfficheBoutique(object sender, RoutedEventArgs e)
        {
            jeuWindow.ArreterLeJeu();
            jeuWindow.Hide();
            EcranBoutique uc = new EcranBoutique(); // crée et charge l'écran de la boutique
            ZoneJeu.Content = uc; // associe l'écran au conteneur
            uc.ButVendre.Click += jeuWindow.Vendre;
            uc.ButRetourBoutique.Click += AfficheRetourAuJeu; // retourne à l'écran de jeu            
        }


        private void AfficheRetourBoutique(object sender, RoutedEventArgs e)
        {
            EcranBoutique uc = new EcranBoutique(); // crée et charge l'écran de la boutique
            ZoneJeu.Content = uc; // associe l'écran au conteneur
            uc.ButRetourBoutique.Click += AfficheRetourAuJeu; // retourne à l'écran de jeu
            uc.ButVendre.Click += AfficheVente; // affiche l'écran de vente
        }

        private void AfficheVente(object sender, RoutedEventArgs e)
        {
            EcranVente uc = new EcranVente(); // crée et charge l'écran de vente
            ZoneJeu.Content = uc; // associe l'écran au conteneur
            uc.ButRetourVendre.Click += AfficheRetourBoutique; // retourne à l'écran de boutique
        }
        
    }
}