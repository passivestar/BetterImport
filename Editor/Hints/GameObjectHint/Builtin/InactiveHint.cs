using UnityEngine;

namespace BetterImport
{
    public sealed class InactiveHint : GameObjectHint
    {
        public override string Text => "-inactive";
        public override bool HasDerivedGameObject { get; } = false;

        public override void OnProcessSourceObject(GameObject sourceObject)
        {
            sourceObject.SetActive(false);
        }
    }
}