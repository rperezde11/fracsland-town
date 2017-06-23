using Utils;

namespace Items
{
    public class EquippableItem : Item
    {
        public static Item hat1 = new EquippableItem(Constants.ID_HAT_1, "Cheap Hat"     , false, "hat1", 2 * Constants.DIAMOND_GOLD_EQUIVALENCE,   1,  0.1f,   10,     5f,  "CheapHat");
        public static Item hat2 = new EquippableItem(Constants.ID_HAT_2, "Hat"           , false, "hat2", 4 * Constants.DIAMOND_GOLD_EQUIVALENCE,   3,  0.3f,   20,     10f, "Hat");
        public static Item hat3 = new EquippableItem(Constants.ID_HAT_3, "Green Hat"     , false, "hat3", 6 * Constants.DIAMOND_GOLD_EQUIVALENCE,   6,  0.5f,   30,     13f, "GreenHat");
        public static Item hat4 = new EquippableItem(Constants.ID_HAT_4, "Elegant Hat"   , false, "hat4", 12 * Constants.DIAMOND_GOLD_EQUIVALENCE,  12,  0.7f,   40,     15f, "ElegantHat");
        public static Item hat5 = new EquippableItem(Constants.ID_HAT_5, "Mr hat"        , false, "hat5", 20 * Constants.DIAMOND_GOLD_EQUIVALENCE,  20,  1f,     60,     17f, "MrHat");
        public static Item hat6 = new EquippableItem(Constants.ID_HAT_6, "Explorer Hat"  , false, "hat6", 35 * Constants.DIAMOND_GOLD_EQUIVALENCE,  35, 1.5f,   90,     20f, "ExplorerHat");
        public static Item hat7 = new EquippableItem(Constants.ID_HAT_7, "Black Hat"     , false, "hat7", 50 * Constants.DIAMOND_GOLD_EQUIVALENCE,  50, 2f,     150,    30f, "BlackHat");

        public static Item axe1 = new EquippableItem(Constants.ID_ATXE_1, "Bronze Axe"        , false, "axe1", 1 * Constants.DIAMOND_GOLD_EQUIVALENCE,   1,  0.1f, 0, 20f  , "axe1");
        public static Item axe2 = new EquippableItem(Constants.ID_ATXE_2, "Bronze Dual Axe"   , false, "axe2", 3 * Constants.DIAMOND_GOLD_EQUIVALENCE,   3,  0.3f, 0, 30f , "axe2");
        public static Item axe3 = new EquippableItem(Constants.ID_ATXE_3, "Metal Axe"         , false, "axe3", 6 * Constants.DIAMOND_GOLD_EQUIVALENCE,   6, 0.1f, 0, 40f  , "axe3");
        public static Item axe4 = new EquippableItem(Constants.ID_ATXE_4, "Metal Dual Axe"    , false, "axe4", 12 * Constants.DIAMOND_GOLD_EQUIVALENCE,  12, 0.3f, 0, 50f , "axe4");
        public static Item axe5 = new EquippableItem(Constants.ID_ATXE_5, "Gold Axe"          , false, "axe5", 24 * Constants.DIAMOND_GOLD_EQUIVALENCE,  24, 0.1f, 0, 60f  , "axe5");
        public static Item axe6 = new EquippableItem(Constants.ID_ATXE_6, "Gold Dual Axe"     , false, "axe6", 48 * Constants.DIAMOND_GOLD_EQUIVALENCE,  48, 0.3f, 0, 80f , "axe6");

        private float _speedBonus;
        public float SpeedBonus { get { return _speedBonus; } }

        private float _healthBonus;
        public float HealthBonus { get { return _healthBonus; } }

        private float _attackBonus;
        public float AttackBonus { get { return _attackBonus; } }

        private string _prefabName;
        public string PrefabName { get { return _prefabName; } }

        public EquippableItem(int id, string name, bool consumible, string image, int goldPrice, int diamondsPrice, float speedBonus, int healthBonus, float attackBonus, string prefabName) : base(id, name, consumible, image, goldPrice, diamondsPrice)
        {
            this._speedBonus = speedBonus;
            this._healthBonus = healthBonus;
            this._attackBonus = attackBonus;
            this._prefabName = prefabName;
        }
    }
}
