using UnityEngine;
using UnityEngine.Networking;
using System;

namespace PygmyMonkey.ColorPalette
{
	public static class DribbbleWebsiteImporter
	{
		public static void Import(Uri uri, Action<ColorPalette> onDone, Action<string> onFail)
		{
			UnityWebRequest webRequest = UnityWebRequest.Get(uri);

			UnityWebRequestAsyncOperation asyncOperation = webRequest.SendWebRequest();
			asyncOperation.completed += (operation) =>
			{
				if (webRequest.isNetworkError || webRequest.isHttpError)
				{
					onFail("Error getting palette from the website: " + uri.AbsoluteUri + "\nPlease contact us at tools@pygmymonkey.com\n" + webRequest.error);
					return;
				}

				ColorPalette colorPalette = new ColorPalette();
				colorPalette.name = uri.AbsolutePath;

				foreach (string line in webRequest.downloadHandler.text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
				{
					if (line.Contains("screenshot-title") && line.Contains("h1"))
					{
						colorPalette.name = line.Substring(line.IndexOf(">") + 1, line.IndexOf("<", line.IndexOf(">")) - line.IndexOf(">") - 1);
					}

					if (line.Contains("<a style=\"background-color:"))
					{
						string hexColorString = line.Substring(line.IndexOf("#") + 1, 6);
						colorPalette.colorInfoList.Add(new ColorInfo(null, ColorUtils.HexToColor(hexColorString)));
					}
				}

				if (colorPalette.colorInfoList == null || colorPalette.colorInfoList.Count == 0)
				{
					onFail("Error getting palette from the website: " + uri.AbsoluteUri + "\nPlease contact us at tools@pygmymonkey.com");
					return;
				}

				onDone(colorPalette);
			};
		}
	}
}