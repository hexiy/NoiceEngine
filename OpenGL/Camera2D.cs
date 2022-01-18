using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
	class Camera2D
	{
		public Vector2 FocusPosition { get; set; }
		public float Zoom { get; set; }
		public Camera2D(Vector2 focusPosition, float zoom)
		{
			this.FocusPosition = focusPosition;
			this.Zoom = zoom;
		}
		public System.Numerics.Matrix4x4 GetProjectionMatrix()
		{
			float left = FocusPosition.X - DisplayManager.WindowSize.X;
			float right = FocusPosition.X + DisplayManager.WindowSize.X;
			float bottom = FocusPosition.Y - DisplayManager.WindowSize.Y;
			float top = FocusPosition.Y + DisplayManager.WindowSize.Y;
			//System.Numerics.Matrix4x4.CreateOrthographicOffCenter
			System.Numerics.Matrix4x4 orthoMatrix = System.Numerics.Matrix4x4.CreateOrthographicOffCenter(left, right, bottom, top, 0.001f, 1000f);

			return orthoMatrix;
		}
	}
}
