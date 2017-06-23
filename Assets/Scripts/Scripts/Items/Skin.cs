using Utils;
using UnityEngine;

namespace Items
{
    /// <summary>
    /// This items are used to personalize the aspect of our player.
    /// </summary>
    public class Skin : Item
    {
        public static Item skin1 = new Skin(Constants.ID_SKIN_1, "Skin 1", false, "skin1", 20 * Constants.DIAMOND_GOLD_EQUIVALENCE, 20, "NPC_MAN_01");
        public static Item skin2 = new Skin(Constants.ID_SKIN_2, "Skin 2", false, "skin2", 20 * Constants.DIAMOND_GOLD_EQUIVALENCE, 20, "NPC_MAN_02");
        public static Item skin3 = new Skin(Constants.ID_SKIN_3, "Skin 3", false, "skin3", 20 * Constants.DIAMOND_GOLD_EQUIVALENCE, 20, "NPC_MAN_03");
        public static Item skin4 = new Skin(Constants.ID_SKIN_4, "Skin 4", false, "skin4", 20 * Constants.DIAMOND_GOLD_EQUIVALENCE, 20, "NPC_MAN_04");
        public static Item skin5 = new Skin(Constants.ID_SKIN_5, "Skin 5", false, "skin5", 20 * Constants.DIAMOND_GOLD_EQUIVALENCE, 20, "NPC_MAN_05");
        public static Item skin6 = new Skin(Constants.ID_SKIN_6, "Skin 6", false, "skin6", 20 * Constants.DIAMOND_GOLD_EQUIVALENCE, 20, "NPC_MAN_06");
        public static Item skin7 = new Skin(Constants.ID_SKIN_7, "Skin 7", false, "skin7", 20 * Constants.DIAMOND_GOLD_EQUIVALENCE, 20, "NPC_MAN_07");
        public static Item skin8 = new Skin(Constants.ID_SKIN_8, "Skin 8", false, "skin8", 20 * Constants.DIAMOND_GOLD_EQUIVALENCE, 20, "NPC_MAN_08");
        public static Item skin9 = new Skin(Constants.ID_SKIN_9, "Skin 9", false, "skin9", 20 * Constants.DIAMOND_GOLD_EQUIVALENCE, 20, "NPC_MAN_09");
        public static Item skin10 = new Skin(Constants.ID_SKIN_10, "Skin 10", false, "skin10", 20 * Constants.DIAMOND_GOLD_EQUIVALENCE, 20, "NPC_MAN_10");
        public static Item skin11 = new Skin(Constants.ID_SKIN_11, "Skin 11", false, "skin11", 20 * Constants.DIAMOND_GOLD_EQUIVALENCE, 20, "NPC_MAN_11");
        public static Item skin12 = new Skin(Constants.ID_SKIN_12, "Skin 12", false, "skin12", 20 * Constants.DIAMOND_GOLD_EQUIVALENCE, 20, "NPC_MAN_12");
        public static Item skin13 = new Skin(Constants.ID_SKIN_13, "Skin 13", false, "skin13", 20 * Constants.DIAMOND_GOLD_EQUIVALENCE, 20, "NPC_MAN_13");

        private Texture _texture;
        public Texture Texture { get { return _texture; } }

        public Skin(int id, string name, bool consumible, string image, int goldPrice, int diamondsPrice, string textureName) : base(id, name, consumible, image, goldPrice, diamondsPrice)
        {
            this._texture = (Texture)Resources.Load("Media/Images/" + textureName, typeof(Texture));
        }
    }

}
