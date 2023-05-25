using UnityEngine;

namespace BetterImport
{
    public sealed class RigidbodyHint : GameObjectHint
    {
        public override string Text => "-rigidbody";
        public override bool HasDerivedGameObject { get; } = false;

        public override void OnProcessSourceObject(GameObject sourceObject)
        {
            if (sourceObject.GetComponent<Rigidbody>() == null)
            {
                var rb = sourceObject.AddComponent<Rigidbody>();
                rb.interpolation = RigidbodyInterpolation.Interpolate;    
            }
            if (sourceObject.TryGetComponent<MeshCollider>(out var meshCollider))
            {
                meshCollider.convex = true;
            }
        }
    }
}