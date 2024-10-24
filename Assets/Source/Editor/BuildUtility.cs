using System.IO;
using UnityEditor;
using UnityEngine;

namespace Quinn.Editor
{
	public static class BuildUtility
	{
		private const string ItchPublisherID = "quinncg";
		private const string ItchProjectID = "call-of-the-cards";
		private static string BasePath
		{
			get
			{
				string path = $"{Application.dataPath}";
				path = path[..path.LastIndexOf('/')];
				path += "/Builds";

				return path;
			}
		}

		[MenuItem("Tools/Build/Build and Run Windows", priority = 200)]
		public static void BuildAndRunWindows()
		{
			Build(BuildTarget.StandaloneWindows, run: true);
		}

		[MenuItem("Tools/Build/Build Windows", priority = 90)]
		public static void BuildWindows()
		{
			Build(BuildTarget.StandaloneWindows);
		}

		[MenuItem("Tools/Build/Build Mac", priority = 95)]
		public static void BuildMac()
		{
			Build(BuildTarget.StandaloneOSX);
		}

		[MenuItem("Tools/Build/Build Linux", priority = 97)]
		public static void BuildLinux()
		{
			Build(BuildTarget.StandaloneLinux64);
		}

		[MenuItem("Tools/Build/Build All", priority = 150)]
		public static void BuildAll()
		{
			Build(BuildTarget.StandaloneWindows, open: false);
			Build(BuildTarget.StandaloneOSX, open: false);
			Build(BuildTarget.StandaloneLinux64, open: false);
		}

		[MenuItem("Tools/Build/Build and Publish Windows", priority = 300)]
		public static void BuildAndPublishWindows()
		{

		}

		private static void Build(BuildTarget target, bool run = false, bool open = true)
		{
			string platformFolder = target switch
			{
				BuildTarget.StandaloneWindows => "Windows",
				BuildTarget.StandaloneOSX => "Mac",
				BuildTarget.StandaloneLinux64 => "Linux",
				_ => throw new System.NotImplementedException($"Failed to build. Invalid build target '{target}'!")
			};

			string path = BasePath + $"/{platformFolder}";

			string fullPath = path + $"/{Application.productName}.exe";
			var options = BuildOptions.None;
			if (run) options |= BuildOptions.AutoRunPlayer;
			if (open) options |= BuildOptions.ShowBuiltPlayer;

			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}

			Debug.Log($"Building at {fullPath}...");
			var report = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, fullPath, target, options);
			Debug.Log($"Build finished in {report.summary.totalTime.TotalMilliseconds}ms.");

			foreach (var dir in Directory.GetDirectories(path))
			{
				if (dir.Contains("DoNotShip"))
				{
					Directory.Delete(dir, true);
					break;
				}
			}
		}

		private static void Publish(string path, string channelID)
		{
			System.Diagnostics.Process.Start("cmd.exe", $"butler push {path} {ItchPublisherID}/{ItchProjectID}:{channelID}");
		}
	}
}
