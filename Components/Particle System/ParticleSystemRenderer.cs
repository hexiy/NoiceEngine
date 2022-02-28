using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Engine;
using System.Diagnostics;

namespace Scripts;

public class ParticleSystemRenderer : SpriteRenderer
{
	public new bool allowMultiple = false;
	public ParticleSystem particleSystem;

	private int particlesInBatcher = 0;

	public override void Awake()
	{
		material.additive = true;
		BatchingManager.AddGameObjectToBatcher(texture.id, this);

		base.Awake();
	}

	public override void Render()
	{
		if (onScreen == false) return;
		if (boxShape == null) return;
		if (texture.loaded == false) return;

		while (particlesInBatcher < particleSystem.particles.Count)
		{
			BatchingManager.AddGameObjectToBatcher(texture.id, this, particlesInBatcher);
			particlesInBatcher++;
		}

		for (int i = 0; i < particleSystem.particles.Count; i++)
		{
			BatchingManager.UpdateAttribs(texture.id, gameObjectID, particleSystem.particles[i].worldPosition, new Vector2(particleSystem.particles[i].radius),
			                              particleSystem.particles[i].color, i);
		}

		Debug.Stat("Particles", particleSystem.particles.Count);
	}
}