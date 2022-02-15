﻿namespace Engine;

[Serializable]
public class Texture
{
	public string path = "";
	public int id;
	public Vector2 size;
	public bool loaded = false;

	public void Load(string _path, bool flipX = true)
	{
		path = _path;
		Texture loadedTexture = TextureCache.GetTexture(_path, flipX);

		id = loadedTexture.id;
		size = loadedTexture.size;

		loaded = true;
	}
	public void Delete()
	{
		TextureCache.DeleteTexture(path);
	}
	public void Use()
	{
		TextureCache.BindTexture(id);
	}
}
