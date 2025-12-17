using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MushroomCatcher
{
    public partial class JEU : Window
    {
        // --- PROPRIÉTÉS JOUEUR ---
        private bool goLeft, goRight, goUp, goDown;
        private const int PlayerSpeed = 12;
        private const int SanteMaximale = 5;
        private int santeActuelle = SanteMaximale;
        private bool estInvincible = false;

        // --- RESSOURCES ---
        private int score = 0;
        private int compteurPotions = 0;
        private int tauxDrop = 100;

        // --- MUSIQUE ---
        // Déclarez le MediaPlayer en tant que membre de la classe
        private System.Windows.Media.MediaPlayer musicPlayer = new System.Windows.Media.MediaPlayer();
        // Déclarez la variable de volume si elle n'existe pas encore
        private double MusicVolume = 1; // Exemple : volume à 50%

        // --- PROPRIÉTÉS ENNEMIS ---
        private const int BotSpeed = 4;
        private const int StopRange = 25;
        private const int FuyardSpeed = 11;
        private const int FleeRange = 300;

        // --- COMBAT ---
        private int proximityRange = 120;
        private int botClickTolerance = 200;
        private int fuyardClickTolerance = 200;

        // --- TIMERS ---
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private DispatcherTimer spawnTimer = new DispatcherTimer();
        private DispatcherTimer timerInvulnerabilite = new DispatcherTimer();

        // --- VARIABLES D'ANIMATION ---
        private BitmapImage[] imagesMarche = new BitmapImage[4];
        private BitmapImage[] imagesChampiMechant = new BitmapImage[3]; // Il y a 3 images
        private int frameMechant = 0;

        // Animation Champignon Fuyard (Run)
        private BitmapImage[] imagesChampiFuyard = new BitmapImage[2]; // Il y a 2 images
        private int frameFuyard = 0;

        private int compteurAnimChampis = 0;
        private int frameActuelle = 0;
        private int compteurAnimation = 0;
        private int vitesseAnimation = 5;

        // Déclaration du Random statique et unique (Utilisation cohérente)
        private static readonly Random rnd = new Random();


        public JEU()
        {
            InitializeComponent();
            moove_map.Focus();
            LancerMusique();

            // Chargement avec le chemin exact vers tes dossiers
            imagesMarche[0] = new BitmapImage(new Uri("pack://application:,,,/MushroomCatcher;component/animationFinal/animation nouveau final/hero_animation/main_hero1.png"));
            imagesMarche[1] = new BitmapImage(new Uri("pack://application:,,,/MushroomCatcher;component/animationFinal/animation nouveau final/hero_animation/main_hero2.png"));
            imagesMarche[2] = new BitmapImage(new Uri("pack://application:,,,/MushroomCatcher;component/animationFinal/animation nouveau final/hero_animation/main_hero3.png"));
            imagesMarche[3] = new BitmapImage(new Uri("pack://application:,,,/MushroomCatcher;component/animationFinal/animation nouveau final/hero_animation/main_hero4.png"));

            imagesChampiMechant[0] = new BitmapImage(new Uri("pack://application:,,,/MushroomCatcher;component/animationFinal/animation nouveau final/animation champignon mechant/champiAW1.png"));
            imagesChampiMechant[1] = new BitmapImage(new Uri("pack://application:,,,/MushroomCatcher;component/animationFinal/animation nouveau final/animation champignon mechant/champiAW2.png"));
            imagesChampiMechant[2] = new BitmapImage(new Uri("pack://application:,,,/MushroomCatcher;component/animationFinal/animation nouveau final/animation champignon mechant/champiAW3.png"));

            // Chargement Fuyard (champiFW)
            imagesChampiFuyard[0] = new BitmapImage(new Uri("pack://application:,,,/MushroomCatcher;component/animationFinal/animation nouveau final/animtion champu. fuit/champiFW1.png"));
            imagesChampiFuyard[1] = new BitmapImage(new Uri("pack://application:,,,/MushroomCatcher;component/animationFinal/animation nouveau final/animtion champu. fuit/champiFW2.png"));

            // --- MODE PLEIN ÉCRAN ---
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Maximized;
            this.Topmost = true;

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

        public void ArreterLeJeu()
        {
            gameTimer.Stop();
            spawnTimer.Stop();
        }

        public void RelancerLeJeu()
        {
            gameTimer.Start();
            spawnTimer.Start();
        }
        
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

        // *** SUPPRIMÉ : Laissez le handler XAML pointer vers Vendre ***
        // private void ButBoutique_Click(object sender, RoutedEventArgs e) { } 

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
            AnimerPersonnage();
            AnimerChampignons();
        }

        private void AnimerPersonnage()
        {
            // Si une touche de mouvement est enfoncée
            if (goLeft || goRight || goUp || goDown)
            {
                compteurAnimation++;
                if (compteurAnimation >= vitesseAnimation)
                {
                    compteurAnimation = 0;
                    frameActuelle++;

                    if (frameActuelle > 3) frameActuelle = 0; // Retour à la première image

                    // On change l'image affichée sur le Canvas
                    Player.Source = imagesMarche[frameActuelle];
                }
            }
            else
            {
                // Si on ne bouge pas, on remet l'image de base
                Player.Source = imagesMarche[0];
            }
        }

        private void AnimerChampignons()
        {
            compteurAnimChampis++;

            if (compteurAnimChampis >= 6) // Vitesse de l'animation
            {
                compteurAnimChampis = 0;

                // Animation du Méchant (boucle de 3 images)
                frameMechant++;
                if (frameMechant > 2) frameMechant = 0;
                mushroomAttack.Source = imagesChampiMechant[frameMechant];

                // Animation du Fuyard (boucle de 2 images)
                frameFuyard++;
                if (frameFuyard > 1) frameFuyard = 0;
                mushroomRun.Source = imagesChampiFuyard[frameFuyard];

                // Si tu as un Boss, tu peux aussi lui mettre l'image du méchant
                // mushroomBoss.Source = imagesChampiMechant[frameMechant];
            }
        }
        private void DeplacerJoueur()
        {
            if (goLeft && Canvas.GetLeft(Player) > 5) 
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) - PlayerSpeed);
            
            if (goRight && Canvas.GetLeft(Player) + Player.Width < moove_map.ActualWidth - 5)
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) + PlayerSpeed);

            if (goUp && Canvas.GetTop(Player) > 5)
                Canvas.SetTop(Player, Canvas.GetTop(Player) - PlayerSpeed);
           
            if (goDown && Canvas.GetTop(Player) + Player.Height < moove_map.ActualHeight - 5) 
                Canvas.SetTop(Player, Canvas.GetTop(Player) + PlayerSpeed);


        }

       

        private void LogiqueEnnemis()
        {
            // CIBLE (Position du joueur)
            double pLeft = Canvas.GetLeft(Player);
            double pTop = Canvas.GetTop(Player);

            // 1. LOGIQUE POUR mushroomAttack (CHASSEUR)
            double botLeft = Canvas.GetLeft(mushroomAttack);
            double botTop = Canvas.GetTop(mushroomAttack);
            double deltaX_Bot = pLeft - botLeft;
            double deltaY_Bot = pTop - botTop;

            if (Math.Abs(deltaX_Bot) >= StopRange || Math.Abs(deltaY_Bot) >= StopRange)
            {
                if (Math.Abs(deltaX_Bot) >= Math.Abs(deltaY_Bot))
                {
                    if (deltaX_Bot > 0) { Canvas.SetLeft(mushroomAttack, botLeft + BotSpeed); }
                    else { Canvas.SetLeft(mushroomAttack, botLeft - BotSpeed); }
                }
                else
                {
                    if (deltaY_Bot > 0) { Canvas.SetTop(mushroomAttack, botTop + BotSpeed); }
                    else { Canvas.SetTop(mushroomAttack, botTop - BotSpeed); }
                }
            }

            //// 2. LOGIQUE POUR mushroomBoss (CHASSEUR LENT)
            //double bossLeft = Canvas.GetLeft(mushroomBoss);
            //double bossTop = Canvas.GetTop(mushroomBoss);
            //double deltaX_Boss = pLeft - bossLeft;
            //double deltaY_Boss = pTop - bossTop;
            //double bossSpeed = BotSpeed / 2; // Vitesse réduite pour le boss

            //if (Math.Abs(deltaX_Boss) >= StopRange || Math.Abs(deltaY_Boss) >= StopRange)
            //{
            //    if (Math.Abs(deltaX_Boss) >= Math.Abs(deltaY_Boss))
            //    {
            //        if (deltaX_Boss > 0) { Canvas.SetLeft(mushroomBoss, bossLeft + bossSpeed); }
            //        else { Canvas.SetLeft(mushroomBoss, bossLeft - bossSpeed); }
            //    }
            //    else
            //    {
            //        if (deltaY_Boss > 0) { Canvas.SetTop(mushroomBoss, bossTop + bossSpeed); }
            //        else { Canvas.SetTop(mushroomBoss, bossTop - bossSpeed); }
            //    }
            //}


            // 3. LOGIQUE POUR mushroomRun (FUYARD)
            double fLeft = Canvas.GetLeft(mushroomRun);
            double fTop = Canvas.GetTop(mushroomRun);
            double deltaX_Fuyard = pLeft - fLeft;
            double deltaY_Fuyard = pTop - fTop;
            double distSq = (deltaX_Fuyard * deltaX_Fuyard) + (deltaY_Fuyard * deltaY_Fuyard);

            if (distSq < FleeRange * FleeRange)
            {
                if (Math.Abs(deltaX_Fuyard) >= Math.Abs(deltaY_Fuyard))
                {
                    if (deltaX_Fuyard < 0) { Canvas.SetLeft(mushroomRun, fLeft + FuyardSpeed); }
                    else { Canvas.SetLeft(mushroomRun, fLeft - FuyardSpeed); }
                }
                else
                {
                    if (deltaY_Fuyard < 0) { Canvas.SetTop(mushroomRun, fTop + FuyardSpeed); }
                    else { Canvas.SetTop(mushroomRun, fTop - FuyardSpeed); }
                }
            }

            // --- LIMITATION DES BORDURES (Pour le fuyard) ---
            double currentLeft = Canvas.GetLeft(mushroomRun);
            double currentTop = Canvas.GetTop(mushroomRun);

            if (currentLeft < 0) { Canvas.SetLeft(mushroomRun, 0); }
            else if (currentLeft + mushroomRun.Width > moove_map.ActualWidth)
            {
                Canvas.SetLeft(mushroomRun, moove_map.ActualWidth - mushroomRun.Width);
            }

            if (currentTop < 0) { Canvas.SetTop(mushroomRun, 0); }
            else if (currentTop + mushroomRun.Height > moove_map.ActualHeight)
            {
                Canvas.SetTop(mushroomRun, moove_map.ActualHeight - mushroomRun.Height);
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
            // 1. Récupérer la position du clic
            Point clickPos = e.GetPosition(moove_map);

            // 2. Identifier l'objet qui a été cliqué
            IInputElement elementClique = moove_map.InputHitTest(clickPos);

            // 3. Vérifier si cet objet est bien une Image (un ennemi)
            if (elementClique is Image cible)
            {
                // On vérifie qu'on ne clique pas sur le joueur lui-même
                if (cible.Name == "Player") return;

                // --- IMMORTALITÉ DU MUSHROOM ATTACK ---
                if (cible.Name == "mushroomAttack") return;

                // Calcul des distances
                double pLeft = Canvas.GetLeft(Player);
                double pTop = Canvas.GetTop(Player);
                double tLeft = Canvas.GetLeft(cible);
                double tTop = Canvas.GetTop(cible);

                double distJoueur = Math.Sqrt(Math.Pow(pLeft - tLeft, 2) + Math.Pow(pTop - tTop, 2));

                // On vérifie si le joueur est assez proche pour attaquer
                if (distJoueur < proximityRange)
                {
                    // --- RÉCOMPENSE SI C'EST UN FUYARD ---
                    // On vérifie si le nom contient "Run" ou ton nouveau nom
                    if (cible.Name == "mushroomRun" || cible.Name == "mushroomrourcontent")
                    {
                        score = score + 300;
                    }

                    // Logique de drop
                    if (rnd.Next(1, 101) <= tauxDrop)
                    {
                        CreerObjetAuSol(tLeft, tTop);
                    }

                    // On retire l'image cliquée du jeu
                    moove_map.Children.Remove(cible);
                    MettreAJourAffichageUI();
                }
            }
        }

        private void CreerObjetAuSol(double x, double y)
        {
            try
            {
                string[] potions = { "potion_bleu.png", "potion_rouge.png", "potion_verte.png", "potion_jaune.png", "potion_violette.png" };

                // Utilisation de rnd.Next(potions.Length)
                string potionChoisie = potions[rnd.Next(potions.Length)];

                Image loot = new Image
                {
                    Width = 50,
                    Height = 50,
                    Tag = "loot",
                    // *** CORRECTION DE L'URI ICI ***
                    Source = new BitmapImage(new Uri($"pack://application:,,,/MushroomCatcher;component/image_potions/{potionChoisie}", UriKind.RelativeOrAbsolute))
                };

                Canvas.SetLeft(loot, x);
                Canvas.SetTop(loot, y);
                moove_map.Children.Add(loot);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur création potion : " + ex.Message);
            }
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

            // Utilisation du Random statique et unique (rnd)
            Canvas.SetLeft(nEnnemi, rnd.Next(0, (int)Math.Max(50, moove_map.ActualWidth - 100)));
            Canvas.SetTop(nEnnemi, rnd.Next(0, (int)Math.Max(50, moove_map.ActualHeight - 100)));
            moove_map.Children.Add(nEnnemi);
        }

        private void LancerMusique()

        {
            {
                musicPlayer.Open(new Uri("soundtrack/music_fond.mp3", UriKind.Relative));
                musicPlayer.MediaEnded += (s, e) => { musicPlayer.Position = TimeSpan.Zero; musicPlayer.Play(); };
                musicPlayer.Volume = MusicVolume;
                musicPlayer.Play();
            }
        }

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // On met à jour la variable MusicVolume
            MusicVolume = e.NewValue;

            // On applique immédiatement au lecteur de musique
            if (musicPlayer != null)
            {
                musicPlayer.Volume = MusicVolume;
            }
        }


        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q) goLeft = true;
            if (e.Key == Key.D) goRight = true;
            if (e.Key == Key.Z) goUp = true;
            if (e.Key == Key.S) goDown = true;
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q) goLeft = false;
            if (e.Key == Key.D) goRight = false;
            if (e.Key == Key.Z) goUp = false;
            if (e.Key == Key.S) goDown = false;
        }
    }
}