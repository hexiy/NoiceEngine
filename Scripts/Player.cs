using Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
	public class Player : Component
	{
		public static Player I { get; set; }

		public override void Awake()
		{
			I = this;

			base.Awake();
		}
		public override void Start()
		{
			base.Start();
		}
	}
}
