using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace PygmyMonkey.ColorPalette
{
	public static class GPLFileImporter
	{
		public static ColorPalette Import(string filePath)
		{
			ColorPalette colorPalette = new ColorPalette();
			colorPalette.name = Path.GetFileNameWithoutExtension(filePath);

			bool nextLineIsColors = false;
			using (StreamReader reader = new StreamReader(filePath))
			{
				string line = "";

				while ((line = reader.ReadLine()) != null)
				{
					if (line.StartsWith("Name:"))
					{
						colorPalette.name = line.Replace("Name: ", string.Empty).Trim();
					}
					
					if (nextLineIsColors)
					{
						ParserColorResult colorResult = extractColor(line);
						if (colorResult.success)
						{
							colorPalette.colorInfoList.Add(new ColorInfo(colorResult.name, colorResult.color));
						}
					}

					if (line.Trim().Equals("#"))
					{
						nextLineIsColors = true;
					}
				}
			}

			if (colorPalette.colorInfoList == null || colorPalette.colorInfoList.Count == 0)
			{
				throw new UnityException("Error parsing the .gpl file at path: " + filePath + ". Are you sure you selected a valid file?");
			}

			return colorPalette;
		}

		private static ParserColorResult extractColor(string text)
		{
			int[] rgbArray = new int[3];
			string name = null;

			int start = int.MinValue;
			int res;
			int pos = 0;
			for (int i = 0; i < text.Length; i++)
			{
				if (start == int.MinValue && int.TryParse(text[i].ToString(), out res))
				{
					start = i;
				}

				if (start > int.MinValue && !int.TryParse(text[i].ToString(), out res))
				{
					rgbArray[pos++] = int.Parse(text.Substring(start, i - start));
					start = int.MinValue;
				}

				if (pos >= rgbArray.Length)
				{
					name = text.Substring(i).Trim();
					break;
				}
			}

			return new ParserColorResult(name, new Color(rgbArray[0]/255.0f, rgbArray[1]/255.0f, rgbArray[2]/255.0f), true);
		}
	}
}