using UnityEngine;

namespace BetterImport
{
    public sealed class ReverbZoneHint : GameObjectHint
    {
        public override string Text => "-reverbzone";
        public override string ContainerName { get; } = "Synced Reverb Zones";
        public override bool SyncScale { get; } = false;

        public override void OnCreateDerivedObject(GameObject sourceObject, GameObject derivedObject)
        {
            derivedObject.AddComponent<AudioReverbZone>();
        }

        public override void OnUpdateDerivedObject(GameObject sourceObject, GameObject derivedObject)
        {
            var bounds = sourceObject.GetComponent<MeshFilter>().sharedMesh.bounds;
            var reverbZone = derivedObject.GetComponent<AudioReverbZone>();
            var sourceScale = sourceObject.transform.localScale;
            var distance = Mathf.Max(new float[] {
                bounds.extents.x * sourceScale.x,
                bounds.extents.y * sourceScale.y,
                bounds.extents.z * sourceScale.z
                });
            reverbZone.minDistance = distance;
            reverbZone.maxDistance = reverbZone.minDistance * 1.2f;
        }
    }
}