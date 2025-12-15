
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
using System.Windows.Threading;//a expliquer

namespace MushroomCatcher
{
    /// <summary>
    /// Logique d'interaction pour JEU.xaml
    /// </summary>
    public partial class JEU : Window
    {

        bool goLeft, goRight, goUp, goDown;
        int playerSpeed = 8;
        int speed = 12;

        int botSpeed = 6; //vitesse du bot
        int stopRange = 15; //tolerennce en pixel pour s'arreter

        int fuyardSpeed = 4;  // vitese du fuyard
        int fleeRange = 500;  // distance ou il commence a fuire


        DispatcherTimer gameTimer = new DispatcherTimer();//a expliquer
        public JEU()
        {

            InitializeComponent();
            moove_map.Focus();

            gameTimer.Tick += GameTimerEvent;//a expliquer
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);//a expliquer
            gameTimer.Start();//a expliquer
        }

        private void ButBoutique_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButRetourJeu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GameTimerEvent(object? sender, EventArgs e)
        {
            if (goLeft && Canvas.GetLeft(Player) > 5)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) - playerSpeed);
            }
            if (goRight && Canvas.GetLeft(Player) + Player.Width < moove_map.ActualWidth - 5)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) + playerSpeed);
            }

            // 3. MOUVEMENT HAUT
            if (goUp && Canvas.GetTop(Player) > 5)
            {
                Canvas.SetTop(Player, Canvas.GetTop(Player) - playerSpeed);
            }

            // 4. MOUVEMENT BAS
            if (goDown && Canvas.GetTop(Player) + Player.Height < moove_map.ActualHeight - 5)
            {
                Canvas.SetTop(Player, Canvas.GetTop(Player) + playerSpeed);
            }
            // CIBLE (Coin supérieur gauche du joueur)
            double playerLeft = Canvas.GetLeft(Player);
            double playerTop = Canvas.GetTop(Player);

            // SOURCE Poursuiveur
            double botLeft = Canvas.GetLeft(mushroomAttack);
            double botTop = Canvas.GetTop(mushroomAttack);

            // SOURCE Fuyard
            double fuyardLeft = Canvas.GetLeft(mushroomRun);
            double fuyardTop = Canvas.GetTop(mushroomRun);


            // Logique du Bot Poursuiveur (Bot)
            // Calcul des différences (Delta)
            double deltaX_Bot = playerLeft - botLeft;
            double deltaY_Bot = playerTop - botTop;

            double absDeltaX_Bot = Math.Abs(deltaX_Bot);
            double absDeltaY_Bot = Math.Abs(deltaY_Bot);

            // Condition d'Arrêt, si l'axe est sup ou egl a la distance d'interaction 
            // il s'arrete sinon se rapproche
            if (absDeltaX_Bot >= stopRange || absDeltaY_Bot >= stopRange)
            {
                // Mouvement Poursuiveur (Priorité à l'axe le plus éloigné)
                if (absDeltaX_Bot >= absDeltaY_Bot)
                {
                    // Mouvement X : On se dirige vers le joueur
                    if (deltaX_Bot > 0)
                    {
                        Canvas.SetLeft(mushroomAttack, botLeft + botSpeed); // Va à droite
                    }
                    else
                    {
                        Canvas.SetLeft(mushroomAttack, botLeft - botSpeed); // Va à gauche
                    }
                }
                else
                {
                    // Mouvement Y : On se dirige vers le joueur
                    if (deltaY_Bot > 0)
                    {
                        Canvas.SetTop(mushroomAttack, botTop + botSpeed); // Va en bas
                    }
                    else
                    {
                        Canvas.SetTop(mushroomAttack, botTop - botSpeed); // Va en haut
                    }
                }
            }
            // Logique du Bot Fuyard (mushroomRun)
            // Calcul des différences (Delta)
            double deltaX_Fuyard = playerLeft - fuyardLeft;
            double deltaY_Fuyard = playerTop - fuyardTop;

            double absDeltaX_Fuyard = Math.Abs(deltaX_Fuyard);
            double absDeltaY_Fuyard = Math.Abs(deltaY_Fuyard);

            // Utilisation de la distance au carré pour la zone de fuite (fleeRange = 250, par exemple)
            double distanceSquared = (deltaX_Fuyard * deltaX_Fuyard) + (deltaY_Fuyard * deltaY_Fuyard);

            if (distanceSquared < fleeRange * fleeRange)
            {
                // Le joueur est dans la zone de fuite : le bot doit fuir !

                // Mouvement Fuyard (Priorité à l'axe le plus éloigné)
                if (absDeltaX_Fuyard >= absDeltaY_Fuyard)
                {
                    // Mouvement X : On s'éloigne du joueur
                    if (deltaX_Fuyard < 0)
                    {
                        Canvas.SetLeft(mushroomRun, fuyardLeft + fuyardSpeed); // Le joueur est à gauche, on va à droite
                    }
                    else
                    {
                        Canvas.SetLeft(mushroomRun, fuyardLeft - fuyardSpeed); // Le joueur est à droite, on va à gauche
                    }
                }
                else
                {
                    // Mouvement Y : On s'éloigne du joueur
                    if (deltaY_Fuyard < 0)
                    {
                        Canvas.SetTop(mushroomRun, fuyardTop + fuyardSpeed); // Le joueur est en haut, on va en bas
                    }
                    else
                    {
                        Canvas.SetTop(mushroomRun, fuyardTop - fuyardSpeed); // Le joueur est en bas, on va en haut
                    }
                }
            }
            //limitation pour que les bot, ici le fuyard ne sort pas de la map quand il fuit 
            double currentLeft = Canvas.GetLeft(mushroomRun);
            double currentTop = Canvas.GetTop(mushroomRun);

            double mapWidth = moove_map.ActualWidth;
            double mapHeight = moove_map.ActualHeight;

            double botWidth = mushroomRun.Width;
            double botHeight = mushroomRun.Height;

            // Collision avec le bord Gauche (X < 0)
            if (currentLeft < 0)
            {
                Canvas.SetLeft(mushroomRun, 0);
            }
            // Collision avec le bord Droit (X > Largeur_Map - Largeur_Bot)
            else if (currentLeft + botWidth > mapWidth)
            {
                // On place le bord droit du bot sur le bord droit de la map
                Canvas.SetLeft(mushroomRun, mapWidth - botWidth);
            }

            // Collision avec le bord Haut (Y < 0)
            if (currentTop < 0)
            {
                Canvas.SetTop(mushroomRun, 0);
            }
            // Collision avec le bord Bas (Y > Hauteur_Map - Hauteur_Bot)
            else if (currentTop + botHeight > mapHeight)
            {
                // On place le bord bas du bot sur le bord bas de la map
                Canvas.SetTop(mushroomRun, mapHeight - botHeight);
            }




        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                goLeft = true;
            }
            if (e.Key == Key.D)
            {
                goRight = true;
            }
            if (e.Key == Key.Z)
            {
                goUp = true;
            }
            if (e.Key == Key.S)
            {
                goDown = true;
            }

        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                goLeft = false;
            }
            if (e.Key == Key.D)
            {
                goRight = false;
            }
            if (e.Key == Key.Z)
            {
                goUp = false;
            }
            if (e.Key == Key.S)
            {
                goDown = false;
            }


        }
    }
}
