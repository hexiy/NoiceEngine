using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Engine;

public static class MaterialAssetManager
{
	public static void CreateDefaultMaterials()
	{
		{
			Material boxMaterial = new Material();

			Shader boxShader = new Shader(Path.Combine(Folders.Shaders, "BoxRenderer.glsl"));
			boxMaterial.SetShader(boxShader);
			using (var sw = new StreamWriter(Path.Combine(Folders.Materials, "BoxMaterial.mat")))
			{
				var xmlSerializer = new XmlSerializer(typeof(Material));

				xmlSerializer.Serialize(sw, boxMaterial);
			}
		}
		{
			Material renderTextureMaterial = new Material();

			Shader renderTextureShader = new Shader(Path.Combine(Folders.Shaders, "RenderTexture.glsl"));
			renderTextureMaterial.SetShader(renderTextureShader);
			using (var sw = new StreamWriter(Path.Combine(Folders.Materials, "RenderTexture.mat")))
			{
				var xmlSerializer = new XmlSerializer(typeof(Material));

				xmlSerializer.Serialize(sw, renderTextureMaterial);
			}
		}
		{
			Material renderTextureMaterial = new Material();

			Shader renderTextureShader = new Shader(Path.Combine(Folders.Shaders, "SpriteRenderer.glsl"));
			renderTextureMaterial.SetShader(renderTextureShader);
			using (var sw = new StreamWriter(Path.Combine(Folders.Materials, "SpriteRenderer.mat")))
			{
				var xmlSerializer = new XmlSerializer(typeof(Material));

				xmlSerializer.Serialize(sw, renderTextureMaterial);
			}
		}
	}

	public static Material LoadMaterial(string materialName)
	{
		using (var sr = new StreamReader(Path.Combine(Folders.Materials, materialName)))
		{
			var xmlSerializer = new XmlSerializer(typeof(Material));
			Material mat = (Material) xmlSerializer.Deserialize(sr);
			if (mat.shader!=null)
			{
				mat.SetShader(mat.shader);
			}

			return mat;
		}
	}

	public static void SaveMaterial(Material material)
	{
		using (var sw = new StreamWriter(material.path))
		{
			var xmlSerializer = new XmlSerializer(typeof(Material));

			xmlSerializer.Serialize(sw, material);
		}
	}
}