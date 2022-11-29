using UnityEngine;

namespace BetterImport
{
    public sealed class BoxColliderHint : GameObjectHint
    {
        public override string Text => "-boxcollider";
        public override bool HasDerivedGameObject { get; } = false;

        public override void OnProcessSourceObject(GameObject sourceObject)
        {
            var boxCollider = sourceObject.GetComponent<BoxCollider>();
            if (boxCollider == null)
            {
                boxCollider = sourceObject.AddComponent<BoxCollider>();
            }
        }
    }
}