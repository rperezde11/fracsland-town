using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

public class UPC  : EditorWindow
{
    private List<FolderPath> folderPaths = new List<FolderPath>();

    /// <summary>
    /// Get all the folder paths inside of the project
    /// </summary>
    public void GetFolderPaths()
    {
        folderPaths.Clear();

        string[] tmpAssets1 = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
        string[] tmpAssets2 = System.Array.FindAll(tmpAssets1, name => !name.EndsWith(".meta"));
        string[] allAssets;

        allAssets = System.Array.FindAll(tmpAssets2, name => !name.EndsWith(".unity"));

        for (int i = 0; i < allAssets.Length; i++)
        {
            System.IO.DirectoryInfo p = System.IO.Directory.GetParent(allAssets[i]);

            if (!p.FullName.Contains("Resources") && !p.FullName.Contains("Plugins") && !p.FullName.Contains("Editor") && !p.FullName.Contains("Temp") && !p.FullName.Contains("Standard Assets"))
            {
                AddToFolderPath(p.FullName);
            }
        }
    }

    /// <summary>
    /// Add a folder path to the folder path list
    /// </summary>
    /// <param name="path"></param>
    private void AddToFolderPath(string path)
    {
        if (folderPaths.Count > 0)
        {
            bool addToPath = true;
            for (int i = 0; i < folderPaths.Count; i++)
            {
                if (folderPaths[i].path == path)
                {
                    addToPath = false;
                    break;
                }
            }

            if (addToPath)
                folderPaths.Add(new FolderPath(path));
        }
        else
        {
            folderPaths.Add(new FolderPath(path));
        }
    }

    /// <summary>
    /// Return the folder paths created
    /// </summary>
    /// <returns></returns>
    public List<FolderPath> ReturnFolderPaths()
    {
        return folderPaths;
    }

    /// <summary>
    /// Get all the objects that are inside of the resources folder
    /// </summary>
    /// <returns></returns>
    public string[] GetAllResourceObjects()
    {
        string[] tmpAssets1 = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
        string[] tmpAssets2 = System.Array.FindAll(tmpAssets1, name => !name.EndsWith(".meta"));
        string[] allAssets;
        List<string> listAssets = new List<string>();

        allAssets = System.Array.FindAll(tmpAssets2, name => !name.EndsWith(".unity"));

        for (int i = 0; i < allAssets.Length; i++)
        {
            System.IO.DirectoryInfo p = System.IO.Directory.GetParent(allAssets[i]);

            if (p.FullName.Contains("Resources"))
            {
                allAssets[i] = allAssets[i].Substring(allAssets[i].IndexOf("/Assets") + 1);
                allAssets[i] = allAssets[i].Replace(@"\", "/");

                listAssets.Add(allAssets[i]);
            }
        }

        allAssets = new string[listAssets.Count];
        for (int i = 0; i < listAssets.Count; i++)
        {
            allAssets[i] = listAssets[i];
        }

        return allAssets;
    }
    
    /// <summary>
    /// Return an object from a specfic file path
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public Object GetObject(string path)
    {
        Object objToFind = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
        if(objToFind != null)
            return objToFind;

        return null;
    }

    /// <summary>
    /// Get all the scene files in the project
    /// </summary>
    /// <returns></returns>
    public string[] GetAllScenes()
    {
        string[] tmpAssets1 = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
        string[] allAssets;

        allAssets = System.Array.FindAll(tmpAssets1, name => name.EndsWith(".unity"));

        for (int i = 0; i < allAssets.Length; i++)
        {
            allAssets[i] = allAssets[i].Substring(allAssets[i].IndexOf("/Assets") + 1);
            allAssets[i] = allAssets[i].Replace(@"\", "/");
        }

        return allAssets;
    }

    public string[] GetAllAssets(bool checkFolderOnce)
    {
        string[] tmpAssets1 = Directory.GetFiles(Application.dataPath, "*.*", SearchOption.AllDirectories);
        string[] tmpAssets2 = System.Array.FindAll(tmpAssets1, name => !name.EndsWith(".meta"));
        string[] allAssets;
        List<string> listAssets = new List<string>();

        allAssets = System.Array.FindAll(tmpAssets2, name => !name.EndsWith(".unity"));

        for (int i = 0; i < allAssets.Length; i++)
        {
            System.IO.DirectoryInfo p = System.IO.Directory.GetParent(allAssets[i]);

            if (!p.FullName.Contains("Resources") && !p.FullName.Contains("Plugins") && !p.FullName.Contains("Editor") && !p.FullName.Contains("Temp") && !p.FullName.Contains("Standard Assets"))
            {
                bool use = false;
                // use for find reference to only check that the is being used
                if (checkFolderOnce)
                {
                    for (int r = 0; r < folderPaths.Count; r++)
                    {
                        if (folderPaths[r].path == p.FullName)
                        {
                            if (folderPaths[r].isUsed)
                            {
                                use = true;
                                break;
                            }
                        }
                    }
                }

                if (use || !checkFolderOnce)
                {
                    allAssets[i] = allAssets[i].Substring(allAssets[i].IndexOf("/Assets") + 1);
                    allAssets[i] = allAssets[i].Replace(@"\", "/");

                    listAssets.Add(allAssets[i]);
                }
            }
        }

        allAssets = new string[listAssets.Count];
        for (int i = 0; i < listAssets.Count; i++)
        {
            allAssets[i] = listAssets[i];
        }

        return allAssets;
    }
}
