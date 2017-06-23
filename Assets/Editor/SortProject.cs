using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class SortProject : UPC
{
    [MenuItem("Window/Unity Project Cleaner/File Structure")]
    static void Init()
    {
        SortProject window = (SortProject)EditorWindow.GetWindow(typeof(SortProject));
        window.Show();
    }

    private bool areYouSure;
    private string root = Application.dataPath;

    void OnGUI()
    {
        if (areYouSure)
        {
            GUILayout.Label("Are you sure you want to sort your project, this cannot be undone?");
            if (GUILayout.Button("Yes"))
            {
                areYouSure = false;
                CreateFolderStructure();
            }
            if (GUILayout.Button("No"))
            {
                areYouSure = false;
            }
        }
        else
        {
            if (GUILayout.Button("Sort Project"))
            {
                areYouSure = true;
            }
            if (GUILayout.Button("Delete Empty Folders"))
            {
                DeleteEmptyFolders();
            }
        }
    }

    /// <summary>
    /// Scan through the directories and flag/remove any that aren't being used
    /// </summary>
    /// <param name="dir"></param>
    /// <param name="stepBack"></param>
    private void Scan(string dir, bool stepBack)
    {
        //directory not empty
        if (Directory.GetFileSystemEntries(dir).Length > 0)
        {
            if (!stepBack)
            {
                foreach (string subdir in Directory.GetDirectories(dir))
                    Scan(subdir, false);
            }
        }
        //directory empty so delete it.
        else
        {
            Directory.Delete(dir);
            string prevDir = dir.Substring(0, dir.LastIndexOf("\\"));
            if (root.Length <= prevDir.Length)
                Scan(prevDir, true);
        }
    }

    /// <summary>
    /// Delete any unused directories
    /// </summary>
    private void DeleteEmptyFolders()
    {
        Scan(Application.dataPath, false);
    } 

    /// <summary>
    /// Create the basic folder structure to hold scripts, models, texture, etc.
    /// </summary>
    private void CreateFolderStructure()
    {
        #region Scripts Folders
        CreateFolder("Assets", "Scripts");

        CreateFolder("Assets/Scripts", "C#");
        CreateFolder("Assets/Scripts", "JS");
        CreateFolder("Assets/Scripts", "Boo");
        CreateFolder("Assets/Scripts", "Shaders");
        #endregion

        #region Animation Folders
        CreateFolder("Assets", "Animations");

        CreateFolder("Assets/Animations", "Clips");
        CreateFolder("Assets/Animations", "Controllers");
        CreateFolder("Assets/Animations", "AvatarMask");
        #endregion

        #region Audio Folders
        CreateFolder("Assets", "Audio");

        CreateFolder("Assets/Audio", "Music");
        CreateFolder("Assets/Audio", "SFX");
        CreateFolder("Assets/Audio", "Videos");
        #endregion

        #region Texture Folders
        CreateFolder("Assets", "Textures");

        CreateFolder("Assets/Textures", "Textures");
        CreateFolder("Assets/Textures", "Materials");
        CreateFolder("Assets/Textures", "Physics Materials");
        CreateFolder("Assets/Textures", "Cubemaps");
        CreateFolder("Assets/Textures", "Render Textures");
        CreateFolder("Assets/Textures", "Sprites");
        CreateFolder("Assets/Textures", "GUI Skins");
        CreateFolder("Assets/Textures", "Models");
        CreateFolder("Assets/Textures", "Prefabs");
        CreateFolder("Assets/Textures", "Fonts");
        CreateFolder("Assets/Textures", "Flares");
        CreateFolder("Assets/Textures", "Render Textures");
        CreateFolder("Assets/Textures", "Lightmaps");
        #endregion

        #region Scenes Folder
        CreateFolder("Assets", "Scenes");
        #endregion


        #region Misc Folder
        CreateFolder("Assets", "Misc");
        #endregion

        SortObject();
    }

    /// <summary>
    /// Create a folder at a specfic path and name
    /// </summary>
    /// <param name="path"></param>
    /// <param name="name"></param>
    private void CreateFolder(string path, string name)
    {
        if (System.IO.Directory.Exists(path + "/" + name))
        {
            //Debug.Log("Folder Exists");
        }
        else
        {
            //Debug.Log("Creating Temp Folder...");
            AssetDatabase.CreateFolder(path, name);
        }
    }

    /// <summary>
    /// Sort the object found by the type they are assigned
    /// </summary>
    private void SortObject()
    {
        string[] allAssets = GetAllAssets(false);

        List<ProjectAsset> pa = new List<ProjectAsset>();

        for (int i = 0; i < allAssets.Length; i++)
        {
            EditorUtility.DisplayProgressBar(
                    "Sorting Assets..." + i + "/" + allAssets.Length + " Complete",
                    "Sorting...",
                    (float)i / (float)allAssets.Length);


            if (GetObject(allAssets[i]) != null)
                pa.Add(new ProjectAsset(GetObject(allAssets[i])));
        }

        for (int i = 0; i < pa.Count; i++)
        {
            EditorUtility.DisplayProgressBar(
                    "Sorting Assets..." + i + "/" + pa.Count + " Complete",
                    "Sorting...",
                    (float)i / (float)pa.Count);

            string oldPath = AssetDatabase.GetAssetPath(pa[i].obj);
            string result = Path.GetFileName(oldPath);

            string p = Path.GetExtension(oldPath);

            switch (p)
            {
                case ".shader":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Scripts/Shaders/" + result);
                    break;
                case ".cs":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Scripts/C#/" + result);
                    break;



                case ".mov":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Audio/Videos/" + result);
                    break;
                case ".avi":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Audio/Videos/" + result);
                    break;
                case ".mpeg4":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Audio/Videos/" + result);
                    break;
                case ".mpg":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Audio/Videos/" + result);
                    break;
                case ".ogg":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Audio/SFX/" + result);
                    break;
                case ".mp3":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Audio/Music/" + result);
                    break;
                case ".wav":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Audio/SFX/" + result);
                    break;
                case ".aiff":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Audio/SFX/" + result);
                    break;




                case ".png":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Textures/" + result);
                    break;
                case ".jpg":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Textures/" + result);
                    break;
                case ".jpeg":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Textures/" + result);
                    break;
                case ".psd":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Textures/" + result);
                    break;
                case ".tif":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Textures/" + result);
                    break;
                case ".tiff":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Textures/" + result);
                    break;
                case ".gif":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Textures/" + result);
                    break;
                case ".bmp":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Textures/" + result);
                    break;
                case ".tga":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Textures/" + result);
                    break;
                case ".iff":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Textures/" + result);
                    break;
                case ".pict":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Textures/" + result);
                    break;





                case ".ma":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Models/" + result);
                    break;
                case ".mb":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Models/" + result);
                    break;
                case ".jas":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Models/" + result);
                    break;
                case ".c4d":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Models/" + result);
                    break;
                case ".blend":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Models/" + result);
                    break;
                case ".fbx":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Models/" + result);
                    break;
                case ".3ds":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Models/" + result);
                    break;
                case ".obj":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Models/" + result);
                    break;
                case ".dxf":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Models/" + result);
                    break;
                case ".coll":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Models/" + result);
                    break;





                case ".mat":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Materials/" + result);
                    break;
                case ".prefab":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Prefabs/" + result);
                    break;
                case ".physicMaterial":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Physics Materials/" + result);
                    break;
                case ".PhysicMaterial":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Physics Materials/" + result);
                    break;
                case ".anim":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Animations/Clips/" + result);
                    break;
                case ".controller":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Animations/Controllers/" + result);
                    break;
                case ".mask":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Animations/AvatarMask/" + result);
                    break;
                case ".boo":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Scripts/Boo/" + result);
                    break;
                case ".cubemap":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Cubemaps/" + result);
                    break;
                case ".guiskin":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/GUI Skins/" + result);
                    break;
                case ".js":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Scripts/JS/" + result);
                    break;


                case ".asset":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Misc/" + result);
                    break;
                case ".fontsettings":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Fonts/" + result);
                    break;
                case ".overrideController":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Animations/Controllers/" + result);
                    break;
                case ".flare":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Flares/" + result);
                    break;
                case ".physicsMaterial2D":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Physics Materials/" + result);
                    break;
                case ".renderTexture":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Render Textures/" + result);
                    break;
                case ".compute":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Scripts/Shaders/" + result);
                    break;
                case ".exr":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Lightmaps/" + result);
                    break;
                case ".ttf":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Textures/Fonts/" + result);
                    break;
                default:
                    Debug.Log("Unknown file: " + p);
                    AssetDatabase.MoveAsset(oldPath, "Assets/Misc/" + result);
                    break;
            }
        }

        string[] allScenes = GetAllScenes();

        List<ProjectAsset> sc = new List<ProjectAsset>();

        for (int i = 0; i < allScenes.Length; i++)
        {
            if (GetObject(allScenes[i]) != null)
                sc.Add(new ProjectAsset(GetObject(allScenes[i])));
        }

        for (int i = 0; i < sc.Count; i++)
        {
            string oldPath = AssetDatabase.GetAssetPath(sc[i].obj);
            string result = Path.GetFileName(oldPath);

            string p = Path.GetExtension(oldPath);

            switch (p)
            {
                case ".unity":
                    AssetDatabase.MoveAsset(oldPath, "Assets/Scenes/" + result);
                    break;

                default:
                    Debug.Log(p);
                    break;
            }
        }

        EditorUtility.ClearProgressBar();
    }
}
