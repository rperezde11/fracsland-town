using UnityEngine;
using System.Collections;


namespace Utils{
	public class Constants {
        
		////////////////////////////////////////////////////////////////////////////////////////////////////
		//                                    USEFULL VARIABLES                                            /
		////////////////////////////////////////////////////////////////////////////////////////////////////
		public const bool DEVELOPMENT = false;
		public const bool USE_FAKE_API = false;
        public const string PATH_RESOURCES = "Media/UsingPrefabs/";
		public const string PATH_TO_SOUNDS = "Media/Sounds/";


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                    STORED DATA NAMES                                            /
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public const string STORAGE_NUM_FAILS = "num_fails";
		public const string STORAGE_BEGUIN_TIME = "time_began";
		public const string STORAGE_NUM_QUESTS = "num_quests";

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                  REQUEST PASSWORD LINK                                          /
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public const string PASSWORD_REQUEST_LINK_DEVELOPMENT = "http://localhost:8000/password_recovery/";
        public const string PASSWORD_REQUEST_LINK_PRODUCTION = "http://37.187.122.29/password_recovery/";

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                          API                                                    /
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public const string API_PORT = "8000";
		public const string API_PROTOCOL = "http://";
		public const string API_DEVELOPMENT_IP = "127.0.0.1";
        public const string API_PRODUCTION_IP = "ns331527.ip-37-187-122.eu";
        //public const string API_PRODUCTION_IP = "37.187.122.29";

        public const string API_BASE_URL = "/api/v1/";
		public const string API_FORMAT_STATEMENT = "?format=json&";

        public const string API_AUTH_URL = "auth/";
        public const string API_GET_PLAYER_STATS_URL = "player_stats/";
        public const string API_GET_LEVEL1CONFIGURATIONS = "level1config/";
        public const string API_POST_TRY_SOLVE = "try_quest/";
        public const string API_ACTIVE_GAMES = "active_games/";
        public const string API_GET_INVITATIONS = "invitations/";
        public const string API_GET_CONFIG = "game_config/";

        public const string API_GET_INVENTORY = "get_inventory/";
        public const string API_UPDATE_ITEM = "update_item/";
        public const string API_UPDATE_GOLD = "update_gold/";
        public const string API_UPDATE_DIAMONDS = "update_diamonds/";
        public const string API_ADD_EXPERIENCE = "add_experience/";
        public const string API_EQUIP_ITEM = "equip_item/";

        /// <summary>
        /// /returns an IP like this http://37.187.122.29:8000/webservice/v1/playerstats/?format=json&
        /// </summary>
        /// <returns>The APIUR.</returns>
        /// <param name="url">URL.</param>
        public static string getAPIURL(string url, int parameter=int.MinValue){
            string IP_PORT = "";
            if (DEVELOPMENT)
            {
                IP_PORT = API_DEVELOPMENT_IP + ":" + API_PORT;
            }
            else
            {
                IP_PORT = API_PRODUCTION_IP;
            }
            string resourceURL = parameter == int.MinValue ? API_PROTOCOL + IP_PORT + API_BASE_URL + url + API_FORMAT_STATEMENT : API_PROTOCOL + IP_PORT + API_BASE_URL + url + parameter.ToString() + "/" + API_FORMAT_STATEMENT;
            Debug.Log(resourceURL);
            return resourceURL;
        }


		////////////////////////////////////////////////////////////////////////////////////////////////////
		//                                     ERROR MESSAGES                                              /
		////////////////////////////////////////////////////////////////////////////////////////////////////

		public const string LEVEL_ERROR_NO_HERO_SPAM_POINT_MESSAGE = "A level must have at least 1 \"HeroSpamPoint\" to spam the hero!!";


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                  GAME MECHANICS CONSTANTS                                       /
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public const float HERO_INTERACTION_MIN_DISTANCE = 1f;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                  GAME ITEMS IDENTIFIERS                                         /
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public const int ID_WOOD = 0;

        //Weapons & tools
        public const int ID_ATXE_1 = 1;
        public const int ID_ATXE_2 = 2;
        public const int ID_ATXE_3 = 3;
        public const int ID_ATXE_4 = 4;
        public const int ID_ATXE_5 = 5;
        public const int ID_ATXE_6 = 6;

        //Hats
        public const int ID_HAT_1 = 7;
        public const int ID_HAT_2 = 8;
        public const int ID_HAT_3 = 9;
        public const int ID_HAT_4 = 10;
        public const int ID_HAT_5 = 11;
        public const int ID_HAT_6 = 12;
        public const int ID_HAT_7 = 13;

        //Skins
        public const int ID_SKIN_1 = 14;
        public const int ID_SKIN_2 = 15;
        public const int ID_SKIN_3 = 16;
        public const int ID_SKIN_4 = 17;
        public const int ID_SKIN_5 = 18;
        public const int ID_SKIN_6 = 19;
        public const int ID_SKIN_7 = 20;
        public const int ID_SKIN_8 = 21;
        public const int ID_SKIN_9 = 22;
        public const int ID_SKIN_10 = 23;
        public const int ID_SKIN_11 = 24;
        public const int ID_SKIN_12 = 25;
        public const int ID_SKIN_13 = 26;

        public const int ID_CARROT = 27;
        public const int ID_PUMPKIN = 28;
        public const int ID_BLUE_MUSHROOM = 29;
        public const int ID_RED_MUSHROOM = 30;
        public const int ID_STRAW = 31;

        public const int ID_HEALTH_POTION = 32;

        public const int ID_FRACTION = 1000;

        public const int DIAMOND_GOLD_EQUIVALENCE = 1000;

        //If you want to change this value remember to change shared_constants.py (server side)
        public const int PLAYER_EQUIPPED_POSITION_WEAPON = 0;
        public const int PLAYER_EQUIPPED_POSITION_HEAD = 1;
        public const int PLAYER_EQUIPPED_POSITION_SKIN = 2;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                            UTILS                                                /
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static System.Random randomNumberGenerator = new System.Random();
        public const int TO_MIN = 60;


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                    FRACTION_TYPES                                               /
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static int FRACTION_TYPE_PIE = 0;
        public static int FRACTION_TYPE_RECTANGULAR = 1;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                       SCENE NAMES                                               /
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public const string SCENE_NAME_SPLASH_SCENE = "Splash";
        public const string SCENE_NAME_BEACH = "Beach";
        public const string SCENE_NAME_FOREST = "Forest";
        public const string SCENE_NAME_FARM = "Farm";
        public const string SCENE_NAME_TOWN = "Town";
        public const string SCENE_NAME_WIN = "Win";

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                       MISSION_IDS                                               /
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public const int MISSION_ID_BRIDGE = 0;
        public const int MISSION_ID_FARM_RABBIT = 1;
        public const int MISSION_ID_FARM_COW = 2;
        public const int MISSION_ID_FARM_CHICKEN = 3;
        public const int MISSION_ID_FOREST_CAPTAIN = 4;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                         LEVELING                                                /
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public const int LEVELING_MAX_LEVEL = 40;
        public const int LEVELING_LEVEL1_EXPERIENCE = 500;

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //                                     PLACES NAMES                                                /
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public const string PLACE_TOWN = "FRACTOWN";
        public const string PLACE_FOREST = "FROZEN WOODS";
        public const string PLACE_BEACH = "SMALL BEACH";
        public const string PLACE_FARM = "FRACFARM";

    }
}