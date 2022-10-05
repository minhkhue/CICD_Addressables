using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
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

        //AddressableAssetSettingsDefaultObject.Settings = AddressableAssetSettings.Create();
        Debug.Log("Building Addressables!!! START PLATFORM: platform: " + Application.platform + " target: " + EditorUserBuildSettings.selectedStandaloneTarget);

        AddressableAssetSettings.CleanPlayerContent();
        AddressableAssetSettings.BuildPlayerContent();

        Debug.Log("Building Addressables!!! DONE");
    }
}