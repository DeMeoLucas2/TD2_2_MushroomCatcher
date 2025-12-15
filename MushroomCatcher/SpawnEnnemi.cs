using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MushroomCatcher
{
    // Définition de la classe pour un spawner à ennemi
    public class EnnemiSpawner
    {

        public List<Ennemi> ActiveEnemies = new List<Ennemi>(); // Liste de stokage de tous les ennemis actifs dans le jeu

        private (int X, int Y) spawnLocation = (50, 30); // Position de spawn

        // Sert à gérer le délai entre les spawns
        private float timeSinceLastSpawn = 0f;
        private const float SpawnInterval = 3.0f; // Apparition toutes les 3 secondes
        private string emotion;

        public EnnemiSpawner(string emotion)
        {
            
            this.emotion = emotion;
        }


        public void Update(float deltaTime,Canvas canva)
        {
            // Mise à jour du temps écoulé
            timeSinceLastSpawn += deltaTime;

            // Vérification si l'intervalle est atteint
            if (timeSinceLastSpawn >= SpawnInterval)
            {
                // Réinitialisation du compteur de temps
                timeSinceLastSpawn = 0f;

                // Lancement de la fonction de création
                SpawnNewEnemy(canva);
            }
        }

        private void SpawnNewEnemy(Canvas canva)
        {
            // Crée une nouvelle instance de l'ennemi
            Ennemi newEnemy = new Ennemi(spawnLocation.X, spawnLocation.Y, emotion);

            // Ajoute l'ennemi à la liste des ennemis actifs
            ActiveEnemies.Add(newEnemy);
            canva.Children.Add(newEnemy.Image);
        }
    } 
}
