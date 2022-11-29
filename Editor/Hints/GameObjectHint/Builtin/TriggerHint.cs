using UnityEngine;

namespace BetterImport
{
    public sealed class TriggerHint : GameObjectHint
    {
        public override string Text => "-trigger";
        public override string ContainerName { get; } = "Synced Triggers";
        public override bool SyncScale { get; } = false;

        public override void OnCreateDerivedObject(GameObject sourceObject, GameObject derivedObject)
        {
            var collider = derivedObject.AddComponent<BoxCollider>();
            collider.isTrigger = true;
        }

        public override void OnUpdateDerivedObject(GameObject sourceObject, GameObject derivedObject)
        {
            var bounds = sourceObject.GetComponent<MeshFilter>().sharedMesh.bounds;
            var collider = derivedObject.GetComponent<BoxCollider>();
            collider.size = bounds.size;
        }
    }
}