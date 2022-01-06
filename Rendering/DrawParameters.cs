﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class DrawParameters
{
	public SpriteSortMode sortMode = SpriteSortMode.Deferred;
	public BlendState blendState = null;
	public SamplerState samplerState = null;
	public DepthStencilState depthStencilState = null;
	public RasterizerState rasterizerState = null;
	public Effect effect = null;
	public Matrix? transformMatrix = null;
}
