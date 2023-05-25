using UnityEngine;

namespace BetterImport
{
    public sealed class MixedLightHint : GameObjectHint
    {
        public override string Text => "-mixedlight";
        public override bool HasDerivedGameObject { get; } = false;

        public override void OnProcessSourceObject(GameObject sourceObject)
        {
            if (sourceObject.TryGetComponent<Light>(out var light))
            {
                light.lightmapBakeType = LightmapBakeType.Mixed;
            }
            else
            {
                Debug.LogWarning($"GameObject {sourceObject.name} does not have a Light component.");
            }
        }
    }
}