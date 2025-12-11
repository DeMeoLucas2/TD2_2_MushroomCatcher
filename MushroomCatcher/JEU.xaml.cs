
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
using System.Windows.Shapes;

namespace MushroomCatcher
{
    /// <summary>
    /// Logique d'interaction pour JEU.xaml
    /// </summary>
    public partial class JEU : UserControl
    {
        private const double VITESSE_DEPLACEMENT = 10;
        public JEU()
        {
            this.Focus();
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            double currentX = Canvas.GetLeft(Joueur);
            double currentY = Canvas.GetTop(Joueur);
            double positionGaucheJoueur = Canvas.GetLeft(Joueur);
            if (e.Key == Key.Left)
            {
                if (positionGaucheJoueur > 0)
                {
                    Canvas.SetLeft(Joueur, positionGaucheJoueur - VITESSE_DEPLACEMENT);
                }
            }
            else if (e.Key == Key.Right)
            {
                if (positionGaucheJoueur + Joueur.Width < map.ActualWidth)
                {
                    Canvas.SetLeft(Joueur, positionGaucheJoueur + VITESSE_DEPLACEMENT);
                }
            }
            else if (e.Key == Key.Up)
            {
                // Limite Haut (ne peut pas aller en Y < 0)
                if (currentY > 0)
                {
                    // La position Y diminue quand on monte
                    Canvas.SetTop(Joueur, currentY - VITESSE_DEPLACEMENT);
                }
            }
            else if (e.Key == Key.Down)
            {
                // Limite Bas (currentY + hauteur du Joueur ne doit pas dépasser la hauteur du Canvas)
                if (currentY + Joueur.Height < map.ActualHeight)
                {
                    // La position Y augmente quand on descend
                    Canvas.SetTop(Joueur, currentY + VITESSE_DEPLACEMENT);
                }
            }
            e.Handled = true;
        }
    }
}
