namespace EasySceneManager
{
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using UnityEngine.SceneManagement;

    public class EasySceneLoader : EditorWindow
    {
        // List to store scene categories and their corresponding scenes
        private List<SceneCategory> sceneCategories = new List<SceneCategory>();

        // Vector2 to store the scroll position of the scene list
        private Vector2 scrollPosition;

        // String to store the search query entered by the user
        private string searchQuery = "";

        // Bool to toggle the visibility of preload settings
        private bool showPreloadSettings = false;

        // Bool to toggle the visibility of unload settings
        private bool showUnloadSettings = false;

        // Float to store the delay before preloading scenes (in seconds)
        private float preloadDelay = 1f;

        // Float to store the delay before unloading unused scenes (in seconds)
        private float unloadDelay = 5f;

        // Menu item to show the Easy Scene Loader window
        [MenuItem("Tools/Easy Scene Loader")]
        public static void ShowWindow()
        {
            GetWindow<EasySceneLoader>("Easy Scene Loader");
        }

        // Called when the window is enabled
        private void OnEnable()
        {
            RefreshSceneList();
        }

        // Called to draw the GUI of the Easy Scene Loader window
        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            // Button to refresh the scene list
            if (GUILayout.Button("Refresh Scene List"))
            {
                RefreshSceneList();
            }

            // Text field to enter the search query
            searchQuery = EditorGUILayout.TextField("Search Scenes", searchQuery);

            // Begin scroll view for the scene list
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Iterate through each scene category
            foreach (SceneCategory category in sceneCategories)
            {
                // Skip empty categories
                if (category.scenes.Count == 0)
                    continue;

                // Draw the category name in a help box style
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(category.name, EditorStyles.boldLabel);

                // Iterate through each scene in the category
                foreach (SceneInfo sceneInfo in category.scenes)
                {
                    // Skip scenes that don't match the search query
                    if (!string.IsNullOrEmpty(searchQuery) && !sceneInfo.name.ToLower().Contains(searchQuery.ToLower()))
                        continue;

                    EditorGUILayout.BeginHorizontal();

                    // Button to load the scene in standard mode
                    if (GUILayout.Button(sceneInfo.name))
                    {
                        LoadSceneStandard(sceneInfo.path);
                    }

                    // Button to load the scene additively
                    if (GUILayout.Button("Additive", GUILayout.Width(80)))
                    {
                        LoadSceneAdditive(sceneInfo.path);
                    }

                    // Button to unload the scene if it's loaded additively
                    if (IsSceneLoadedAdditive(sceneInfo.path) && GUILayout.Button("Unload", GUILayout.Width(60)))
                    {
                        UnloadScene(sceneInfo.path);
                    }

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
            }

            // End scroll view for the scene list
            EditorGUILayout.EndScrollView();

            // Foldout for preload settings
            showPreloadSettings = EditorGUILayout.Foldout(showPreloadSettings, "Preload Settings");
            if (showPreloadSettings)
            {
                // Float field to set the preload delay
                preloadDelay = EditorGUILayout.FloatField("Preload Delay (seconds)", preloadDelay);

                // Button to preload all scenes
                if (GUILayout.Button("Preload All Scenes"))
                {
                    PreloadAllScenes();
                }
            }

            // Foldout for unload settings
            showUnloadSettings = EditorGUILayout.Foldout(showUnloadSettings, "Unload Settings");
            if (showUnloadSettings)
            {
                // Float field to set the unload delay
                unloadDelay = EditorGUILayout.FloatField("Unload Delay (seconds)", unloadDelay);

                // Button to unload unused scenes
                if (GUILayout.Button("Unload Unused Scenes"))
                {
                    UnloadUnusedScenes();
                }
            }

            EditorGUILayout.EndVertical();
        }

        // Refreshes the list of scenes and categories
        private void RefreshSceneList()
        {
            sceneCategories.Clear();

            // Get all scene paths in the project
            string[] scenePaths = AssetDatabase.FindAssets("t:Scene")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .ToArray();

            // Iterate through each scene path
            foreach (string scenePath in scenePaths)
            {
                // Get the scene name and category name
                string sceneName = Path.GetFileNameWithoutExtension(scenePath);
                string categoryName = GetSceneCategory(scenePath);

                // Find or create the scene category
                SceneCategory category = sceneCategories.Find(c => c.name == categoryName);
                if (category == null)
                {
                    category = new SceneCategory(categoryName);
                    sceneCategories.Add(category);
                }

                // Add the scene to the category
                category.scenes.Add(new SceneInfo(sceneName, scenePath));
            }

            // Sort categories and scenes alphabetically
            sceneCategories = sceneCategories.OrderBy(c => c.name).ToList();
            foreach (SceneCategory category in sceneCategories)
            {
                category.scenes = category.scenes.OrderBy(s => s.name).ToList();
            }
        }

        // Gets the category name based on the scene path
        private string GetSceneCategory(string scenePath)
        {
            string[] folders = scenePath.Split('/');
            if (folders.Length > 2)
            {
                return folders[folders.Length - 2];
            }
            return "Uncategorized";
        }

        // Loads a scene additively
        private void LoadSceneAdditive(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            }
        }

        // Loads a scene in standard mode
        private void LoadSceneStandard(string scenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            }
        }

        // Unloads a scene
        private void UnloadScene(string scenePath)
        {
            Scene scene = EditorSceneManager.GetSceneByPath(scenePath);
            if (scene.isLoaded)
            {
                EditorSceneManager.CloseScene(scene, true);
            }
        }

        // Checks if a scene is loaded additively
        private bool IsSceneLoadedAdditive(string scenePath)
        {
            Scene scene = EditorSceneManager.GetSceneByPath(scenePath);
            return scene.isLoaded && scene != EditorSceneManager.GetActiveScene();
        }

        // Preloads all scenes additively
        private void PreloadAllScenes()
        {
            foreach (SceneCategory category in sceneCategories)
            {
                foreach (SceneInfo sceneInfo in category.scenes)
                {
                    if (!EditorSceneManager.GetSceneByPath(sceneInfo.path).isLoaded)
                    {
                        EditorSceneManager.LoadSceneInPlayMode(sceneInfo.path, new LoadSceneParameters(LoadSceneMode.Additive));
                    }
                }
            }

            EditorApplication.update += DelayedUnloadUnusedScenes;
        }

        // Initiates the unloading of unused scenes
        private void UnloadUnusedScenes()
        {
            EditorApplication.update += DelayedUnloadUnusedScenes;
        }

        // Unloads unused scenes after a delay
        private void DelayedUnloadUnusedScenes()
        {
            if (EditorApplication.timeSinceStartup >= unloadDelay)
            {
                EditorApplication.update -= DelayedUnloadUnusedScenes;

                for (int i = 0; i < EditorSceneManager.sceneCount; i++)
                {
                    Scene scene = EditorSceneManager.GetSceneAt(i);
                    if (scene != EditorSceneManager.GetActiveScene() && scene.isLoaded)
                    {
                        EditorSceneManager.CloseScene(scene, true);
                    }
                }
            }
        }

        // Class to represent a scene category
        private class SceneCategory
        {
            public string name;
            public List<SceneInfo> scenes;

            public SceneCategory(string name)
            {
                this.name = name;
                scenes = new List<SceneInfo>();
            }
        }

        // Class to represent a scene
        private class SceneInfo
        {
            public string name;
            public string path;

            public SceneInfo(string name, string path)
            {
                this.name = name;
                this.path = path;
            }
        }
    }
}