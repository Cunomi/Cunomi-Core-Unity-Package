using System.Collections.Generic;
using System.IO;
using Cunomi.Core.Editor.CreateAssetMenu;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Cunomi.Core.Editor.Menu
{
    public class BuildMenuItem : MonoBehaviour
    {
        [MenuItem("Cunomi/Build Tools/Build All")]
        public static void BuildAll()
        {
            BuildWindowsBoth();
            BuildLinuxBoth();
            BuildMacOSBoth();
        }
        
        [MenuItem("Cunomi/Build Tools/Clear All")]
        public static void ClearAll()
        {
            ClearWindowsBoth();
            ClearLinuxBoth();
            ClearMacOSBoth();
        }
        
        [MenuItem("Cunomi/Build Tools/Windows/Build Both")]
        public static void BuildWindowsBoth()
        {
            BuildWindowsClient();
            BuildWindowsServer();
        }
        
        [MenuItem("Cunomi/Build Tools/Windows/Clear Both")]
        public static void ClearWindowsBoth()
        {
            ClearDirectory("Builds/Windows/Client");
            ClearDirectory("Builds/Windows/Server");
        }
        
        [MenuItem("Cunomi/Build Tools/Windows/Build Client")]
        public static void BuildWindowsClient()
        {
            Debug.Log("Building Windows Client...");
            ClearDirectory("Builds/Windows/Client");
            var buildReport = BuildPipeline.BuildPlayer(GetBuildPlayerOptions(
                "Builds/Windows/Client/Client.exe", 
                BuildTarget.StandaloneWindows64, 
                StandaloneBuildSubtarget.Player, 
                BuildOptions.CompressWithLz4HC
            ));
            PrintBuildResult(buildReport.summary, "Windows Client");
        }

        [MenuItem("Cunomi/Build Tools/Windows/Build Server")]
        public static void BuildWindowsServer()
        {
            Debug.Log("Building Windows Server...");
            ClearDirectory("Builds/Windows/Server");
            var buildReport = BuildPipeline.BuildPlayer(GetBuildPlayerOptions(
                "Builds/Windows/Server/Server.exe", 
                BuildTarget.StandaloneWindows64, 
                StandaloneBuildSubtarget.Server, 
                BuildOptions.CompressWithLz4HC
            ));
            PrintBuildResult(buildReport.summary, "Windows Server");
        }
        
        [MenuItem("Cunomi/Build Tools/Linux/Build Both")]
        public static void BuildLinuxBoth()
        {
            BuildLinuxClient();
            BuildLinuxServer();
        }
        
        [MenuItem("Cunomi/Build Tools/Linux/Clear Both")]
        public static void ClearLinuxBoth()
        {
            ClearDirectory("Builds/Linux/Client");
            ClearDirectory("Builds/Linux/Server");
        }
        
        [MenuItem("Cunomi/Build Tools/Linux/Build Client")]
        public static void BuildLinuxClient()
        {
            Debug.Log("Building Linux Client...");
            ClearDirectory("Builds/Linux/Client");
            var buildReport = BuildPipeline.BuildPlayer(GetBuildPlayerOptions(
                "Builds/Linux/Client/Client.x86_64", 
                BuildTarget.StandaloneLinux64, 
                StandaloneBuildSubtarget.Player, 
                BuildOptions.CompressWithLz4HC
            ));
            PrintBuildResult(buildReport.summary, "Linux Client");
        }

        [MenuItem("Cunomi/Build Tools/Linux/Build Server")]
        public static void BuildLinuxServer()
        {
            Debug.Log("Building Linux Server...");
            ClearDirectory("Builds/Linux/Server");
            var buildReport = BuildPipeline.BuildPlayer(GetBuildPlayerOptions(
                "Builds/Linux/Server/Server.x86_64", 
                BuildTarget.StandaloneLinux64, 
                StandaloneBuildSubtarget.Server, 
                BuildOptions.CompressWithLz4HC
            ));
            PrintBuildResult(buildReport.summary, "Linux Server");
        }
        
        [MenuItem("Cunomi/Build Tools/MacOS/Build Both")]
        public static void BuildMacOSBoth()
        {
            BuildMacOSClient();
            BuildMacOSServer();
        }
        
        [MenuItem("Cunomi/Build Tools/MacOS/Clear Both")]
        public static void ClearMacOSBoth()
        {
            ClearDirectory("Builds/MacOO/Client");
            ClearDirectory("Builds/MacOS/Server");
        }
        
        [MenuItem("Cunomi/Build Tools/MacOS/Build Client")]
        public static void BuildMacOSClient()
        {
            Debug.Log("Building MacOS Client...");
            ClearDirectory("Builds/MacOS/Client");
            var buildReport = BuildPipeline.BuildPlayer(GetBuildPlayerOptions(
                "Builds/MacOS/Client", 
                BuildTarget.StandaloneOSX, 
                StandaloneBuildSubtarget.Player, 
                BuildOptions.CompressWithLz4HC
            ));
            PrintBuildResult(buildReport.summary, "MacOS Client");
        }

        [MenuItem("Cunomi/Build Tools/MacOS/Build Server")]
        public static void BuildMacOSServer()
        {
            Debug.Log("Building MacOS Server...");
            ClearDirectory("Builds/MacOS/Server");
            var buildReport = BuildPipeline.BuildPlayer(GetBuildPlayerOptions(
                "Builds/MacOS/Server", 
                BuildTarget.StandaloneOSX, 
                StandaloneBuildSubtarget.Server, 
                BuildOptions.CompressWithLz4HC
            ));
            PrintBuildResult(buildReport.summary, "MacOS Server");
        }
        
        private static void ClearDirectory(string directoryToClear)
        {
            var directoryInfo = new DirectoryInfo(directoryToClear);

            if (!directoryInfo.Exists || directoryInfo.GetFiles().Length == 0 && directoryInfo.GetDirectories().Length == 0)
            {
                Debug.Log("There is nothing to clear.");
                return;
            }
            
            Debug.Log($"Clearing directory \"{directoryToClear}\"...");

            foreach (var file in directoryInfo.EnumerateFiles())
            {
                file.Delete();
            }

            foreach (var directory in directoryInfo.EnumerateDirectories())
            {
                directory.Delete(true);
            }
        }

        private static void PrintBuildResult(BuildSummary buildSummary, string targetName)
        {
            switch (buildSummary.result)
            {
                case BuildResult.Succeeded:
                    Debug.Log($"{targetName} has been successfully built.");
                    break;
                case BuildResult.Failed:
                    Debug.LogError($"The build process for {targetName} has failed.");
                    break;
                case BuildResult.Cancelled:
                    Debug.LogError($"The build process for {targetName} was cancelled.");
                    break;
                default:
                    Debug.LogError($"The build result for {targetName} is unknown.");
                    break;
            }
        }

        private static BuildPlayerOptions GetBuildPlayerOptions(
            string locationPathName, 
            BuildTarget buildTarget, 
            StandaloneBuildSubtarget buildSubtarget, 
            BuildOptions buildOptions
        )
        {
            var sceneNames = GetSceneNames();

            if (sceneNames.Length < 1)
            {
                throw new BuildFailedException("Project could not be build without a scene! Please add a scene in the BuildSceneConfig!");
            }
            
            return new BuildPlayerOptions
            {
                scenes = sceneNames,
                locationPathName = locationPathName,
                target = buildTarget,
                subtarget = (int) buildSubtarget,
                options = buildOptions
            };
        }

        private static string[] GetSceneNames()
        {
            var buildSceneConfig = LoadOrCreateBuildSceneConfig();
            var sceneNames = new List<string>();

            if (buildSceneConfig == null)
            {
                return sceneNames.ToArray();
            }

            foreach (var scene in buildSceneConfig.GetScenes())
            {
                if (scene != null)
                {
                    sceneNames.Add(AssetDatabase.GetAssetPath(scene));
                    
                }
            }

            return sceneNames.ToArray();
        }

        private static BuildSceneConfig LoadOrCreateBuildSceneConfig()
        {
            var guids = AssetDatabase.FindAssets("t:BuildSceneConfig");

            if (guids.Length == 0)
            {
                Debug.Log("No BuildSceneConfig found. Creating a new one.");
                
                var newBuildSceneConfig = ScriptableObject.CreateInstance<BuildSceneConfig>();
                AssetDatabase.CreateAsset(newBuildSceneConfig, "Assets/NewBuildSceneConfig.asset");
                AssetDatabase.SaveAssets();
                
                Debug.Log("New BuildSceneConfig created at: Assets/NewBuildSceneConfig.asset");
                return newBuildSceneConfig;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            var buildSceneConfig = AssetDatabase.LoadAssetAtPath<BuildSceneConfig>(path);

            if (buildSceneConfig == null)
            {
                Debug.LogError($"Failed to load BuildSceneConfig at path: {path}");
                return null;
            }

            Debug.Log($"Loaded BuildSceneConfig from path: {path}");
            return buildSceneConfig;
        }
    }
}
