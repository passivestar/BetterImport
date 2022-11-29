using UnityEditor;
using UnityEditor.AssetImporters;

namespace BetterImport
{
    public sealed class LoopHint : AnimationHint
    {
        public override string Text => "-loop";

        public override void OnPreprocess(AssetImporter assetImporter, string assetPath, AssetImportContext context)
        {
            var modelImporter = assetImporter as ModelImporter;
            foreach (var clipAnimation in modelImporter.clipAnimations)
            {
                clipAnimation.loopTime = true;
            }
        }
    }
}