using UnityEngine;

namespace BetterImport
{
    public sealed class StaticHint : GameObjectHint
    {
        public override string Text => "-static";
        public override bool HasDerivedGameObject { get; } = false;

        public override void OnProcessSourceObject(GameObject sourceObject)
        {
            sourceObject.isStatic = true;
        }
    }
}