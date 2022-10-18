using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.Compilation;
using UnityEngine; 

public class BuildScriptsAddressables
{
    static bool isListening = false;

    public static void SetPlatformWindows()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
        EditorUserBuildSettings.selectedStandaloneTarget = BuildTarget.StandaloneWindows64;

        BuildAddressables();
    }

    public static void SetPlatformMacOS()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
        EditorUserBuildSettings.selectedStandaloneTarget = BuildTarget.StandaloneOSX;


        BuildAddressables();
    }

    public static void BuildAddressables(object o = null)
    {
        if (EditorApplication.isCompiling)
        {
            Debug.Log("Delaying until compilation is finished...");

            if (!isListening)
                CompilationPipeline.compilationFinished += BuildAddressables;
            isListening = true;
            return;
        }

        if (isListening)
            CompilationPipeline.compilationFinished -= BuildAddressables;

        ////AddressableAssetSettingsDefaultObject.Settings = AddressableAssetSettings.Create();
        //Debug.Log("Building Addressables!!! START PLATFORM: platform: " + Application.platform + " target: " + EditorUserBuildSettings.selectedStandaloneTarget);

        //AddressableAssetSettings.CleanPlayerContent();
        //AddressableAssetSettings.BuildPlayerContent();

        //Debug.Log("Building Addressables!!! DONE");

        UpdateOrBuildPlayerContent(BuildTarget.StandaloneOSX);
    }


    public static void BuildOrUpdatePlayerContent(string contentStatePath)
    {
        bool contentFileExists = File.Exists(contentStatePath);
        if (contentFileExists)
        {
            ContentUpdateScript.BuildContentUpdate(AddressableAssetSettingsDefaultObject.Settings, contentStatePath);
        }
        else
        {
            AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
            AddressableAssetSettings.BuildPlayerContent();
        }
    }

    public static void UpdateOrBuildPlayerContent(BuildTarget buildTarget)
    {
        var strTarget = "";
        AddressableAssetSettingsDefaultObject.Settings.activeProfileId = AddressableAssetSettingsDefaultObject.Settings.profileSettings.GetProfileId("Hosted");
        if(buildTarget == BuildTarget.StandaloneOSX){
            strTarget = "OSX";
        }
        string contentStatePath = "Assets/AddressableAssetsData/" + strTarget + "/addressables_content_state.bin";
        BuildOrUpdatePlayerContent(contentStatePath);
    }
}
