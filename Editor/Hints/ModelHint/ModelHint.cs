using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

namespace BetterImport
{
    public abstract class ModelHint : Hint
    {
        public static Dictionary<string, ModelHint> hintByHintText = new();

        static ModelHint()
        {
            MakeInstancesOfAll<ModelHint>().ForEach(hint => hintByHintText.Add(hint.Text, hint));
        }

        public virtual void OnPreprocess(AssetImporter assetImporter, string assetPath, AssetImportContext context) { }
        public virtual void OnPostprocess(AssetImporter assetImporter, string assetPath, AssetImportContext context, GameObject gameObject) { }

        public class ModelPostprocessor : AssetPostprocessor
        {
            void OnPreprocessModel()
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

            void OnPostprocessModel(GameObject gameObject)
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
                        hint.OnPostprocess(assetImporter, assetPath, context, gameObject);
                    }
                }
            }
        }
    }
}