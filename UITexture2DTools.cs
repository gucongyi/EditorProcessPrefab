using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class UITexture2DTools
{
    [MenuItem("Assets/计算文件夹下图片资源大小", false)]
    public static void CalcFolderTexturesTotalSize()
    {
        string folderPath = GetFolderSelect();
        long totalSize = 0;
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            Debug.LogError("Please select a folder!");
            return;
        }
        string[] assetGUIDs = AssetDatabase.FindAssets("t:texture2D", new[] { folderPath });
        Texture2D[] assets = new Texture2D[assetGUIDs.Length];
        for (int i = 0; i < assetGUIDs.Length; i++)
        {
            assets[i] = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(assetGUIDs[i]));
            long sizeTex= GetTextureFileSize(assets[i]);
            totalSize += sizeTex;
        }
        string formatSize = EditorUtility.FormatBytes(totalSize);
        Debug.LogError($"========文件夹:{folderPath} 总压缩大小:{formatSize}");
    }

    public static string  GetFolderSelect()
    {
        Type projectWindowUtilType = typeof(ProjectWindowUtil);
        MethodInfo getActiveFolderPath = projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
        object obj = getActiveFolderPath.Invoke(null, new object[0]);
        string pathToCurrentFolder = obj.ToString();
        //Debug.Log(pathToCurrentFolder);
        return pathToCurrentFolder;
    }
    public static long GetTextureRuntimeMemorySize(Texture2D texture)
    {
        long memorySize = 0;

        // 使用反射获取UnityEditor.TextureUtil类的Type
        Type textureUtilType = typeof(TextureImporter).Assembly.GetType("UnityEditor.TextureUtil");

        // 使用反射获取UnityEditor.TextureUtil类的GetRuntimeMemorySizeLong方法
        MethodInfo getRuntimeMemorySizeLongMethod = textureUtilType.GetMethod("GetRuntimeMemorySizeLong", BindingFlags.Static | BindingFlags.Public);

        // 调用GetRuntimeMemorySizeLong方法获取运行时内存大小
        memorySize = (long)getRuntimeMemorySizeLongMethod.Invoke(null, new object[] { texture });

        return memorySize;
    }

    public static long GetTextureFileSize(Texture2D texture)
    {
        long fileSize = 0;

        // 使用反射获取UnityEditor.TextureUtil类的Type
        Type textureUtilType = typeof(TextureImporter).Assembly.GetType("UnityEditor.TextureUtil");

        // 使用反射获取UnityEditor.TextureUtil类的GetStorageMemorySizeLong方法
        MethodInfo getStorageMemorySizeLongMethod = textureUtilType.GetMethod("GetStorageMemorySizeLong", BindingFlags.Static | BindingFlags.Public);

        // 调用GetStorageMemorySizeLong方法获取存储内存大小
        fileSize = (long)getStorageMemorySizeLongMethod.Invoke(null, new object[] { texture });

        return fileSize;
    }


}


