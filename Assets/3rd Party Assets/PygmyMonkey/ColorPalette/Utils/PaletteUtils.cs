using UnityEngine;
using System.Collections.Generic;

namespace PygmyMonkey.ColorPalette
{
	public static class PaletteUtils
	{
		#if UNITY_EDITOR
		public static ColorPalette GetRandomPalette(int colorCount)
		{
			ColorPalette colorPalette = new ColorPalette();
			colorPalette.name = "Random";

			for (int i = 0; i < colorCount; i++)
			{
				colorPalette.colorInfoList.Add(new ColorInfo("Random", getRandomColor()));
			}

			return colorPalette;
		}
		
		private static Color getRandomColor()
		{
			return new Color(Random.Range(0, 255)/255f, Random.Range(0, 255)/255f, Random.Range(0, 255)/255f, 255/255f);
		}

		public static ColorPalette GetRandomPastelPalette(int colorCount)
		{
			ColorPalette colorPalette = new ColorPalette();
			colorPalette.name = "Random pastel";
			
			for (int i = 0; i < colorCount; i++)
			{
				colorPalette.colorInfoList.Add(new ColorInfo("Random", getRandomColorFromHue(0.25f)));
			}
			
			return colorPalette;
		}
		
		public static ColorPalette GetRandomVividPalette(int colorCount)
		{
			ColorPalette colorPalette = new ColorPalette();
			colorPalette.name = "Random vivid";
			
			for (int i = 0; i < colorCount; i++)
			{
				colorPalette.colorInfoList.Add(new ColorInfo("Random", getRandomColorFromHue(0.75f)));
			}
			
			return colorPalette;
		}

		private static Color getRandomColorFromHue(float saturation)
		{
			#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			return UnityEditor.EditorGUIUtility.HSVToRGB(Random.Range(0.0f, 1.0f), saturation, 0.95f);
			#else
			return Color.HSVToRGB(Random.Range(0.0f, 1.0f), saturation, 0.95f);
			#endif
		}
		
		public static ColorPalette GetRandomFromColorPalette(Color color, float offset, int colorCount)
		{
			ColorPalette colorPalette = new ColorPalette();
			colorPalette.name = "Random offset";
			
			for (int i = 0; i < colorCount; i++)
			{
				colorPalette.colorInfoList.Add(new ColorInfo("Random", getRandomFromColor(color, offset)));
			}
			
			return colorPalette;
		}
		
		private static Color getRandomFromColor(Color color, float offset)
		{
			Color newColor = new Color();
			newColor.r = color.r + Random.Range(0.0f, 1.0f) * 2 * offset - offset;
			newColor.g = color.g + Random.Range(0.0f, 1.0f) * 2 * offset - offset;
			newColor.b = color.b + Random.Range(0.0f, 1.0f) * 2 * offset - offset;
			newColor.a = color.a;
			return newColor;
		}
		
		public static ColorPalette GetRandomGoldenRatioPalette(float saturation, int colorCount)
		{
			ColorPalette colorPalette = new ColorPalette();
			colorPalette.name = "Random golden ratio";
			
			float goldenRatio = 0.617033988f;
			float hue = Random.Range(0.0f, 1.0f);
			for (int i = 0; i < colorCount; i++)
			{
				hue += goldenRatio;
				hue %= 1;

				#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
				Color color = UnityEditor.EditorGUIUtility.HSVToRGB(hue, saturation, 0.95f);
				#else
				Color color = Color.HSVToRGB(hue, saturation, 0.95f);
				#endif

				colorPalette.colorInfoList.Add(new ColorInfo("Random", color));
			}
			
			return colorPalette;
		}

		public static ColorPalette GetGradientPalette(Color colorFrom, Color colorTo, int colorCount)
		{
			ColorPalette colorPalette = new ColorPalette();
			colorPalette.name = "Gradient";

			colorPalette.colorInfoList.Add(new ColorInfo("Random", colorFrom));

			float rDelta = colorFrom.r - colorTo.r;
			float gDelta = colorFrom.g - colorTo.g;
			float bDelta = colorFrom.b - colorTo.b;
			float aDelta = colorFrom.a - colorTo.a;

			float intermediateColorCount = colorCount - 2;
			for (int i = 0; i < intermediateColorCount; i++)
			{
				float offset = (i + 1) / (intermediateColorCount + 1);

				float rColor = colorFrom.r - offset * rDelta;
				float gColor = colorFrom.g - offset * gDelta;
				float bColor = colorFrom.b - offset * bDelta;
				float aColor = colorFrom.a - offset * aDelta;

				colorPalette.colorInfoList.Add(new ColorInfo("Random", new Color(rColor, gColor, bColor, aColor)));
			}

			colorPalette.colorInfoList.Add(new ColorInfo("Random", colorTo));

			return colorPalette;
		}
		#endif
		
		public static void UpdatePaletteObjectsInCurrentScene()
		{
			ColorPaletteObject[] colorPaletteObjectArray = UnityEngine.Object.FindObjectsOfType<ColorPaletteObject>() ;
			foreach (ColorPaletteObject colorPaletteObject in colorPaletteObjectArray)
			{
				colorPaletteObject.updateColor();
			}

			#if UNITY_EDITOR
			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
			#endif
		}

		public static void SavePalettes(ColorPaletteData paletteData)
		{
			#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(paletteData);
			UnityEditor.AssetDatabase.SaveAssets();
			UnityEditor.AssetDatabase.Refresh();
			#endif
		}
	}
}