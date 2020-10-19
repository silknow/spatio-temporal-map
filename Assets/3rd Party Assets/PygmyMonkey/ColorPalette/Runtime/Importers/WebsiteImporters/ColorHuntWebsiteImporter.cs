using UnityEngine.Networking;
using System;

namespace PygmyMonkey.ColorPalette
{
	public static class ColorHuntWebsiteImporter
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
					if (line.StartsWith("itemer("))
					{
						string[] data = line.Split(new string[] { "'," }, StringSplitOptions.RemoveEmptyEntries);
						colorPalette.name = data[1].Trim();
                        if (colorPalette.name.StartsWith("'"))
                        {
							colorPalette.name = colorPalette.name.Substring(1);
                        }
						UnityEngine.Debug.Log(colorPalette.name);

						string hexColorCombined = data[2].Trim();
						if (hexColorCombined.StartsWith("'"))
						{
							hexColorCombined = hexColorCombined.Substring(1);
						}

                        for (int i = 0; i < hexColorCombined.Length; i += 6)
                        {
							string hexColor = hexColorCombined.Substring(i, Math.Min(6, hexColorCombined.Length - i));

                            if (hexColor.Length == 6)
                            {
								colorPalette.colorInfoList.Add(new ColorInfo(null, ColorUtils.HexToColor(hexColor)));
							}
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