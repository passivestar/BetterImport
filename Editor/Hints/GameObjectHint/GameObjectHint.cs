using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;

namespace BetterImport
{
    public abstract class GameObjectHint : Hint
    {
        public virtual string ContainerName { get; }
        public virtual bool SyncPosition { get; } = true;
        public virtual bool SyncRotation { get; } = true;
        public virtual bool SyncScale { get; } = true;
        public virtual bool HasDerivedGameObject { get; } = true;

        public static List<string> containerNames = new();
        public static Dictionary<string, GameObjectHint> hintByHintText = new();

        public static string StripAllHints(string name)
        {
            return Regex.Replace(name, string.Join("|", hintByHintText.Keys.ToArray()), "");
        }

        public static List<GameObject> GetAllGameObjects()
        {
            var gameObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
            var validGameObjects = gameObjects.ToList().Where(o => o.scene.IsValid()).ToList();
            return validGameObjects;
        }

        static GameObjectHint()
        {
            foreach (var hint in MakeInstancesOfAll<GameObjectHint>())
            {
                containerNames.Add(hint.ContainerName);
                hintByHintText.Add(hint.Text, hint);
            }
        }

        public virtual void OnProcessSourceObject(GameObject sourceObject)
        {
            // When the source object is first encountered, this method is called
        }

        public virtual void OnCreateDerivedObject(GameObject sourceObject, GameObject derivedObject)
        {
            // When a new derived GameObject is created, this method is called
        }

        public virtual void OnUpdateDerivedObject(GameObject sourceObject, GameObject derivedObject)
        {
            // When an existing derived GameObject is updated, this method is called
        }

        public virtual GameObject CreateGameObject(GameObject sourceObject, GameObject container)
        {
            var derivedObjectName = StripAllHints(sourceObject.name);

            // Check if an object with such name already exists
            var derivedObject = container.transform.Find(derivedObjectName)?.gameObject;
            if (derivedObject == null)
            {
                // Create a new object
                derivedObject = new GameObject();
                derivedObject.name = derivedObjectName;
                derivedObject.transform.SetParent(container.transform, false);
                OnCreateDerivedObject(sourceObject, derivedObject);
                sourceObject.SetActive(false);
            }
            return derivedObject;
        }

        public virtual void Init()
        {
            // Called first on PostprocessAllAssets
        }

        public class Postprocessor : AssetPostprocessor
        {
            static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                if (Config.data.enableHints)
                {
                    // Initiating sync asyncronously to avoid problems:
                    Sync();
                }
            }

            static async void Sync()
            {
                await Task.Delay(100);

                foreach (var hint in hintByHintText.Values)
                {
                    hint.Init();
                }

                var gameObjects = GetAllGameObjects();

                foreach (var obj in gameObjects)
                {
                    // If a container, remove derived objects if their source is gone
                    if (containerNames.Contains(obj.name))
                    {
                        var parent = obj.transform.parent;
                        // Convert to a list so that we can destroy objects while iterating
                        foreach (Transform t in obj.transform.Cast<Transform>().ToList())
                        {
                            var go = t.gameObject;
                            var children = parent.Cast<Transform>().ToList();

                            var sourceFound = children.Any(g =>
                            {
                                return hintByHintText.Keys.Any(hint => g.name.Contains(hint))
                                    && StripAllHints(g.name) == go.name;
                            });

                            if (!sourceFound)
                            {
                                UnityEngine.Object.DestroyImmediate(go);
                            }
                        }
                        // Destroy the container if it's empty
                        if (obj.transform.childCount == 0)
                        {
                            UnityEngine.Object.DestroyImmediate(obj);
                        }
                    }

                    if (obj == null)
                    {
                        continue;
                    }

                    // If not a container, look for a hints
                    foreach (var (hintText, hint) in hintByHintText)
                    {
                        if (!Config.IsHintEnabled(hint))
                        {
                            continue;
                        }

                        if (obj.name.Contains(hintText))
                        {
                            hint.OnProcessSourceObject(obj);

                            if (hint.ContainerName == null)
                            {
                                continue;
                            }

                            var parent = obj.transform.parent;

                            // Find or create a container
                            var container = parent.Find(hint.ContainerName)?.gameObject;
                            if (container == null)
                            {
                                container = new GameObject();
                                container.name = hint.ContainerName;
                                container.transform.SetParent(parent);
                            }

                            var derivedObject = hint.CreateGameObject(obj, container);

                            // Sync position
                            var transform = obj.transform;
                            derivedObject.transform.position = transform.position;
                            derivedObject.transform.rotation = transform.rotation;
                            if (hint.SyncScale)
                            {
                                derivedObject.transform.localScale = transform.localScale;
                            }

                            hint.OnUpdateDerivedObject(obj, derivedObject);
                        }
                    }
                }
            }
        }
    }
}