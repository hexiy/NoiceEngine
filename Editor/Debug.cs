﻿using System.Collections.Generic;
using System.Diagnostics;

namespace Engine;

public static class Debug
{
	private static List<string> logs = new List<string>();

	private static readonly int LOG_LIMIT = 1000;

	public static Dictionary<string, Stopwatch> timers = new Dictionary<string, Stopwatch>();
	public static Dictionary<string, float> stats = new Dictionary<string, float>();

	public static void Log(string message)
	{
		logs.Add($"[{DateTime.Now.ToString("HH:mm:ss")}]   " + message);
		if (logs.Count > LOG_LIMIT)
		{
			logs.RemoveAt(0);
		}
	}
	public static void StartTimer(string timerName)
	{
		if (timers.ContainsKey(timerName))
		{
			timers[timerName].Start();
		}
		else
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			timers.Add(timerName, sw);
		}
	}
	public static void CountStat(string statName, float value)
	{
		if (stats.ContainsKey(statName)==false)
		{
			stats[statName] = 0;
		}
		stats[statName] += value;

	}
	public static void EndTimer(string timerName)
	{
		timers[timerName].Stop();
	}
	public static void ClearTimers()
	{
		timers.Clear();
	}
	public static void ClearStats()
	{
		stats.Clear();
	}


	public static void ClearLogs()
	{
		logs.Clear();
	}
	public static ref List<string> GetLogs()
	{
		return ref logs;
	}
}
