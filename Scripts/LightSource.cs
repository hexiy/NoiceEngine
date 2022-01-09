using Engine;
using Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LightSource : Component
{
	[ShowInEditor] public float intensity = 5;
	[ShowInEditor] public float falloff = 2;
	[ShowInEditor] public float parameteridk = 1;
	[ShowInEditor] public float flickerStrength = 1;
}

