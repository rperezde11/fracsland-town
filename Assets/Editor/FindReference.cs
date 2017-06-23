using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Threading;
public class FindReference : UPC 
{
    public enum SortingOptions
    {
        All,
        Animations,
        Audio,
        Prefabs,
        Models,
        Textures,
        Materials,
        Scripts,
        GUISkins,
        Cubemaps,
        Shaders,
        Misc,
        Fonts,
        Movies,
        Flares
    }

    #region Variables

    private string[] allAssetPaths;                                         // holds all the project files asset paths
    private string[] allResourcePaths;                                      // used to hold all the resources objects(items in the resources folders)
    private List<ProjectAsset> projectAssets = new List<ProjectAsset>();    // used to hold all the assets in the project
    private Object[] sceneAssets;                                           // used to hold all the object in the current scene
    private List<Object> dependiences = new List<Object>();                 // a list of all the dependencies in the scene(scripts, materials, textures, etc)

    private string[] allScenesNames;                                        // all the scenes in the project
    private bool[] scenesToRun;                                             // check boxs for the scenes to use when running the test

    private bool runGetAssetsCheck;                                         // inital check for all assets
    private bool runSceneCheck;                                             // run the scene check to setup the system
    private bool runReferenceCheck;                                         // run the reference check per scene for each object
    private bool runScan;
    private bool getFolderPaths;                                            // used to init all the folder paths in the project that contain assets

    private Vector2 scrollView;                                             // scroll view for the assets

    private bool hideUsedObject;                                            // used to hide/unhide assets that are being used(for a cleaner interface)
    private bool findAllObjects;

    

    private static List<string> directoryToExclude = new List<string>();


    private SortingOptions sortingOption = SortingOptions.All;
    private int sortingOptionIndex;
    private SortingOptions[] sortOptionsText;

    private bool toggleFolderGroup = true;
    private Vector2 folderScrollPosition;
    private Vector2 sceneScrollPosition;
    #endregion

    [MenuItem("Window/Unity Project Cleaner/Find Unused Assets")]
    static void Init()
    {
        FindReference window = (FindReference)EditorWindow.GetWindow(typeof(FindReference));
        window.Show();
    }

