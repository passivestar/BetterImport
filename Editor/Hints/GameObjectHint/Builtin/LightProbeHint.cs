using UnityEngine;

namespace BetterImport
{
    public sealed class LightProbeHint : GameObjectHint
    {
        public override string Text => "-lightprobe";
        public override string ContainerName { get; } = "Synced Light Probes";
        public override bool SyncScale { get; } = false;

        const float lightProbeInitialSize = 2f; // 2 meters

        public override void OnCreateDerivedObject(GameObject sourceObject, GameObject derivedObject)
        {
            derivedObject.AddComponent<LightProbeGroup>();
        }

        public override void OnUpdateDerivedObject(GameObject sourceObject, GameObject derivedObject)
        {
            var bounds = sourceObject.GetComponent<MeshFilter>().sharedMesh.bounds;
            var probe = derivedObject.GetComponent<LightProbeGroup>();
            var sourceScale = sourceObject.transform.localScale;
            // Probe scale should have positive values and
            // take into account initial probe size
            var scale = new Vector3(
                bounds.size.x * Mathf.Abs(sourceScale.x),
                bounds.size.y * Mathf.Abs(sourceScale.y),
                bounds.size.z * Mathf.Abs(sourceScale.z)
            );
            derivedObject.transform.localScale = scale / lightProbeInitialSize;
        }
    }
}