using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace BetterImport
{
    public sealed class PrefabHint : GameObjectHint
    {
        public override string Text => "-prefab";
        public override string ContainerName { get; } = "Synced Prefabs";
        public override bool SyncScale { get; } = true;

        List<GameObject> prefabs;

        public override void Init()
        {
            var prefabGuids = AssetDatabase.FindAssets("t:Prefab");
            var prefabPaths = prefabGuids.Select(AssetDatabase.GUIDToAssetPath);
            prefabs = prefabPaths.Select(AssetDatabase.LoadAssetAtPath<GameObject>).ToList();
        }
        
        public override GameObject CreateGameObject(GameObject sourceObject, GameObject container)
        {
            var derivedObjectName = sourceObject.name.Replace(Text, "");

            // Check if an object with such name already exists
            var derivedObject = container.transform.Find(derivedObjectName)?.gameObject;
            if (derivedObject == null)
            {
                // Create a new object
                var prefab = prefabs.Find(p =>
                {
                    return p.name.ToLower() == Regex.Replace(derivedObjectName, @"\.\d+", "").ToLower();
                });

                if (prefab == null)
                {
                    return null;
                }

                derivedObject = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                derivedObject.name = derivedObjectName;
                derivedObject.transform.SetParent(container.transform, false);
                sourceObject.SetActive(false);
            }
            return derivedObject;
        }
    }
}