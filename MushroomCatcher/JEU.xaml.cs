
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Diagnostics;
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
using static MushroomCatcher.JEU;
using System.Diagnostics;
using System.Threading;


namespace MushroomCatcher
{
    /// <summary>
    /// Logique d'interaction pour JEU.xaml
    /// </summary>
    public partial class JEU : Window
    {

        bool goLeft, goRight, goUp, goDown;
        int playerSpeed = 8;

        int botSpeed =4; //vitesse du bot
        int stopRange = 50; //tolerennce en pixel pour s'arreter

        int fuyardSpeed = 5;  // vitese du fuyard
        int fleeRange = 300;  // distance ou il commence a fuire

        private int botClickTolerance = 200; //distancxe a laquel le clic est reconnu

        private int proximityRange = 80; // Zone de tolérance pour le joueur (par exemple, le joueur doit être à moins de 80 pixels du Bot)
        private int botHealth = 2; // Le bot commence avec 2 points de vie

        // Propriétés de santé
        private const int SanteMaximale = 3;
        private int santeActuelle = SanteMaximale;

        private bool estInvincible = false;
        private DispatcherTimer timerInvulnerabilite = new DispatcherTimer();
        DispatcherTimer gameTimer = new DispatcherTimer();//a expliquer
        public JEU()
        {

            InitializeComponent();
            moove_map.Focus();
            Ajouter_enemis(moove_map);

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

            // MOUVEMENT HAUT
            if (goUp && Canvas.GetTop(Player) > 5)
            {
                Canvas.SetTop(Player, Canvas.GetTop(Player) - playerSpeed);
            }

            // MOUVEMENT BAS
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
            /* Essaie spawn ennemi
       Ps : Cela ne fonctionne pas
       // Définition de la classe pour un ennemi
       public class Enemy
       {
           public string Name { get; private set; }

           public (int X, int Y) Position { get; set; }

           public Enemy(int initialX, int initialY)
           {
               this.Name = "AngryMush"; // Nom de l'ennemi par défaut

               this.Position = (initialX, initialY);
               Console.WriteLine($"[ENEMIES] Un {Name} est apparu à ({initialX}, {initialY}) !");
           }

           // Méthode de base pour l'ennemi
           public void Move()
           {
               // Logique de mouvement simple (par exemple, se déplace de 1 unité vers le bas)
               Position = (Position.X, Position.Y + 1);
           }
       }

       // Définition de la classe pour un spawner à ennemi
       public class EnemySpawner
       {

           public List<Enemy> ActiveEnemies { get; private set; } // Liste de stokage de tous les ennemis actifs dans le jeu

           private (int X, int Y) spawnLocation = (50, 30); // Position de spawn

           // Sert à gérer le délai entre les spawns
           private float timeSinceLastSpawn = 0f;
           private const float SpawnInterval = 3.0f; // Apparition toutes les 3 secondes

           public EnemySpawner()
           {
               ActiveEnemies = new List<Enemy>();
           }

           public void Update(float deltaTime)
           {
               // Mise à jour du temps écoulé
               timeSinceLastSpawn += deltaTime;

               // Vérification si l'intervalle est atteint
               if (timeSinceLastSpawn >= SpawnInterval)
               {
                   // Réinitialisation du compteur de temps
                   timeSinceLastSpawn = 0f;

                   // Lancement de la fonction de création
                   SpawnNewEnemy();
               }
           }

           private void SpawnNewEnemy()
           {
               // Crée une nouvelle instance de l'ennemi
               Enemy newEnemy = new Enemy(spawnLocation.X, spawnLocation.Y);

               // Ajoute l'ennemi à la liste des ennemis actifs
               ActiveEnemies.Add(newEnemy);
           }
       }

       public class Program
       {
           public static void SpawnRepeat(string[] args)
           {
               EnemySpawner spawner = new EnemySpawner(); // Crée l'objet Spawner

               // Mesure le temps très précisément 
               Stopwatch stopwatch = new Stopwatch();
               stopwatch.Start();

               // La boucle de jeu principale
               while (true)
               {
                   // Calcul du DeltaTime (temps écoulé depuis dernier "tour")
                   float deltaTime = (float)stopwatch.Elapsed.TotalSeconds;
                   stopwatch.Restart(); // Réinitialise timer du prochain tour

                   // Mise à jour du spawner
                   spawner.Update(deltaTime);

                   // Limite la vitesse de la boucle (ex: 2000 millisecondes = 1 fois toutes les 2 secondes)
                   Thread.Sleep(2000);
               }
           }
       }*/


        }


