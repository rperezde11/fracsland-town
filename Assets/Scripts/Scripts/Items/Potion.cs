using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;

namespace Items
{
    /// <summary>
    /// Used to heal the player
    /// </summary>
    class Potion : Item
    {
        public static Potion HealthPotion = new Potion(Constants.ID_HEALTH_POTION, "Health Potion", true, null);

        private int _healthToRestore;
        public int HealthToRestore { get { return _healthToRestore; } }

        public Potion(int id, string name, bool consumible, string image) : base(id, name, image)
        {
            _healthToRestore = 30;
        }

        public void Use()
        {
            LevelController.instance.Hero.RestoreHealth(_healthToRestore);
            JukeBox.instance.LoadAndPlaySound("drink", 1f);
        }
    }

}
