using UnityEngine;
using UnityEngine.Networking;
using System;

namespace PygmyMonkey.ColorPalette
{
	public static class ColrdWebsiteImporter
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
					if (line.Contains("<h1>") && line.Contains("</h1>"))
					{
						colorPalette.name = line.Substring(line.IndexOf("h1>") + 3);
						colorPalette.name = colorPalette.name.Substring(0, colorPalette.name.IndexOf("</h1>"));
					}

					if (line.Contains("span class=\"gpTitle\"") && !line.Contains("a href"))
					{
						string[] resultArray = System.Text.RegularExpressions.Regex.Split(line, "background-color");
						for (int i = 0; i < resultArray.Length; i++)
						{
							if (!resultArray[i].Contains("opacity"))
							{
								continue;
							}

							string rgb = resultArray[i].Substring(resultArray[i].IndexOf("(") + 1);
							rgb = rgb.Substring(0, rgb.IndexOf(")"));
							string opacity = resultArray[i].Substring(resultArray[i].IndexOf("opacity=") + 8);
							opacity = opacity.Substring(0, opacity.LastIndexOf(");"));

							string[] rgbArray = rgb.Split(',');

							colorPalette.colorInfoList.Add(new ColorInfo(null, new Color(int.Parse(rgbArray[0]) / 255f, int.Parse(rgbArray[1]) / 255f, int.Parse(rgbArray[2]) / 255f, int.Parse(opacity) / 100f)));
						}
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