using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CustomArguments
{
    public static string buildpath = "buildpath";
    public static string keypath = "keypath";
    public static string keyalias = "keyalias";
    public static string version = "version";
    public static string filename = "filename";
    public static string keypass = "keypass";
}

public class CustomCommandLineArguments
{
    private static List<string> Keys => new List<string>
    {
        CustomArguments.buildpath,
        CustomArguments.keypath,
        CustomArguments.keyalias,
        CustomArguments.version,
        CustomArguments.filename,
        CustomArguments.keypass,
    };
    private static Dictionary<string, string> Args = new Dictionary<string, string>();
    private static bool _isParsed;
    private static string DebugKey => "[CMD_ARGS]";

    public static void Parse(string[] commandLineArgs)
    {
        for (var i = 0; i < commandLineArgs.Length; i++)
        {
            if (Keys.Contains(commandLineArgs[i]))
            {
                if (i + 1 > commandLineArgs.Length)
                    Debug.Log($"{DebugKey} Cant parse argument {commandLineArgs[i]}; Seems no value given");
                Args.Add(commandLineArgs[i], commandLineArgs[i + 1]);
            }
        }

        _isParsed = true;
    }

    public static string GetValue(string key)
    {
        if (!IsParsed()) return string.Empty;
        return Args.ContainsKey(key) ? Args[key] : string.Empty;
    }

    public static void Log()
    {
        if (!IsParsed()) return;
        foreach (var keyValuePair in Args)
        {
            Debug.Log($"{DebugKey} {keyValuePair.Key}:{keyValuePair.Value}");
        }
    }

    private static bool IsParsed()
    {
        if (_isParsed) return true;
        Debug.Log($"{DebugKey} ParseArguments first; Call CustomCommandLineArguments.Parse(string[] args)");
        return false;
    }

}

class AutoBuild
{
    static string[] SCENES = FindEnabledEditorScenes();

    static string ANDROID_SDK = "/Users/developer/Library/Android/sdk";
    private static BuildOptions _options;



    //rules
    //last args is password to alias and keystore
    //last-1 path to builds folder
    //last-2 path to keystore
    [MenuItem("Quality Of Life/Build/AABPipelineOnly")]
    public static void BuildAABViaPipeLine()
    {
        LogArguments();
        BuildSettings();
        LoadAdsAdress();
        CustomCommandLineArguments.Parse(Environment.GetCommandLineArgs());
        CustomCommandLineArguments.Log();

        PlayerSettings.Android.keystoreName = CustomCommandLineArguments.GetValue(CustomArguments.keypath);
        PlayerSettings.Android.keystorePass = CustomCommandLineArguments.GetValue(CustomArguments.keypass);
        PlayerSettings.Android.keyaliasName = CustomCommandLineArguments.GetValue(CustomArguments.keyalias);
        PlayerSettings.Android.keyaliasPass = CustomCommandLineArguments.GetValue(CustomArguments.keypass);

        EditorPrefs.SetString("AndroidSdkRoot", Environment.GetEnvironmentVariable(ANDROID_SDK));

        PlayerSettings.Android.bundleVersionCode = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.All;
        PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)30;

        PlayerSettings.bundleVersion = CustomCommandLineArguments.GetValue(CustomArguments.version);

        //_options = BuildOptions.AllowDebugging | BuildOptions.CompressWithLz4HC;
        var _options = BuildOptions.CompressWithLz4HC;
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = SCENES,
            locationPathName = CustomCommandLineArguments.GetValue(CustomArguments.buildpath) +
                               $"{CustomCommandLineArguments.GetValue(CustomArguments.filename)}.aab",
            target = BuildTarget.Android,
            options = _options,
        };

        EditorUserBuildSettings.buildAppBundle = true;
        Debug.Log(BuildPipeline.BuildPlayer(buildPlayerOptions));
    }

    private static void LoadAdsAdress()
    {
        //var so = (GoogleMobileAds.Editor.GoogleMobileAdsSettings)AssetDatabase.LoadAssetAtPath("Assets/GoogleMobileAds/Resources/GoogleMobileAdsSettings.asset", typeof(GoogleMobileAds.Editor.GoogleMobileAdsSettings));
        //so.GoogleMobileAdsAndroidAppId = "ca-app-pub-4848661661926858~8637235124";
    }

    public static void BuildViaPipeLine()
    {
        LogArguments();
        BuildSettings();
        LoadAdsAdress();
        CustomCommandLineArguments.Parse(Environment.GetCommandLineArgs());
        CustomCommandLineArguments.Log();


        PlayerSettings.Android.keystoreName = CustomCommandLineArguments.GetValue(CustomArguments.keypath);
        PlayerSettings.Android.keystorePass = CustomCommandLineArguments.GetValue(CustomArguments.keypass);
        PlayerSettings.Android.keyaliasName = CustomCommandLineArguments.GetValue(CustomArguments.keyalias);
        PlayerSettings.Android.keyaliasPass = CustomCommandLineArguments.GetValue(CustomArguments.keypass);

        EditorPrefs.SetString("AndroidSdkRoot", Environment.GetEnvironmentVariable(ANDROID_SDK));

        PlayerSettings.Android.bundleVersionCode = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.All;
        PlayerSettings.Android.targetSdkVersion = (AndroidSdkVersions)30;

        PlayerSettings.bundleVersion = CustomCommandLineArguments.GetValue(CustomArguments.version);
        var _options = BuildOptions.AllowDebugging;// | BuildOptions.CompressWithLz4HC;
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = SCENES,
            locationPathName = CustomCommandLineArguments.GetValue(CustomArguments.buildpath) +
                               $"{CustomCommandLineArguments.GetValue(CustomArguments.filename)}.apk",
            target = BuildTarget.Android,
            options = _options,
        };

        EditorUserBuildSettings.buildAppBundle = false;
        Debug.Log(BuildPipeline.BuildPlayer(buildPlayerOptions));
    }

    private static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled) continue;
            EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }

    static void BuildSettings()
    {
        PlayerSettings.SplashScreen.show = false;

    }

    static void LogArguments()
    {
        Debug.Log("#################BuildAndroid#################");

        foreach (string VARIABLE in System.Environment.GetCommandLineArgs())
        {
            Debug.Log($"#################{VARIABLE}#################");
        }
    }
}