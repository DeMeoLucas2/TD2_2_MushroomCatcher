using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MushroomCatcher
{
    public class SpawnRepeter
    {
        private static Stopwatch stopwatch;
        private static EnnemiSpawner spawnerSad;
        private static EnnemiSpawner spawnerAngry;

        public static void SpawnRepeat(string[] args)
        {
            EnnemiSpawner spawnerSad = new EnnemiSpawner("sad"); // Crée l'objet Spawner d'ennemi sad
            EnnemiSpawner spawnerAngry = new EnnemiSpawner("angry"); // Crée l'objet Spawner d'ennemi angry

            // Mesure le temps assez précisément 
            stopwatch = new Stopwatch();
            stopwatch.Start();           
        }

        public void Update(Canvas canva)
        {
            // Calcul du DeltaTime (temps écoulé depuis dernier "tour")
            float deltaTime = (float)stopwatch.Elapsed.TotalSeconds;
            stopwatch.Restart(); // Réinitialise timer du prochain tour

            // Mise à jour du spawner
            spawnerAngry.Update(deltaTime,canva);
            spawnerSad.Update(deltaTime,canva);

            // Limite la vitesse de la boucle (ex: 2000 millisecondes = 1 fois toutes les 2 secondes)
            Thread.Sleep(2000);
        }
    }
}