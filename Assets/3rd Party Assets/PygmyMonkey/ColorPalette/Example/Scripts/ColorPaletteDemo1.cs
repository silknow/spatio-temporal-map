using UnityEngine;

namespace PygmyMonkey.ColorPalette
{
	public class ColorPaletteDemo1 : MonoBehaviour
	{
		void OnGUI()
		{
			if (GUI.Button(new Rect(10, 10, 150, 30), "Use Palette 'Red'"))
			{
				ColorPaletteData.Singleton.setCurrentPalette("01 - Red");
			}
			
			if (GUI.Button(new Rect(10, 50, 150, 30), "Use Palette 'Blue'"))
			{
				ColorPaletteData.Singleton.setCurrentPalette("06 - Blue");
			}
			
			if (GUI.Button(new Rect(10, 90, 150, 30), "Use Palette 'Green'"))
			{
				ColorPaletteData.Singleton.setCurrentPalette("10 - Green");
			}
			
			if (GUI.Button(new Rect(10, 130, 150, 30), "Use Palette at Index 3"))
			{
				ColorPaletteData.Singleton.setCurrentPalette(3);
			}
		}
	}
}