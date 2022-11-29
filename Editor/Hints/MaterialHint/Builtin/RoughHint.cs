using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

namespace BetterImport
{
    public sealed class RoughHint : MaterialHint
    {
        public override string Text => "-rough";

        public override void OnPreprocess(AssetImporter assetImporter, string assetPath, AssetImportContext context, MaterialDescription description, Material material, AnimationClip[] materialAnimation)
        {
            material.SetFloat("_Smoothness", 0);
        }
    }
}