using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace PygmyMonkey.ColorPalette
{
	public static class SVGFileImporter
	{
		public static ColorPalette Import(string filePath)
		{
			ColorPalette colorPalette = new ColorPalette();
			colorPalette.name = Path.GetFileNameWithoutExtension(filePath);

			using (StreamReader reader = new StreamReader(filePath))
			{
				string line = "";

				while ((line = reader.ReadLine()) != null)
				{
					if (line.Contains("fill="))
					{
						ParserColorResult colorResult = extractColor(line);
						if (colorResult.success)
						{
							colorPalette.colorInfoList.Add(new ColorInfo(colorResult.name, colorResult.color));
						}
					}
				}
			}

			if (colorPalette.colorInfoList == null || colorPalette.colorInfoList.Count == 0)
			{
				throw new UnityException("Error parsing the .svg file at path: " + filePath + ". Are you sure you selected a valid file?");
			}

			return colorPalette;
		}

		private static ParserColorResult extractColor(string text)
		{
			try
			{
				int start = text.IndexOf("fill=\"") + 7;
				int end = text.IndexOf("\"", start);

				string colorString = text.Substring(start, end - start);

				return new ParserColorResult(null, ColorUtils.HexToColor(colorString), true);
			}
			catch (System.Exception)
			{
				return new ParserColorResult(null, Color.white, false);
			}
		}
	}
}