    private void SetupDirectories()
    {
        // setup inital directories
        directoryToExclude.Add(Application.dataPath + "/Resources");
        directoryToExclude.Add(Application.dataPath + "/Temp");
        directoryToExclude.Add(Application.dataPath + "/Plugins");
        directoryToExclude.Add(Application.dataPath + "/Editor");
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();

        hideUsedObject = GUILayout.Toggle(hideUsedObject, "Hide Used Assets");
        findAllObjects = GUILayout.Toggle(findAllObjects, "Find Inactive Objects");

        GUILayout.EndHorizontal();
        if (GUILayout.Button("Get Folder Paths"))
        {
            GetFolderPaths();
        }

        if (ReturnFolderPaths().Count > 0)
        {
            toggleFolderGroup = EditorGUILayout.BeginToggleGroup("Folders to Include", toggleFolderGroup);
            folderScrollPosition = GUILayout.BeginScrollView(folderScrollPosition, GUILayout.Height(120));
            for (int i = 0; i < ReturnFolderPaths().Count; i++)
            {
                ReturnFolderPaths()[i].isUsed = GUILayout.Toggle(ReturnFolderPaths()[i].isUsed, ReturnFolderPaths()[i].path);
            }
            GUILayout.EndScrollView();
            EditorGUILayout.EndToggleGroup();

            if (GUILayout.Button("Get All Assets"))
            {
                ResetAssetCheck();
            }
        }

        if (runSceneCheck)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("All Scenes");

            if (GUILayout.Button("Uncheck All"))
            {
                for (int x = 0; x < scenesToRun.Length; x++)
                {
                    scenesToRun[x] = false;
                }
            }
            if (GUILayout.Button("Check All"))
            {
                for (int x = 0; x < scenesToRun.Length; x++)
                {
                    scenesToRun[x] = true;
                }
            }
            GUILayout.EndHorizontal();

            sceneScrollPosition = GUILayout.BeginScrollView(sceneScrollPosition, GUILayout.Height(120));
            for (int i = 0; i < allScenesNames.Length; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                scenesToRun[i] = GUILayout.Toggle(scenesToRun[i], allScenesNames[i]);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Sort by: ");
            sortingOptionIndex = EditorGUILayout.Popup(sortingOptionIndex, ReturnSortingTypes());
            sortingOption = (SortingOptions)sortingOptionIndex;
            GUILayout.EndHorizontal();


            if (GUILayout.Button("Scan"))
            {
                runScan = true;
            }
            
            if (runReferenceCheck)
            {
                if (GUILayout.Button("Move all Trash"))
                {
                    MoveToTrash();
                }
            }

            scrollView = GUILayout.BeginScrollView(scrollView);

            if (hideUsedObject)
            {
                for (int i = 0; i < projectAssets.Count; i++)
                {
                    GUILayout.BeginHorizontal();

                    if (!projectAssets[i].isUsed)
                    {
                        if (sortingOption == SortingOptions.All)
                        {
                            EditorGUILayout.ObjectField(projectAssets[i].obj, typeof(Object), false);
                            projectAssets[i].isUsed = GUILayout.Toggle(projectAssets[i].isUsed, "");
                        }
                        else
                        {
                            if (projectAssets[i].sortingOption == sortingOption)
                            {
                                EditorGUILayout.ObjectField(projectAssets[i].obj, typeof(Object), false);
                                projectAssets[i].isUsed = GUILayout.Toggle(projectAssets[i].isUsed, "");
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                for (int i = 0; i < projectAssets.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    if (sortingOption == SortingOptions.All)
                    {
                        EditorGUILayout.ObjectField(projectAssets[i].obj, typeof(Object), false);
                        projectAssets[i].isUsed = GUILayout.Toggle(projectAssets[i].isUsed, "");
                    }
                    else
                    {
                        if (projectAssets[i].sortingOption == sortingOption)
                        {
                            EditorGUILayout.ObjectField(projectAssets[i].obj, typeof(Object), false);
                            projectAssets[i].isUsed = GUILayout.Toggle(projectAssets[i].isUsed, "");
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
        }
    }

    void Update()
    {
        if (runGetAssetsCheck)
        {
            RunAssetCheck();
        }

        if (runScan)
        {
            RunScan();
        }
    }

    /// <summary>
    /// Scan through all the assets to see if they are being used in the scenes
    /// </summary>
    private void RunScan()
    {
        for (int j = 0; j < scenesToRun.Length; j++)
        {
            if (EditorUtility.DisplayCancelableProgressBar(
                        "Dependiences Scanning... " + j + "/" + scenesToRun.Length + " Complete",
                        "Scanning Dependiences in Scene: " + allScenesNames[j].ToString(),
                        (float)j / (float)scenesToRun.Length))
            {
                runReferenceCheck = true;
                runScan = false;

                EditorUtility.ClearProgressBar();
            }


            if (scenesToRun[j])
            {
                EditorApplication.OpenScene(allScenesNames[j]);


                if (!findAllObjects)
                    sceneAssets = GameObject.FindObjectsOfType(typeof(GameObject)) as GameObject[];
                else
                    sceneAssets = Resources.FindObjectsOfTypeAll<GameObject>();

                Object[] o = EditorUtility.CollectDependencies(sceneAssets);

                for (int i = 0; i < o.Length; i++)
                {
                    dependiences.Add(o[i]);
                }


                for (int i = 0; i < dependiences.Count; i++)
                {
                    for (int x = 0; x < projectAssets.Count; x++)
                    {
                        if (dependiences[i] == projectAssets[x].obj)
                        {
                            projectAssets[x].isUsed = true;
                            break;
                        }
                    }
                }

                System.Type[] typelist = GetTypesInNamespace(Assembly.GetExecutingAssembly(), "Player");
                for (int i = 0; i < typelist.Length; i++)
                {
                    Debug.Log(typelist[i].Name);
                }

            }
        }

        runReferenceCheck = true;
        runScan = false;
        EditorUtility.ClearProgressBar();
    }

    private System.Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
    {
        return assembly.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, System.StringComparison.Ordinal)).ToArray();
    }

    /// <summary>
    /// Run through all the assets in the project and set them up to be ProjectAsset's
    /// </summary>
    private void RunAssetCheck()
    {
        Debug.Log("Start Time: " + System.DateTime.Now);

        // get all the scenes in the projects
        allScenesNames = GetAllScenes();
        scenesToRun = new bool[allScenesNames.Length];
        for (int x = 0; x < scenesToRun.Length; x++)
        {
            scenesToRun[x] = true;
        }

        // get all the assets that are in the project
        allAssetPaths = GetAllAssets(true);
        for (int i = 0; i < allAssetPaths.Length; i++)
        {
            if (GetObject(allAssetPaths[i]) != null)
            {
                projectAssets.Add(new ProjectAsset(GetObject(allAssetPaths[i])));
                projectAssets[projectAssets.Count - 1].SetSortingOption(Path.GetExtension(allAssetPaths[i]));
            }
        }


        allResourcePaths = GetAllResourceObjects();
        Object[] allResObjects = new Object[allResourcePaths.Length];
        for (int i = 0; i < allResourcePaths.Length; i++)
        {
            if (GetObject(allResourcePaths[i]) != null)
            {
                allResObjects[i] = GetObject(allResourcePaths[i]);
                projectAssets.Add(new ProjectAsset(GetObject(allResourcePaths[i])));
                projectAssets[projectAssets.Count - 1].SetSortingOption(Path.GetExtension(allResourcePaths[i]));
            }
        }

        Object[] depend = EditorUtility.CollectDependencies(allResObjects);


        for (int i = 0; i < depend.Length; i++)
        {
            if (EditorUtility.DisplayCancelableProgressBar(
                "Scanning Assets..." + i + "/" + allResourcePaths.Length + " Complete",
                "Scanning All Assets",
                (float)i / (float)depend.Length))
            {
                runSceneCheck = true;
                runGetAssetsCheck = false;
                EditorUtility.ClearProgressBar();
            }

            for (int x = 0; x < projectAssets.Count; x++)
            {
                if (depend[i] == projectAssets[x].obj)
                {
                    projectAssets[x].isUsed = true;
                    break;
                }
            }
        }

        Debug.Log("End Time: " + System.DateTime.Now);
        // flag to be able to scan the scenes once all the assets are loaded
        runSceneCheck = true;
        runGetAssetsCheck = false;
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// Reset everything so we don't carry over anything
    /// </summary>
    private void ResetAssetCheck()
    {
        allAssetPaths = null;
        allResourcePaths = null;
        projectAssets.Clear();
        sceneAssets = null;
        allScenesNames = null;
        scenesToRun = null;
        dependiences.Clear();
        runScan = false;

        runGetAssetsCheck = true;
    }

    /// <summary>
    /// Returns the Names for the sorting option types
    /// </summary>
    /// <returns></returns>
    private string[] ReturnSortingTypes()
    {
        int amt = System.Enum.GetNames(typeof(SortingOptions)).Length;
        string[] t = new string[amt];

        for (int i = 0; i < amt; i++)
        {
            t[i] = System.Enum.GetName(typeof(SortingOptions), i);
        }


        return t;
    }
    
    /// <summary>
    /// Move the items that are not found, into a temp folder
    /// </summary>
    private void MoveToTrash()
    {
        if (System.IO.Directory.Exists("Assets/Temp"))
        {
            Debug.Log("Temp Folder Exists");
        }
        else
        {
            Debug.Log("Creating Temp Folder...");
            AssetDatabase.CreateFolder("Assets", "Temp");
        }

        for (int i = 0; i < projectAssets.Count; i++)
        {
            if(!projectAssets[i].isUsed)
            {
                string oldPath = AssetDatabase.GetAssetPath(projectAssets[i].obj);
                //Debug.Log(oldPath);
                
                string result = Path.GetFileName(oldPath);
                AssetDatabase.MoveAsset(oldPath, "Assets/Temp/" + result);
                //AssetDatabase.MoveAsset(oldPath, "Assets/Temp/" + projectAssets[i].obj.name + Path.GetExtension(oldPath));
                
            }
        }
    }
}

public class ProjectAsset
{
    public ProjectAsset(Object obj)
    {
        this.obj = obj;
        isUsed = false;
    }
    public Object obj;
    public bool isUsed;

    public FindReference.SortingOptions sortingOption;

    public void SetSortingOption(string sortingOption)
    {
        FindReference.SortingOptions newSortingOption = FindReference.SortingOptions.All;

        switch (sortingOption)
        {
            case ".shader":
                newSortingOption = FindReference.SortingOptions.Shaders;
                break;
            case ".cs":
                newSortingOption = FindReference.SortingOptions.Scripts;
                break;



            case ".mov":
                newSortingOption = FindReference.SortingOptions.Movies;
                break;
            case ".avi":
                newSortingOption = FindReference.SortingOptions.Movies;
                break;
            case ".mpeg4":
                newSortingOption = FindReference.SortingOptions.Movies;
                break;
            case ".mpg":
                newSortingOption = FindReference.SortingOptions.Movies;
                break;
            case ".ogg":
                newSortingOption = FindReference.SortingOptions.Audio;
                break;
            case ".mp3":
                newSortingOption = FindReference.SortingOptions.Audio;
                break;
            case ".wav":
                newSortingOption = FindReference.SortingOptions.Audio;
                break;
            case ".aiff":
                newSortingOption = FindReference.SortingOptions.Audio;
                break;




            case ".png":
                newSortingOption = FindReference.SortingOptions.Textures;
                break;
            case ".jpg":
                newSortingOption = FindReference.SortingOptions.Textures;
                break;
            case ".jpeg":
                newSortingOption = FindReference.SortingOptions.Textures;
                break;
            case ".psd":
                newSortingOption = FindReference.SortingOptions.Textures;
                break;
            case ".tiff":
                newSortingOption = FindReference.SortingOptions.Textures;
                break;
            case ".gif":
                newSortingOption = FindReference.SortingOptions.Textures;
                break;
            case ".bmp":
                newSortingOption = FindReference.SortingOptions.Textures;
                break;
            case ".tga":
                newSortingOption = FindReference.SortingOptions.Textures;
                break;
            case ".iff":
                newSortingOption = FindReference.SortingOptions.Textures;
                break;
            case ".pict":
                newSortingOption = FindReference.SortingOptions.Textures;
                break;





            case ".ma":
                newSortingOption = FindReference.SortingOptions.Models;
                break;
            case ".mb":
                newSortingOption = FindReference.SortingOptions.Models;
                break;
            case ".jas":
                newSortingOption = FindReference.SortingOptions.Models;
                break;
            case ".c4d":
                newSortingOption = FindReference.SortingOptions.Models;
                break;
            case ".blend":
                newSortingOption = FindReference.SortingOptions.Models;
                break;
            case ".fbx":
                newSortingOption = FindReference.SortingOptions.Models;
                break;
            case ".3ds":
                newSortingOption = FindReference.SortingOptions.Models;
                break;
            case ".obj":
                newSortingOption = FindReference.SortingOptions.Models;
                break;
            case ".dxf":
                newSortingOption = FindReference.SortingOptions.Models;
                break;
            case ".coll":
                newSortingOption = FindReference.SortingOptions.Models;
                break;





            case ".mat":
                newSortingOption = FindReference.SortingOptions.Materials;
                break;
            case ".prefab":
                newSortingOption = FindReference.SortingOptions.Prefabs;
                break;
            case ".physicMaterial":
                newSortingOption = FindReference.SortingOptions.Materials;
                break;
            case ".PhysicMaterial":
                newSortingOption = FindReference.SortingOptions.Materials;
                break;
            case ".anim":
                newSortingOption = FindReference.SortingOptions.Animations;
                break;
            case ".controller":
                newSortingOption = FindReference.SortingOptions.Animations;
                break;
            case ".mask":
                newSortingOption = FindReference.SortingOptions.Animations;
                break;
            case ".boo":
                newSortingOption = FindReference.SortingOptions.Scripts;
                break;
            case ".cubemap":
                newSortingOption = FindReference.SortingOptions.Cubemaps;
                break;
            case ".guiskin":
                newSortingOption = FindReference.SortingOptions.GUISkins;
                break;
            case ".js":
                newSortingOption = FindReference.SortingOptions.Scripts;
                break;


            case ".asset":
                newSortingOption = FindReference.SortingOptions.Misc;
                break;
            case ".fontsettings":
                newSortingOption = FindReference.SortingOptions.Fonts;
                break;
            case ".overrideController":
                newSortingOption = FindReference.SortingOptions.Animations;
                break;
            case ".flare":
                newSortingOption = FindReference.SortingOptions.Flares;
                break;
            case ".physicsMaterial2D":
                newSortingOption = FindReference.SortingOptions.Materials;
                break;
            case ".renderTexture":
                newSortingOption = FindReference.SortingOptions.Textures;
                break;
            case ".compute":
                newSortingOption = FindReference.SortingOptions.Shaders;
                break;

            default:
                newSortingOption = FindReference.SortingOptions.Misc;
                break;
        }

        this.sortingOption = newSortingOption;
    }
}

public class FolderPath
{
    public string path = "";
    public bool isUsed;

    public FolderPath(string path)
    {
        this.path = path;
        isUsed = true;
    }
}