using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEditor.VersionControl;
using UnityEngine;
public class ApiServer 
{
    public static string ImportingModelName = "Model01";
    
}
public class ModelImporter
{
    public static void SetAddressableGroup(string assetPath, string modelName)
    {
        var groupName = modelName;
        var settings = AddressableAssetSettingsDefaultObject.Settings;
 
        if (settings)
        {
            var group = settings.FindGroup(groupName);
            //Debug.Log("Name of selected group: " + group.name);

            //if (group)settings.RemoveGroup(group);
            if(!group)
                group = settings.CreateGroup(groupName, false, false, true, null, typeof(ContentUpdateGroupSchema), typeof(BundledAssetGroupSchema));
            
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
 
            var e = settings.CreateOrMoveEntry(guid, group, false, false);
            var entriesAdded = new List<AddressableAssetEntry> {e};
 
            group.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, false, true);
            settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entriesAdded, true, false);
            AssetDatabase.SaveAssets();

        }
    }

    public static void ExtractMaterials(string directPath, string modelName)
    {
        
        var info = new DirectoryInfo(directPath + "/" + modelName);
        Debug.Log("Get List file from path : " +info);
        var fileInfo = info.GetFiles();
        Debug.Log(fileInfo.Length);
        if (fileInfo == null || fileInfo.Length ==0) return;
        //if ( fileInfo.Length != 4 && fileInfo.Length != 5 )  return;// == 5 .DS_Store File
        Debug.Log("Extract");
        var assetPath = directPath + "/" + modelName + "/"+modelName+".FBX";
        var destinationPath = directPath + "/" + modelName;
        Debug.Log("assetPath : " + assetPath);
        Debug.Log("destinationPath : " + destinationPath);
        //
        //
        HashSet<string> hashSet = new HashSet<string>();
        IEnumerable<Object> enumerable = from x in AssetDatabase.LoadAllAssetsAtPath(assetPath)
            where x.GetType() == typeof(Material)
            select x; 
        Debug.LogWarning(enumerable.Count());
        foreach (Object item in enumerable)
        {
            string path = System.IO.Path.Combine(destinationPath, item.name) + ".mat";
            Debug.Log("Path:" + path);
            path = AssetDatabase.GenerateUniqueAssetPath(path);
            string value = AssetDatabase.ExtractAsset(item, path);
            if (string.IsNullOrEmpty(value))
            {
                hashSet.Add(assetPath);
            }
        }
 
        foreach (string item2 in hashSet)
        {
            AssetDatabase.WriteImportSettingsIfDirty(item2);
            AssetDatabase.ImportAsset(item2, ImportAssetOptions.ForceUpdate);
        }
    }

    public static void DeleteAllMaterial(string directPath, string modelName)
    {
        var path = directPath + "/" + modelName;
        var info = new DirectoryInfo(path);
        Debug.Log("Get List file from path : " +info);
        var fileInfo = info.GetFiles();
        if (fileInfo == null || fileInfo.Length ==0) return;

        string[] trashFolders = {path};
        var allMaterials = AssetDatabase.FindAssets("t: Material",trashFolders);
        foreach (var guid in allMaterials)
        {
            var pathDelete = AssetDatabase.GUIDToAssetPath(guid);
            if (pathDelete.Contains(".mat"))
            {
                // Debug.Log(pathDelete);
                AssetDatabase.DeleteAsset(pathDelete);
            }
        }
    }
    [MenuItem("NOVA/ImportModel/ImportModel", false, 111)]
    public static void ImportModel()
    {
        var directPath = "Assets/Nova/Models";

        var modelName = ApiServer.ImportingModelName;
        Debug.Log(modelName);

        //Create prefab  
        var path = directPath + "/" + modelName + "/" + modelName+".prefab";
        var model = (GameObject)AssetDatabase.LoadMainAssetAtPath(directPath + "/" + modelName + "/" + modelName+".fbx");
        if( !File.Exists( path ))
        {
            GameObject instance = PrefabUtility.InstantiatePrefab(model) as GameObject;
            var variant = PrefabUtility.SaveAsPrefabAsset(instance, path);
            GameObject.DestroyImmediate(instance);
        }
        
        //Delete All Materials
        // DeleteAllMaterial(directPath, modelName);
        //Extract materials
        ExtractMaterials(directPath, modelName);

        SetAddressableGroup(path, modelName);

    }
}
