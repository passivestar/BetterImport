using UnityEngine;

namespace BetterImport
{
    public sealed class ReflectionProbeHint : GameObjectHint
    {
        public override string Text => "-reflectionprobe";
        public override string ContainerName { get; } = "Synced Reflection Probes";
        public override bool SyncScale { get; } = false;

        public override void OnCreateDerivedObject(GameObject sourceObject, GameObject derivedObject)
        {
            derivedObject.AddComponent<ReflectionProbe>();
        }

        public override void OnUpdateDerivedObject(GameObject sourceObject, GameObject derivedObject)
        {
            var bounds = sourceObject.GetComponent<MeshFilter>().sharedMesh.bounds;
            var probe = derivedObject.GetComponent<ReflectionProbe>();
            var sourceScale = sourceObject.transform.localScale;
            // Probe size should have positive values
            var size = new Vector3(
                bounds.size.x * Mathf.Abs(sourceScale.x),
                bounds.size.y * Mathf.Abs(sourceScale.y),
                bounds.size.z * Mathf.Abs(sourceScale.z)
            );
            // "Rotate" the size to make sure it's properly aligned
            probe.size = sourceObject.transform.rotation * size;
        }
    }
}