using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;

namespace BetterImport
{
    [InitializeOnLoad]
    public class Config : EditorWindow
    {
        static Config window;
        public const string configKey = "BetterImportConfig";
        public static ConfigData data;

        public struct ConfigData
        {
            public bool enableHints;
            public List<string> enabledHints;
            public bool createAnimationControllers;
            public bool fixNormalMapsType;
            public bool createMetallicSmoothnessMaps;
            public bool createAlbedoAlphaMaps;
            public bool makeMaterialsRoughByDefault;
        }

        public static Dictionary<string, bool> hints = new();

        static List<System.Type> hintTypes;

        void OnEnable()
        {
            Init();
        }

        [InitializeOnLoadMethod]
        static void Init()
        {
            // Debug.Log("BetterImport: Refreshing config...");

            RefreshHintTypes();

            if (ConfigExists())
            {
                data = ReadConfig();
                foreach (var hint in data.enabledHints)
                {
                    if (!hints.ContainsKey(hint))
                    {
                        hints.Add(hint, true);
                    }
                }
                foreach (var type in hintTypes)
                {
                    var name = Regex.Replace(type.Name, "Hint$", "");
                    if (!hints.ContainsKey(name))
                    {
                        hints.Add(name, true);
                    }
                }
            }
            else
            {
                Debug.Log("BetterImport: No config found, creating default config");
                data = new ConfigData();
                data.enableHints = true;
                data.enabledHints = new List<string>();
                data.createAnimationControllers = true;
                data.fixNormalMapsType = true;
                data.createMetallicSmoothnessMaps = true;
                data.createAlbedoAlphaMaps = true;
                data.makeMaterialsRoughByDefault = true;

                hints = new Dictionary<string, bool>();

                foreach (var type in hintTypes)
                {
                    var name = Regex.Replace(type.Name, "Hint$", "");
                    data.enabledHints.Add(name);
                    if (!hints.ContainsKey(name))
                    {
                        hints.Add(name, true);
                    }
                }
            }

            SaveConfig();
        }

        static void RefreshHintTypes()
        {
            hintTypes = Hint.GetHintTypes();
        }

        static bool ConfigExists()
        {
            return EditorPrefs.HasKey(configKey);
        }

        static ConfigData ReadConfig()
        {
            return JsonUtility.FromJson<ConfigData>(EditorPrefs.GetString(configKey));
        }

        static void SaveConfig()
        {
            data.enabledHints = hints.Keys.ToList().FindAll(key => hints[key]);
            EditorPrefs.SetString(configKey, JsonUtility.ToJson(data));
        }

        public static bool IsHintEnabled(System.Object hint)
        {
            var name = Regex.Replace(hint.GetType().Name, "Hint$", "");
            return hints.ContainsKey(name) && hints[name];
        }

        [MenuItem("Tools/Better Import/Settings")]
        public static void ShowWindow()
        {
            window = GetWindow<Config>();
            window.titleContent = new GUIContent("Better Import");
            window.minSize = new Vector2(200, 200);
            window.maxSize = new Vector2(800, 1000);
            window.Show();
        }

        [MenuItem("Tools/Better Import/Reset Config")]
        public static void ResetConfig()
        {
            EditorPrefs.DeleteKey(configKey);
            Init();
        }

        bool hintsExpanded = false;

        void OnGUI()
        {
            var style = new GUIStyle(GUI.skin.label);
            style.margin = new RectOffset(10, 10, 10, 10);

            EditorGUILayout.BeginVertical(style); 

            // Hints

            EditorGUILayout.LabelField("Hints", EditorStyles.largeLabel);

            data.enableHints = EditorGUILayout.ToggleLeft("Enable Hints", data.enableHints);

            if (data.enableHints)
            {
                hintsExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(hintsExpanded, "List of Hints");
                if (hintsExpanded)
                {
                    if (GUILayout.Button("Refresh Hint Types"))
                    {
                        RefreshHintTypes();
                    }

                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Select All"))
                    {
                        foreach (var key in hints.Keys.ToList())
                        {
                            hints[key] = true;
                        }
                        SaveConfig();
                    }
                    if (GUILayout.Button("Select None"))
                    {
                        foreach (var key in hints.Keys.ToList())
                        {
                            hints[key] = false;
                        }
                        SaveConfig();
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginVertical(GUI.skin.box);

                    foreach (var type in hintTypes)
                    {
                        var baseClassName = Regex.Replace(type.BaseType.Name, "Hint$", "");
                        var hintText = type.GetProperty("Text").GetValue(null) as string;
                        var name = Regex.Replace(type.Name, "Hint$", "");
                        hints[name] = EditorGUILayout.ToggleLeft($"{baseClassName}{hintText}", hints[name]);
                    }

                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            // Animations

            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Animations", EditorStyles.largeLabel);
            data.createAnimationControllers = EditorGUILayout.ToggleLeft("Create Animation Controllers", data.createAnimationControllers);

            // Materials

            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField("Materials", EditorStyles.largeLabel);
            data.fixNormalMapsType = EditorGUILayout.ToggleLeft("Fix Normal Maps Type", data.fixNormalMapsType);
            data.createMetallicSmoothnessMaps = EditorGUILayout.ToggleLeft("Create Metallic-Smoothness Maps", data.createMetallicSmoothnessMaps);
            data.createAlbedoAlphaMaps = EditorGUILayout.ToggleLeft("Create Albedo-Alpha Maps", data.createAlbedoAlphaMaps);
            data.makeMaterialsRoughByDefault = EditorGUILayout.ToggleLeft("Make Materials Rough By Default", data.makeMaterialsRoughByDefault);

            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                SaveConfig();
            }
        }
    }
}