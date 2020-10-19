using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace PygmyMonkey.ColorPalette
{
	public static class ColorPresetFileImporter
	{
		public static ColorPalette Import(string filePath)
		{
			ColorPalette colorPalette = new ColorPalette();
			colorPalette.name = Path.GetFileNameWithoutExtension(filePath);

			using (StreamReader reader = new StreamReader(filePath))
			{
				string line = "";
				ColorInfo colorInfo = new ColorInfo();

				while ((line = reader.ReadLine()) != null)
				{
					if (line.StartsWith("  m_Name:") && string.IsNullOrEmpty(colorPalette.name))
					{
						line = line.Replace("m_Name:", string.Empty).Trim();
						colorPalette.name = line;
					}
					
					if (line.StartsWith("  - m_Name:"))
					{
						colorInfo = new ColorInfo();
						
						line = line.Replace("- m_Name:", string.Empty).Trim();
						colorInfo.name = line;
					}

					if (line.StartsWith("    m_Color:"))
					{
						line = line.Replace("m_Color:", string.Empty).Trim();
						colorInfo.color = extractColor(line);
						
						colorPalette.colorInfoList.Add(colorInfo);
					}
				}
			}

			if (colorPalette.colorInfoList == null || colorPalette.colorInfoList.Count == 0)
			{
				throw new UnityException("Error parsing the color preset file at path: " + filePath + ". Are you sure you selected a valid file?");
			}

			return colorPalette;
		}

		private static string extractColorString(string text, string colorLetter) { return extractColorString(text, colorLetter, false); }
		private static string extractColorString(string text, string colorLetter, bool isFinal)
		{
			int start = text.IndexOf(colorLetter + ":") + 2;
			int end = text.IndexOf(isFinal ? "}" : ",", start);

			return text.Substring(start, end - start);
		}

		private static Color extractColor(string text)
		{
			return new Color(float.Parse(extractColorString(text, "r")), float.Parse(extractColorString(text, "g")), float.Parse(extractColorString(text, "b")), float.Parse(extractColorString(text, "a", true)));
		}
	}
}