        private void MouseClick(object sender, MouseButtonEventArgs e)
        {
            // Récupère la position du clic et la stocke
            Point clickPosition = e.GetPosition(moove_map);

            // Passe la position du clic à la méthode de vérification
            CheckForAttack(clickPosition);
        }

        private void CheckForAttack(Point clickPosition)   //GENERER AVEC VS SEUL
        {

            //---------------------------------------------------------------------------
            //-----------------le dangereux qui se fait attaquer-------------------------
            //---------------------------------------------------------------------------

            // Récupérer la position du Bot Poursuiveur
            double botLeft = Canvas.GetLeft(mushroomAttack);
            double botTop = Canvas.GetTop(mushroomAttack);
            // Calculer le centre du Bot
            double botCenterX = botLeft + (mushroomAttack.Width / 2);
            double botCenterY = botTop + (mushroomAttack.Height / 2);
            // Calculer la distance entre le clic et le centre du Bot
            double deltaX = clickPosition.X - botCenterX;
            double deltaY = clickPosition.Y - botCenterY;

            double distance = Math.Sqrt((deltaX * deltaX) + (deltaY * deltaY));
            // Vérifier si la distance est dans la tolérance de clic et si le joueur est proche du Bot
            if (distance <= botClickTolerance)
            {
                // Récupérer la position du joueur
                double playerLeft = Canvas.GetLeft(Player);
                double playerTop = Canvas.GetTop(Player);
                // Calculer la distance entre le joueur et le Bot
                double deltaX_Player = playerLeft - botLeft;
                double deltaY_Player = playerTop - botTop;
                double distanceToPlayer = Math.Sqrt((deltaX_Player * deltaX_Player) + (deltaY_Player * deltaY_Player));
                // Vérifier si le joueur est proche du Bot
                if (distanceToPlayer <= proximityRange)
                {
                    // Le Bot est attaqué
                    moove_map.Children.Remove(mushroomAttack);
                }
            }

            //---------------------------------------------------------------------------
            //-----------------le fuyard qui se fait attaquer----------------------------
            //---------------------------------------------------------------------------
            double fuyardLeft = Canvas.GetLeft(mushroomRun);
            double fuyardTop = Canvas.GetTop(mushroomRun);
            // Calculer le centre du Bot
            double fuyardCenterX = fuyardLeft + (mushroomRun.Width / 2);
            double fuyardCenterY = fuyardTop + (mushroomRun.Height / 2);
            // Calculer la distance entre le clic et le centre du Bot
            double deltaFX = clickPosition.X - fuyardCenterX;
            double deltaFY = clickPosition.Y - fuyardCenterY;

            double distanceF = Math.Sqrt((deltaFX * deltaFX) + (deltaFY * deltaFY));
            // Vérifier si la distance est dans la tolérance de clic et si le joueur est proche du Bot
            if (distance <= botClickTolerance)
            {
                // Récupérer la position du joueur
                double playerLeft = Canvas.GetLeft(Player);
                double playerTop = Canvas.GetTop(Player);
                // Calculer la distance entre le joueur et le Bot
                double deltaX_Player = playerLeft - fuyardLeft;
                double deltaY_Player = playerTop - fuyardTop;
                double distanceToPlayer = Math.Sqrt((deltaX_Player * deltaX_Player) + (deltaY_Player * deltaY_Player));
                // Vérifier si le joueur est proche du Bot
                if (distanceToPlayer <= proximityRange)
                {
                    // Le Bot est attaqué
                    moove_map.Children.Remove(mushroomRun);
                }
            }
        }
    }
}