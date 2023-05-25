using UnityEngine;

namespace BetterImport
{
    public sealed class BakedLightHint : GameObjectHint
    {
        public override string Text => "-bakedlight";
        public override bool HasDerivedGameObject { get; } = false;

        public override void OnProcessSourceObject(GameObject sourceObject)
        {
            if (sourceObject.TryGetComponent<Light>(out var light))
            {
                light.lightmapBakeType = LightmapBakeType.Baked;
            }
            else
            {
                Debug.LogWarning($"GameObject {sourceObject.name} does not have a Light component.");
            }
        }
    }
}