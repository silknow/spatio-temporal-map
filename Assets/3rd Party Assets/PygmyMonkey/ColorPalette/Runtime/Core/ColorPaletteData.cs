using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PygmyMonkey.ColorPalette
{
	[Serializable]
	public class ColorPaletteData : ScriptableObject
	{
		private const string m_prefCurrentColorPaletteIndex = "PREF_CURRENT_COLOR_PALETTE_INDEX";

		// The current selected palette index
		[SerializeField] private int m_currentPaletteIndex;
		public int currentPaletteIndex
		{
			get { return m_currentPaletteIndex; }
			set
			{
				m_currentPaletteIndex = value;

				PlayerPrefs.SetInt(m_prefCurrentColorPaletteIndex, m_currentPaletteIndex);
				PaletteUtils.SavePalettes(this);

				if (OnCurrentPaletteChanged != null)
				{
					OnCurrentPaletteChanged();
				}
			}
		}
		
		// The list of all the color palettes
		public List<ColorPalette> colorPaletteList;
		
		public delegate void CurrentPaletteHandler();
		public static event CurrentPaletteHandler OnCurrentPaletteChanged;

		private static ColorPaletteData mInstance;
		public static ColorPaletteData Singleton
		{
			get
			{
				if (mInstance == null)
				{
					mInstance = (ColorPaletteData)Resources.Load("ColorPaletteData", typeof(ColorPaletteData));
					if (mInstance == null)
					{
						throw new Exception("We could not find the ScriptableObject ColorPaletteData.asset inside the folder 'PygmyMonkey/ColorPalette/Resources/'");
					}

					mInstance.init();
				}
				
				return mInstance;
			}
		}

		public void init()
		{
			if (colorPaletteList != null)
			{
				foreach (ColorPalette colorPalette in colorPaletteList)
				{
					colorPalette.restoreFromOldVersion();
				}
				
				PaletteUtils.SavePalettes(this);
			}
			
			if (colorPaletteList == null || colorPaletteList.Count == 0)
			{
				restoreDefaultPalettes();
			}
			
			m_currentPaletteIndex = PlayerPrefs.GetInt(m_prefCurrentColorPaletteIndex, 0);
		}
		
		public void restoreDefaultPalettes()
		{
			colorPaletteList = PaletteFirstUseUtils.GetDefaultPalette();
		}

		public void clearPalettes()
		{
			colorPaletteList.RemoveRange(1, colorPaletteList.Count - 1);
		}

		public ColorPalette getCurrentPalette()
		{
			return colorPaletteList[currentPaletteIndex];
		}

		public void setCurrentPalette(string name)
		{
			setCurrentPalette(getPaletteIndexFromName(name));
		}

		public void setCurrentPalette(int index)
		{
			currentPaletteIndex = index;
			PaletteUtils.UpdatePaletteObjectsInCurrentScene();
		}

		public ColorPalette getRandomPalette()
		{
			return colorPaletteList[UnityEngine.Random.Range(0, colorPaletteList.Count)];
		}

		public void setRandomCurrentPalette()
		{
			setCurrentPalette(UnityEngine.Random.Range(0, colorPaletteList.Count));
		}
		
		public string[] getColorPaletteNameArray()
		{
			return colorPaletteList.Select(x => x.name).ToArray();
		}
		
		public ColorPalette fromName(string name)
		{
			return colorPaletteList.Where(x => x.name.Equals(name)).FirstOrDefault();
		}

		public int getPaletteIndexFromName(string name)
		{
			for (int i = 0; i < colorPaletteList.Count; i++)
			{
				if (colorPaletteList[i].name.Equals(name))
				{
					return i;
				}
			}
			
			throw new UnityException("Could not found a palette with the name: " + name);
		}
	}
}
