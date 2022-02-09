﻿namespace Engine;

public static class Time
{
	public static float deltaTime = 0.01666666f;
	public static float fixedDeltaTime = 0.02f;
	public static float elapsedTime = 0f;
	public static float elapsedSeconds = 0f;
	public static ulong elapsedTicks = 0;
	public static ulong timeScale = 0;

	public static void Update()
	{
		deltaTime = (float)Window.I.UpdateTime;
		if (elapsedTicks % 5 == 0)
		{
			//Debug.Log("fps:  " + (int)(1f / deltaTime));
		}
		//if (Global.GameRunning)
		//{
		elapsedTime += deltaTime;
		elapsedSeconds = elapsedTime * 1000;
		elapsedTicks++;
		//}

	}
}
