using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace MushroomCatcher
{
    // Définition de la classe pour un ennemi
    public class Ennemi
    {
        public string Name { get; private set; }

        public (int X, int Y) Position { get; set; }

        public System.Windows.Controls.Image Image { get;set; }

        public Ennemi(int initialX, int initialY, string emotion)
        {
            this.Name = "AngryMush"; // Nom de l'ennemi par défaut
            this.Position = (initialX, initialY);

            // Initialise l'image de l'ennemi
            string imageEnnemi = $"pack://appliscation:,,,/image_gameplay/mushroom_{emotion}.png";
            Image = new System.Windows.Controls.Image();
            Image.Source = new BitmapImage(new Uri(imageEnnemi));

        }

    }
}
