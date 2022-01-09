using Engine;
using Microsoft.Xna.Framework;
using Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class PoleLightSwinger : Component
{

	public override void Update()
	{
		transform.rotation = new Vector3(0, 0, (float)Math.Sin(Time.elapsedTime*2) * MathF.PI / 10);
	}
}

