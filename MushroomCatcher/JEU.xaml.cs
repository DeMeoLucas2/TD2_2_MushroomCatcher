using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MushroomCatcher
{
    public partial class JEU : Window
    {
        // --- PROPRIÉTÉS JOUEUR ---
        private bool goLeft, goRight, goUp, goDown;
        private const int PlayerSpeed = 32;
        private const int SanteMaximale = 5;
        private int santeActuelle = SanteMaximale;
        private bool estInvincible = false;

        // --- RESSOURCES ---
        private int argent = 0;
        private int score = 0;
        private int compteurPotions = 0;
        private int tauxDrop = 50;

        // --- PROPRIÉTÉS ENNEMIS ---
        private const int BotSpeed = 4;
        private const int StopRange = 25;
        private const int FuyardSpeed = 5;
        private const int FleeRange = 300;

        // --- COMBAT ---
        private int proximityRange = 120;
        private int botClickTolerance = 60;

        // --- TIMERS ---
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private DispatcherTimer spawnTimer = new DispatcherTimer();
        private DispatcherTimer timerInvulnerabilite = new DispatcherTimer();

        public JEU()
        {
            InitializeComponent();
            moove_map.Focus();

            gameTimer.Tick += GameTimerEvent;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Start();

            spawnTimer.Interval = TimeSpan.FromSeconds(5);
            spawnTimer.Tick += SpawnNouvelEnnemi;
            spawnTimer.Start();

            timerInvulnerabilite.Interval = TimeSpan.FromSeconds(1.5);
            timerInvulnerabilite.Tick += (s, e) => {
                estInvincible = false;
                Player.Opacity = 1.0;
                timerInvulnerabilite.Stop();
            };

            MettreAJourBarreDeSante();
            MettreAJourAffichageUI();
        }

        // --- LOGIQUE BOUTIQUE ---        
        public void Vendre(object sender, RoutedEventArgs e)
        {
            if (compteurPotions > 0)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Vendre 1 potion contre 100 points ?\n(Stock : {compteurPotions})",
                    "Boutique", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    compteurPotions--;
                    score += 100;
                    MettreAJourAffichageUI();
                }
            }
            else
            {
                MessageBox.Show("Pas de potions en stock !");
            }
        }

        private void MettreAJourAffichageUI()
        {
            if (TxtPotions != null) TxtPotions.Text = "Potions: " + compteurPotions;
            if (TxtScore != null) TxtScore.Text = "Score: " + score;
        }

        private void ButRetourJeu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void GameTimerEvent(object? sender, EventArgs e)
        {
            DeplacerJoueur();
            LogiqueEnnemis();
            GererCollisions();
        }

        private void DeplacerJoueur()
        {
            if (goLeft && Canvas.GetLeft(Player) > 5) Canvas.SetLeft(Player, Canvas.GetLeft(Player) - PlayerSpeed);
            if (goRight && Canvas.GetLeft(Player) + Player.Width < moove_map.ActualWidth - 5) Canvas.SetLeft(Player, Canvas.GetLeft(Player) + PlayerSpeed);
            if (goUp && Canvas.GetTop(Player) > 5) Canvas.SetTop(Player, Canvas.GetTop(Player) - PlayerSpeed);
            if (goDown && Canvas.GetTop(Player) + Player.Height < moove_map.ActualHeight - 5) Canvas.SetTop(Player, Canvas.GetTop(Player) + PlayerSpeed);
        }

        private void LogiqueEnnemis()
        {
            double pLeft = Canvas.GetLeft(Player);
            double pTop = Canvas.GetTop(Player);

            foreach (var x in moove_map.Children.OfType<Image>().ToList())
            {
                // CHASSEUR (mushroomAttack et mushroomBoss)
                if (x.Name == "mushroomAttack" || x.Name == "mushroomBoss")
                {
                    double bLeft = Canvas.GetLeft(x);
                    double bTop = Canvas.GetTop(x);
                    double speed = (x.Name == "mushroomBoss") ? BotSpeed / 2 : BotSpeed;

                    if (Math.Abs(pLeft - bLeft) > StopRange)
                        Canvas.SetLeft(x, bLeft + (pLeft > bLeft ? speed : -speed));
                    if (Math.Abs(pTop - bTop) > StopRange)
                        Canvas.SetTop(x, bTop + (pTop > bTop ? speed : -speed));
                }
                // FUYARD (mushroomRun)
                else if (x.Name == "mushroomRun")
                {
                    double fLeft = Canvas.GetLeft(x);
                    double fTop = Canvas.GetTop(x);
                    double dist = Math.Sqrt(Math.Pow(pLeft - fLeft, 2) + Math.Pow(pTop - fTop, 2));

                    if (dist < FleeRange)
                    {
                        Canvas.SetLeft(x, fLeft + (pLeft > fLeft ? -FuyardSpeed : FuyardSpeed));
                        Canvas.SetTop(x, fTop + (pTop > fTop ? -FuyardSpeed : FuyardSpeed));
                    }
                }
            }
        }

        private void GererCollisions()
        {
            Rect joueurRect = new Rect(Canvas.GetLeft(Player), Canvas.GetTop(Player), Player.Width, Player.Height);

            foreach (var x in moove_map.Children.OfType<Image>().ToList())
            {
                string tag = x.Tag as string;

                if (!estInvincible && (tag == "ennemi" || x.Name == "mushroomAttack" || x.Name == "mushroomBoss"))
                {
                    Rect ennemiRect = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (joueurRect.IntersectsWith(ennemiRect))
                    {
                        SubirAttaque(1);
                    }
                }

                if (tag == "loot")
                {
                    Rect lootRect = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    if (joueurRect.IntersectsWith(lootRect))
                    {
                        moove_map.Children.Remove(x);
                        compteurPotions++;
                        score += 10;
                        MettreAJourAffichageUI();

                        if (santeActuelle < SanteMaximale)
                        {
                            santeActuelle++;
                            MettreAJourBarreDeSante();
                        }
                    }
                }
            }
        }

        private void MouseClick(object sender, MouseButtonEventArgs e)
        {
            Point clickPos = e.GetPosition(moove_map);
            double pLeft = Canvas.GetLeft(Player);
            double pTop = Canvas.GetTop(Player);

            var cibles = moove_map.Children.OfType<Image>()
                .Where(x => (x.Tag as string) == "ennemi" || x.Name == "mushroomRun" || x.Name == "mushroomBoss")
                .ToList();

            Random rnd = new Random();

            foreach (var target in cibles)
            {
                double tLeft = Canvas.GetLeft(target);
                double tTop = Canvas.GetTop(target);
                double distClick = Math.Sqrt(Math.Pow(clickPos.X - (tLeft + target.Width / 2), 2) + Math.Pow(clickPos.Y - (tTop + target.Height / 2), 2));
                double distJoueur = Math.Sqrt(Math.Pow(pLeft - tLeft, 2) + Math.Pow(pTop - tTop, 2));

                if (distClick < botClickTolerance && distJoueur < proximityRange)
                {
                    if (rnd.Next(1, 101) <= tauxDrop) CreerObjetAuSol(tLeft, tTop);

                    moove_map.Children.Remove(target);
                    score += 20;
                    MettreAJourAffichageUI();
                }
            }
        }

        private void CreerObjetAuSol(double x, double y)
        {
            Image loot = new Image
            {
                Width = 50,
                Height = 50,
                Tag = "loot",
                Source = new BitmapImage(new Uri("pack://application:,,,/MushroomCatcher;component/image_potions/potion_bleu.png"))
            };
            Canvas.SetLeft(loot, x); Canvas.SetTop(loot, y);
            moove_map.Children.Add(loot);
        }

        public void SubirAttaque(int degats)
        {
            if (estInvincible || santeActuelle <= 0) return;
            santeActuelle -= degats;
            estInvincible = true;
            Player.Opacity = 0.5;
            timerInvulnerabilite.Start();
            MettreAJourBarreDeSante();

            if (santeActuelle <= 0)
            {
                gameTimer.Stop(); spawnTimer.Stop();
                MessageBox.Show($"Game Over ! Score : {score}");
            }
        }

        private void MettreAJourBarreDeSante()
        {
            if (ImageBarreSante == null) return;
            string imgName = $"Barre{santeActuelle}V.png";
            try
            {
                ImageBarreSante.Source = new BitmapImage(new Uri($"pack://application:,,,/MushroomCatcher;component/Image/{imgName}"));
            }
            catch { }
        }

        private void SpawnNouvelEnnemi(object? sender, EventArgs e)
        {
            Image nEnnemi = new Image { Width = 100, Height = 100, Tag = "ennemi" };
            nEnnemi.Source = new BitmapImage(new Uri("pack://application:,,,/MushroomCatcher;component/image_gameplay/MushroomToutcontent.png"));
            Random r = new Random();
            Canvas.SetLeft(nEnnemi, r.Next(0, (int)Math.Max(50, moove_map.ActualWidth - 100)));
            Canvas.SetTop(nEnnemi, r.Next(0, (int)Math.Max(50, moove_map.ActualHeight - 100)));
            moove_map.Children.Add(nEnnemi);
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q || e.Key == Key.A) goLeft = true;
            if (e.Key == Key.D) goRight = true;
            if (e.Key == Key.Z || e.Key == Key.W) goUp = true;
            if (e.Key == Key.S) goDown = true;
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q || e.Key == Key.A) goLeft = false;
            if (e.Key == Key.D) goRight = false;
            if (e.Key == Key.Z || e.Key == Key.W) goUp = false;
            if (e.Key == Key.S) goDown = false;
        }
    }
}

