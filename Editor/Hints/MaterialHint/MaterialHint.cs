using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

namespace BetterImport
{
    public abstract class MaterialHint : Hint
    {
        public static Dictionary<string, MaterialHint> hintByHintText = new();

        static MaterialHint()
        {
            MakeInstancesOfAll<MaterialHint>().ForEach(hint => hintByHintText.Add(hint.Text, hint));
        }

        public virtual void OnPreprocess(AssetImporter assetImporter, string assetPath, AssetImportContext context, MaterialDescription description, Material material, AnimationClip[] materialAnimation) { }
        public virtual void OnPostprocess(AssetImporter assetImporter, string assetPath, AssetImportContext context, Material material) { }

        public class MaterialPostprocessor : AssetPostprocessor
        {
            void OnPreprocessMaterialDescription(MaterialDescription description, Material material, AnimationClip[] materialAnimation)
            {
                foreach (var (hintText, hint) in hintByHintText)
                {
                    if (!Config.IsHintEnabled(hint))
                    {
                        continue;
                    }
                    if (material.name.Contains(hintText))
                    {
                        hint.OnPreprocess(assetImporter, assetPath, context, description, material, materialAnimation);
                    }
                }
            }

            void OnPostprocessMaterial(Material material)
            {
                foreach (var hint in hintByHintText.Values)
                {
                    if (!Config.IsHintEnabled(hint))
                    {
                        continue;
                    }
                    if (material.name.Contains(hint.Text))
                    {
                        hint.OnPostprocess(assetImporter, assetPath, context, material);
                    }
                }
            }
        }
    }
}