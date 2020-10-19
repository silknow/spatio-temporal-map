using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PygmyMonkey.ColorPalette
{
	[Serializable]
	public class ColorPalette
	{
		public string name;
		public bool showDetails = false;
		public List<ColorInfo> colorInfoList = new List<ColorInfo>();
		
		[Obsolete("colorList is now obsolete, please use colorInfoList instead")]
		public List<Color> colorList = new List<Color>();

		public ColorPalette()
		{
			
		}

		public ColorPalette(string name, List<ColorInfo> colorInfoListInput)
		{
			this.name = name;

			foreach (ColorInfo colorInfo in colorInfoListInput)
			{
				Color newColor = new Color(colorInfo.color.r / 255.0f, colorInfo.color.g / 255.0f, colorInfo.color.b / 255.0f, colorInfo.color.a / 255.0f);
				colorInfoList.Add(new ColorInfo(colorInfo.name, newColor));
			}
		}
		
		public ColorPalette Copy()
		{
			ColorPalette colorPalette = new ColorPalette();
			colorPalette.name = name;
			colorPalette.colorInfoList = new List<ColorInfo>();
			for (int i = 0; i < colorInfoList.Count; i++)
			{
				colorPalette.colorInfoList.Add(colorInfoList[i].Copy());
			}

			return colorPalette;
		}
		
		public ColorInfo getColorFromName(string colorName)
		{
			ColorInfo colorInfo = colorInfoList.Where(x => x.name.Equals(colorName)).FirstOrDefault();
			
			if (colorInfo == null)
			{
				throw new Exception("Could not find a color named '" + colorName + "' in the palette '" + name + "'");
			}
			 
			return colorInfo;
		}

		public ColorInfo getRandomColorInfo()
		{
			int randomIndex = UnityEngine.Random.Range(0, colorInfoList.Count);
			return colorInfoList[randomIndex];
		}

		public Color getRandomColor()
		{
			return getRandomColorInfo().color;
		}
		
		#pragma warning disable 0618
		public void restoreFromOldVersion()
		{
			if (colorList != null && colorList.Count > 0)
			{
				colorInfoList.Clear();
				
				for (int i = 0; i < colorList.Count; i++)
				{
					colorInfoList.Add(new ColorInfo(null, colorList[i]));
				}
				
				colorList.Clear();
			}
			
			colorList = null;
		}
		#pragma warning restore 0618
	}
}
