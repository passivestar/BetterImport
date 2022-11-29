using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

namespace BetterImport
{
    public sealed class ColliderHint : ModelHint
    {
        public override string Text => "-collider";

        public override void OnPreprocess(AssetImporter assetImporter, string assetPath, AssetImportContext context)
        {
            var modelImporter = assetImporter as ModelImporter;
            modelImporter.addCollider = true;
        }
    }
}