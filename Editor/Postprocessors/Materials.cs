using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEditor.AssetImporters;

namespace BetterImport
{
public class MaterialsPostprocessor : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if (!Config.data.createMetallicSmoothnessMaps)
            {
                return;
            }

            var filename = Path.GetFileNameWithoutExtension(assetPath);
            var textureImporter = (TextureImporter)assetImporter;

            if (filename.Contains("Normal"))
            {
                textureImporter.textureType = TextureImporterType.NormalMap;
            }

            if (!(filename.EndsWith("Metallic") || filename.EndsWith("Roughness") || filename.EndsWith("Smoothness")))
            {
                return;
            }

            textureImporter.isReadable = true;
            textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            textureImporter.mipmapEnabled = false;
        }

        void OnPostprocessTexture(Texture2D texture)
        {
            if (!Config.data.createMetallicSmoothnessMaps)
            {
                return;
            }

            var filename = Path.GetFileNameWithoutExtension(assetPath);

            if (filename.EndsWith("Metallic") && !MetallicSmoothnessAlreadyExists("Metallic"))
            {
                ProcessMetallicTexture(texture, filename);
                return;
            }

            if (filename.EndsWith("Roughness") && !MetallicSmoothnessAlreadyExists("Roughness"))
            {
                ProcessRoughnessTexture(texture, filename);
                return;
            }

            if (filename.EndsWith("Smoothness") && !filename.EndsWith("MetallicSmoothness") && !MetallicSmoothnessAlreadyExists("Smoothness"))
            {
                ProcessSmoothnessTexture(texture, filename);
                return;
            }
        }

        string GetMetallicSmoothnessPath(string suffix)
        {
            var filename = Path.GetFileNameWithoutExtension(assetPath);
            var directory = Path.GetDirectoryName(assetPath);
            var extension = Path.GetExtension(assetPath);
            return Path.Combine(directory, filename.Replace(suffix, "MetallicSmoothness") + ".png");
        }

        bool MetallicSmoothnessAlreadyExists(string suffix)
        {
            return File.Exists(GetMetallicSmoothnessPath(suffix));
        }

        void CombineTextures(string path, Func<Color32[], Color32[], Color32[]> combiner, Texture2D textureA = null, Texture2D textureB = null)
        {
            var height = textureA != null ? textureA.height : textureB.height;
            var width = textureA != null ? textureA.width : textureB.width;

            if (textureA == null)
            {
                textureA = new Texture2D(width, height, TextureFormat.ARGB32, false);
                textureA.SetPixels32(new Color32[width * height]);
            }
            else if (textureB == null)
            {
                textureB = new Texture2D(width, height, TextureFormat.ARGB32, false);
                textureB.SetPixels32(new Color32[width * height]);
            }
            else if (textureA.width != textureB.width || textureA.height != textureB.height)
            {
                Debug.LogError("Textures must be the same size");
                return;
            }

            var textureAPixels = textureA.GetPixels32();
            var textureBPixels = textureB.GetPixels32();

            var combined = new Texture2D(textureA.width, textureA.height, TextureFormat.ARGB32, false);
            var combinedPixels = combiner(textureAPixels, textureBPixels);

            combined.SetPixels32(combinedPixels);
            File.WriteAllBytes(path, combined.EncodeToPNG());
            AssetDatabase.ImportAsset(path);
        }

        void CombineTexturesMetallicRoughness(string path, Texture2D metallic, Texture2D roughness)
        {
            CombineTextures(path, (metallicPixels, roughnessPixels) =>
            {
                for (int i = 0; i < metallicPixels.Length; i++)
                {
                    metallicPixels[i].a = (byte)(byte.MaxValue - roughnessPixels[i].r);
                }
                return metallicPixels;
            }, metallic, roughness);
        }

        void CombineTexturesMetallicSmoothness(string path, Texture2D metallic, Texture2D smoothness)
        {
            CombineTextures(path, (metallicPixels, smoothnessPixels) =>
            {
                for (int i = 0; i < metallicPixels.Length; i++)
                {
                    metallicPixels[i].a = smoothnessPixels[i].r;
                }
                return metallicPixels;
            }, metallic, smoothness);
        }

        void CombineTexturesMetallic(string path, Texture2D metallic)
        {
            CombineTextures(path, (metallicPixels, _) => metallicPixels, metallic);
        }

        void CombineTexturesRoughness(string path, Texture2D roughness)
        {
            CombineTextures(path, (blackPixels, roughnessPixels) =>
            {
                for (int i = 0; i < blackPixels.Length; i++)
                {
                    blackPixels[i].a = (byte)(byte.MaxValue - roughnessPixels[i].r);
                }
                return blackPixels;
            }, null, roughness);
        }

        void CombineTexturesSmoothness(string path, Texture2D smoothness)
        {
            CombineTextures(path, (blackPixels, smoothnessPixels) =>
            {
                for (int i = 0; i < blackPixels.Length; i++)
                {
                    blackPixels[i].a = smoothnessPixels[i].r;
                }
                return blackPixels;
            }, null, smoothness);
        }

        void ProcessMetallicTexture(Texture2D texture, string filename)
        {
            Texture2D metallic = texture;
            var roughnessPath = assetPath.Replace("Metallic", "Roughness");
            var smoothnessPath = assetPath.Replace("Metallic", "Smoothness");
            var metallicSmoothnessPath = GetMetallicSmoothnessPath("Metallic");

            if (File.Exists(roughnessPath))
            {
                var roughness = AssetDatabase.LoadAssetAtPath<Texture2D>(roughnessPath);
                if (roughness != null)
                {
                    CombineTexturesMetallicRoughness(metallicSmoothnessPath, metallic, roughness);
                }
            }
            else if (File.Exists(smoothnessPath))
            {
                var smoothness = AssetDatabase.LoadAssetAtPath<Texture2D>(smoothnessPath);
                if (smoothness != null)
                {
                    CombineTexturesMetallicSmoothness(metallicSmoothnessPath, metallic, smoothness);
                }
            }
            else
            {
                CombineTexturesMetallic(metallicSmoothnessPath, metallic);
            }
        }

        void ProcessRoughnessTexture(Texture2D texture, string filename)
        {
            Texture2D roughness = texture;
            var metallicPath = assetPath.Replace("Roughness", "Metallic");
            var metallicSmoothnessPath = GetMetallicSmoothnessPath("Roughness");

            if (File.Exists(metallicPath))
            {
                var metallic = AssetDatabase.LoadAssetAtPath<Texture2D>(metallicPath);
                if (metallic != null)
                {
                    CombineTexturesMetallicRoughness(metallicSmoothnessPath, metallic, roughness);
                }
            }
            else
            {
                CombineTexturesRoughness(metallicSmoothnessPath, roughness);
            }
        }

        void ProcessSmoothnessTexture(Texture2D texture, string filename)
        {
            Texture2D smoothness = texture;
            var metallicPath = assetPath.Replace("Smoothness", "Metallic");
            var metallicSmoothnessPath = GetMetallicSmoothnessPath("Smoothness");

            if (File.Exists(metallicPath))
            {
                var metallic = AssetDatabase.LoadAssetAtPath<Texture2D>(metallicPath);
                if (metallic != null)
                {
                    CombineTexturesMetallicSmoothness(metallicSmoothnessPath, metallic, smoothness);
                }
            }
            else
            {
                CombineTexturesSmoothness(metallicSmoothnessPath, smoothness);
            }
        }

        public void OnPreprocessMaterialDescription(MaterialDescription description, Material material, AnimationClip[] materialAnimation)
        {
            if (!Config.data.createMetallicSmoothnessMaps)
            {
                return;
            }

            // Try to assign the metallic-smoothness map:
            var materialName = description.materialName;
            var textures = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets" });

            var metallicSmoothnessMapPath = textures
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault(t =>
                {
                    var filename = Path.GetFileNameWithoutExtension(t);
                    return filename.Contains(materialName) && filename.Contains("MetallicSmoothness");
                });

            if (metallicSmoothnessMapPath != null)
            {
                var metallicSmoothnessMap = AssetDatabase.LoadAssetAtPath<Texture2D>(metallicSmoothnessMapPath);
                material.SetTexture("_MetallicGlossMap", metallicSmoothnessMap);
            }
            else
            {
                material.SetFloat("_Smoothness", 0f);
            }
        }
    }
}