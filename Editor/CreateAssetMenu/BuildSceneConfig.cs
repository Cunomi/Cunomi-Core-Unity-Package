using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Cunomi.Core.Editor.CreateAssetMenu
{
    [CreateAssetMenu(fileName = "NewBuildSceneConfig", menuName = "Cunomi Core/Build Scene Config", order = 0)]
    public class BuildSceneConfig : ScriptableObject
    {
        [SerializeField] private List<SceneAsset> scenes;
        
        [SerializeField] private Boolean isMainInstance;

        public List<SceneAsset> GetScenes()
        {
            return scenes;
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            var guids = AssetDatabase.FindAssets("t:BuildSceneConfig");

            if (guids.Length > 1)
            {
                var pathsToDelete = new List<string>();
                Debug.LogError("You tried to create another Build Scene Config which is not allowed. The new file will therefore be deleted automatically!");

                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var buildMenuSceneConfig = AssetDatabase.LoadAssetAtPath<BuildSceneConfig>(path);

                    if (buildMenuSceneConfig != null && !buildMenuSceneConfig.isMainInstance)
                    {
                        pathsToDelete.Add(path);
                    }
                }

                foreach (var path in pathsToDelete)
                {
                    EditorApplication.delayCall += () =>
                    {
                        AssetDatabase.DeleteAsset(path);
                        AssetDatabase.SaveAssets();
                    };
                }

                if (guids.Length == pathsToDelete.Count)
                {
                    EditorApplication.delayCall += () =>
                    {
                        AssetDatabase.CreateAsset(CreateInstance<BuildSceneConfig>(), pathsToDelete[0]);
                        AssetDatabase.SaveAssets();
                    };
                }
            }
            else
            {
                isMainInstance = true;
            }
        }
#endif
    }
}
