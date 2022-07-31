namespace Engine;

public static class Playmode
{
	public static void PlayMode_Start()
	{
		Scene.I.SaveScene();
		Global.GameRunning = true;
	}

	public static void PlayMode_Stop()
	{
		Scene.I.LoadScene(Scene.I.scenePath);
		Global.GameRunning = false;
	}

	private static void SaveCurrentSceneBeforePlay()
	{
	}

	private static void LoadSceneSavedBeforePlay()
	{
	}
}