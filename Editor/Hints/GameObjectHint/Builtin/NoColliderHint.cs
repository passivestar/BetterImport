using UnityEngine;

namespace BetterImport
{
    public sealed class NoColliderHint : GameObjectHint
    {
        public override string Text => "-nocollider";
        public override bool HasDerivedGameObject { get; } = false;

        public override void OnProcessSourceObject(GameObject sourceObject)
        {
            var meshCollider = sourceObject.GetComponent<MeshCollider>();
            if (meshCollider != null)
            {
                meshCollider.enabled = false;
            }
        }
    }
}