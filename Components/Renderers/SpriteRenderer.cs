﻿using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Xml.Serialization;

namespace Engine;

public class SpriteRenderer : Renderer
{
	public Texture texture;

	public bool additive;
	public virtual bool Batched { get; set; } = true;

	public override void Awake()
	{
		material = new Material(ShaderCache.spriteRendererShader, BufferCache.spriteRendererVAO);
		if (texture == null)
		{
			texture = new Texture();
		}
		else
		{
			LoadTexture(texture.path);
		}

		base.Awake();
	}

	public void LoadTexture(string _texturePath)
	{
		if (_texturePath.Contains("Assets") == false)
		{
			_texturePath = Path.Combine("Assets", _texturePath);
		}

		if (File.Exists(_texturePath) == false)
		{
			return;
		}

		texture.Load(_texturePath);

		UpdateBoxShapeSize();
		if (Batched)
		{
			BatchingManager.AddGameObjectToBatcher(texture.id, this);
		}
	}

	internal virtual void UpdateBoxShapeSize()
	{
		if (boxShape != null)
		{
			boxShape.size = texture.size;
		}
	}

	public override void OnNewComponentAdded(Component comp)
	{
		if (comp is BoxShape && texture != null)
		{
			UpdateBoxShapeSize();
		}

		base.OnNewComponentAdded(comp);
	}

	public void SetMaterial(Shader shader, int vao)
	{
	}

	public override void Render()
	{
		if (onScreen == false) return;
		if (boxShape == null) return;
		if (texture.loaded == false) return;

		BatchingManager.UpdateAttribs(texture.id, gameObject);
	}
}
// STENCIL working

/*
public override void Render()
		{
			if (boxShape == null) return;
			if (texture.loaded == false) return;
			// stencil experiment
			GL.Enable(EnableCap.StencilTest);
			GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
			GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
			GL.StencilMask(0xFF);
			shader.Use();
			shader.SetMatrix4x4("u_mvp", GetModelViewProjection());
			shader.SetVector4("u_color", color.ToVector4());

			GL.BindVertexArray(vao);
			GL.Enable(EnableCap.Blend);

			if (additive)
			{
				GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusConstantColor);
			}
			else
			{
				GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
			}
			texture.Use();
			GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
			// stencil after
			GL.StencilFunc(StencilFunction.Notequal, 1, 0xFF);
			GL.StencilMask(0x00);
			GL.Disable(EnableCap.DepthTest);

			shader.Use();
			shader.SetMatrix4x4("u_mvp", GetModelViewProjectionForOutline(thickness));
			//shader.SetVector4("u_color", new Vector4(MathF.Abs(MathF.Sin(Time.elapsedTime * 0.3f)), MathF.Abs(MathF.Cos(Time.elapsedTime * 0.3f)), 1, 1));
			shader.SetVector4("u_color", Color.Black.ToVector4());

			texture.Use();
			GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

			GL.StencilMask(0xFF);
			GL.StencilFunc(StencilFunction.Always, 1, 0xFF);
			GL.Enable(EnableCap.DepthTest);

			GL.BindVertexArray(0);
			GL.Disable(EnableCap.Blend);
		}
*/