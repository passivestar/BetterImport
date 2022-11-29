#if USE_SRP
using UnityEngine;
using UnityEngine.Rendering;

namespace BetterImport
{
    public sealed class VolumeHint : GameObjectHint
    {
        public override string Text => "-volume";
        public override string ContainerName { get; } = "Synced Volumes";

        public override void OnCreateDerivedObject(GameObject sourceObject, GameObject derivedObject)
        {
            var collider = derivedObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;

            var volume = derivedObject.AddComponent<Volume>();
            volume.isGlobal = false;
        }

        public override void OnUpdateDerivedObject(GameObject sourceObject, GameObject derivedObject)
        {
            var bounds = sourceObject.GetComponent<MeshFilter>().sharedMesh.bounds;
            var collider = derivedObject.GetComponent<BoxCollider>();
            collider.size = bounds.size;
        }
    }
}
#endif