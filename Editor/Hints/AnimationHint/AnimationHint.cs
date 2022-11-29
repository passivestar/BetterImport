using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

namespace BetterImport
{
    public abstract class AnimationHint : Hint
    {
        public static Dictionary<string, AnimationHint> hintByHintText = new();

        static AnimationHint()
        {
            MakeInstancesOfAll<AnimationHint>().ForEach(hint => hintByHintText.Add(hint.Text, hint));
        }

        public virtual void OnPreprocess(AssetImporter assetImporter, string assetPath, AssetImportContext context) {}
        public virtual void OnPostprocess(AssetImporter assetImporter, string assetPath, AssetImportContext context, GameObject root, AnimationClip clip) {}

        public class AnimationPostprocessor : AssetPostprocessor
        {
            void OnPreprocessAnimation()
            {
                foreach (var (hintText, hint) in hintByHintText)
                {
                    if (!Config.IsHintEnabled(hint))
                    {
                        continue;
                    }
                    var filename = Path.GetFileNameWithoutExtension(assetPath);
                    if (filename.Contains(hintText))
                    {
                        hint.OnPreprocess(assetImporter, assetPath, context);
                    }
                }
            }

            void OnPostprocessAnimation(GameObject root, AnimationClip clip)
            {
                foreach (var hint in hintByHintText.Values)
                {
                    if (!Config.IsHintEnabled(hint))
                    {
                        continue;
                    }
                    var filename = Path.GetFileNameWithoutExtension(assetPath);
                    if (filename.Contains(hint.Text))
                    {
                        hint.OnPostprocess(assetImporter, assetPath, context, root, clip);
                    }
                }
            }
        }
    }
}