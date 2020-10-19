using UnityEngine;
using UnityEngine.UI;

namespace PygmyMonkey.ColorPalette
{
	public class ColorPaletteDemo3 : MonoBehaviour
	{
		[SerializeField] private ColorPaletteObject[] randomObjectArray1 = null;
		[SerializeField] private ColorPaletteObject[] randomObjectArray2 = null;
		[SerializeField] private ColorPaletteObject[] randomObjectArray3 = null;
		[SerializeField] private Text smileyText1 = null;
		[SerializeField] private Text smileyText2 = null;
		[SerializeField] private Text smileyText3 = null;
		[SerializeField] private ColorPaletteObject[] smiley4 = null;
		
		private int smileyPaletteIndex;
		
		void Start()
		{
			onButtonRandomClicked();
			
			// Set smiley palette to the smiley
			smileyPaletteIndex = ColorPaletteData.Singleton.getPaletteIndexFromName("Smiley");
			for (int i = 0; i < smiley4.Length; i++)
			{
				smiley4[i].setCustomPalette(smileyPaletteIndex);
			}
		}

		public void onButtonRandomClicked()
		{
			int randomIndex1 = Random.Range(1, ColorPaletteData.Singleton.colorPaletteList.Count);
			smileyText1.text = "Random Palette #" + randomIndex1;
			foreach (ColorPaletteObject colorPaletteObject in randomObjectArray1)
			{
				colorPaletteObject.setCustomPalette(randomIndex1);
			}

			int randomIndex2 = Random.Range(1, ColorPaletteData.Singleton.colorPaletteList.Count);
			smileyText2.text = "Random Palette #" + randomIndex2;
			foreach (ColorPaletteObject colorPaletteObject in randomObjectArray2)
			{
				colorPaletteObject.setCustomPalette(randomIndex2);
			}

			int randomIndex3 = Random.Range(1, ColorPaletteData.Singleton.colorPaletteList.Count);
			smileyText3.text = "Random Palette #" + randomIndex3;
			foreach (ColorPaletteObject colorPaletteObject in randomObjectArray3)
			{
				colorPaletteObject.setCustomPalette(randomIndex3);
			}
		}
		
		public void onButtonInfoClicked()
		{
			ColorPalette colorPalette = ColorPaletteData.Singleton.colorPaletteList[smileyPaletteIndex];
			
			Debug.Log("Palette name: " + colorPalette.name);
			for (int i = 0; i < colorPalette.colorInfoList.Count; i++)
			{
				ColorInfo colorInfo = colorPalette.colorInfoList[i];
				Debug.Log("Color: " + colorInfo.name + " - " + colorInfo.color);
			}
		}
		
		public void onButtonPrintColorNameClicked()
		{
			ColorPalette colorPalette = ColorPaletteData.Singleton.colorPaletteList[smileyPaletteIndex];
			ColorInfo colorInfo = colorPalette.getColorFromName("Mouth"); 
			Debug.Log(colorInfo);
		}
		
		public void onButtonPrintColorIndexClicked()
		{
			ColorPalette colorPalette = ColorPaletteData.Singleton.colorPaletteList[smileyPaletteIndex];
			ColorInfo colorInfo = colorPalette.colorInfoList[0]; 
			Debug.Log(colorInfo);
		}
	}
}