
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
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                goLeft = true;
            }
            if (e.Key == Key.Right)
            {
                goRight = true;
            }
            if (e.Key == Key.Up)
            {
                goUp = true;
            }
            if (e.Key == Key.Down)
            {
                goDown = true;
            }

        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                goLeft = false;
            }
            if (e.Key == Key.Right)
            {
                goRight = false;
            }
            if (e.Key == Key.Up)
            {
                goUp = false;
            }
            if (e.Key == Key.Down)
            {
                goDown = false;
            }


        }
    }
}
