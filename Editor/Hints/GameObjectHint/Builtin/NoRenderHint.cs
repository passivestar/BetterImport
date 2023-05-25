using UnityEngine;

namespace BetterImport
{
    public sealed class NoRenderHint : GameObjectHint
    {
        public override string Text => "-norender";
        public override bool HasDerivedGameObject { get; } = false;

        public override void OnProcessSourceObject(GameObject sourceObject)
        {
            var renderer = sourceObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }
    }
}