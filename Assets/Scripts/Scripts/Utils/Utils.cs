using Items;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine;

/// <summary>
/// This class is not used but it could be usefull
/// to store that in the HD of the machine about
/// missions and other stuff.
/// </summary>
namespace Utils
{

    public static class Utils
    {
        private static string GetQuestPath(ConfiguredQuest quest)
        {
            return Application.persistentDataPath + "q_" + quest.questConfigurationID.ToString() + ".dat";
        }

        public static void SaveQuestConfiguration(ConfiguredQuest data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(GetQuestPath(data), FileMode.OpenOrCreate);
            bf.Serialize(file, data);
            file.Close();
        }

        public static ConfiguredQuest LoadQuestConfiguration(ConfiguredQuest quest)
        {
            try
            {
                string questPath = GetQuestPath(quest);
                if (File.Exists(questPath))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream file = File.Open(questPath, FileMode.Open);
                    ConfiguredQuest data = (ConfiguredQuest)bf.Deserialize(file);
                    file.Close();
                    return data;
                }
                return null;
            }catch(FileNotFoundException exeption)
            {
                return DataManager.instance.ActualQuest;
            }

        }


        public static int GetStandartRewardForLevel(int level)
        {
            return Random.Range(10, 20) * level;
        }
    }

}
