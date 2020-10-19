using System;
using UnityEngine;

namespace PygmyMonkey.ColorPalette
{
	[Serializable]
	public class ColorInfo
	{
		public string name;
		public Color color;

		public ColorInfo()
		{
		}

		public ColorInfo(string name, Color color)
		{
			this.name = name;
			this.color = color;
		}

		public ColorInfo Copy()
		{
			ColorInfo colorInfo = new ColorInfo();
			colorInfo.name = name;
			colorInfo.color = color;
			return colorInfo;
		}
		
		public override string ToString()
		{
			string theName = name;
			if (string.IsNullOrEmpty(theName))
			{
				theName = "N/A";
			}
			
			string theHexColor = "#" + ColorUtils.ColorToHex(color);
			string theColorDetail = "R: " + color.r.ToString("0.00") + " - G: " + color.g.ToString("0.00") + " - B: " + color.b.ToString("0.00") + " - A: " + color.a.ToString("0.00");
			
			return theName + "\n" + theHexColor + "\n" + theColorDetail;
		}
	}
}
