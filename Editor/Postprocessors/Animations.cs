using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System.Linq;

namespace BetterImport
{
    public class AnimationsPostprocessor : AssetPostprocessor
    {
        static AnimatorController GetOrCreateAnimatorController(string assetPath)
        {
            var controllerPath = Path.ChangeExtension(assetPath, ".controller");
            return File.Exists(controllerPath)
                ? AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath)
                : AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        }

        void OnPostprocessAnimation(GameObject root, AnimationClip clip)
        {
            if (!Config.data.createAnimationControllers)
            {
                return;
            }

            if (root.GetComponent<Animator>() == null)
            {
                root.AddComponent<Animator>();
            }
        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (!Config.data.createAnimationControllers)
            {
                return;
            }

            foreach (var asset in importedAssets.Where(asset => asset.EndsWith(".fbx")))
            {
                var clips = AssetDatabase.LoadAllAssetRepresentationsAtPath(asset)
                    .OfType<AnimationClip>()
                    .ToList();

                if (clips.Count > 0)
                {
                    var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(asset);
                    var animator = gameObject.GetComponent<Animator>();
                    var controller = GetOrCreateAnimatorController(asset);
                    animator.runtimeAnimatorController = controller;

                    var controllerClips = controller.animationClips.ToList();
                    foreach (var clip in clips)
                    {
                        if (!controllerClips.Contains(clip))
                        {
                            clip.name = clip.name.Replace(".", "_");
                            controller.AddMotion(clip);
                        }
                    }
                }
            }
        }
    }
}