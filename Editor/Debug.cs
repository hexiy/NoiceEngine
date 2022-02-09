using System.Collections.Generic;

namespace Engine;

public static class Debug
{
	private static List<string> logs = new List<string>();

	private static readonly int LOG_LIMIT = 1000;

	public static void Log(string message)
	{
		logs.Add($"[{DateTime.Now.ToString("HH:mm:ss")}]   " + message);
		if (logs.Count > LOG_LIMIT)
		{
			logs.RemoveAt(0);
		}
	}
	public static void Clear()
	{
		logs.Clear();
	}
	public static ref List<string> GetLogs()
	{
		return ref logs;
	}
}
