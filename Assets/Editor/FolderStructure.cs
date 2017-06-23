using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FolderStructure
{
    public enum ExtensionType
    {
        png,
        jpg,
        jpeg,
        tiff,
        gif,
        wav,
        mp3,
        fbx,
        obj,
        mb,
        ma,
        cs,
        js,
        boo,
        shader
    }

    public int id;
    public string folderName = "Folder";
    public bool isExtension;
    public bool hasParent;
    public int extensionT = 0;

    public Rect myRect;
    public List<Rect> childRects = new List<Rect>();
    public List<FolderStructure> childFolders = new List<FolderStructure>();

    // ex - Assets/Textures/Player/Armor/ +folderName
    private List<ExtensionType> extensionTypes = new List<ExtensionType>();

    public FolderStructure() { }
    public FolderStructure(Rect myRect, int id)
    {
        this.myRect = myRect;
        this.id = id;
    }

    public void AddToExtensionType(ExtensionType newType)
    {
        extensionTypes.Add(newType);
    }

    public string[] ReturnExtensionTypes()
    {
        int amt = Enum.GetNames(typeof(ExtensionType)).Length;
        string[] t = new string[amt];

        for (int i = 0; i < amt; i++)
        {
            t[i] = "." + Enum.GetName(typeof(ExtensionType), i);
        }


        return t;
    }

    public void UpdateChildRectPositions()
    {
        for(int i = 0; i < childRects.Count; i++)
        {
            childRects[i] = childFolders[i].myRect;
        }
    }
}
