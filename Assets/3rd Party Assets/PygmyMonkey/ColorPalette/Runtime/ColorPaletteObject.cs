using UnityEngine;
using UnityEngine.UI;

namespace PygmyMonkey.ColorPalette
{
	[AddComponentMenu("PygmyMonkey/ColorPalette Object")]
	public class ColorPaletteObject : MonoBehaviour
	{
		public int colorIndex;
		public React react = React.CURRENT_PALETTE;
		public int customPaletteIndex;
		public bool overrideAlpha = false;
		public float alpha = 1.0f;

		public enum React
		{
			NONE,
			CURRENT_PALETTE,
			CUSTOM_PALETTE,
		}

		void Start()
		{
			updateColor();
		}

		void OnEnable()
		{
			onPaletteChanged();
			if (react == React.CURRENT_PALETTE)
			{
				ColorPaletteData.OnCurrentPaletteChanged += onPaletteChanged;
			}
		}

		void OnDisable()
		{
			if (react == React.CURRENT_PALETTE)
			{
				ColorPaletteData.OnCurrentPaletteChanged -= onPaletteChanged;
			}
		}

		private void onPaletteChanged()
		{
			updateColor();
		}

		public void setCustomPalette(int index)
		{
			customPaletteIndex = index;
			updateColor();
		}

		public void setCustomPalette(string paletteName)
		{
			customPaletteIndex = ColorPaletteData.Singleton.getPaletteIndexFromName(paletteName);
			updateColor();
		}

		public void updateColor()
		{
			Color color = getColor();

			if (!color.Equals(default(Color))) // Color black (opacity 0) will not update
			{
				SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
				Image imageComponent = this.GetComponent<Image>();
				Text textComponent = this.GetComponent<Text>();

				if (spriteRenderer != null)
				{
					if (color != spriteRenderer.color)
					{
						spriteRenderer.color = color;
						setDirty();
					}
				}
				else if (imageComponent != null)
				{
					if (color != imageComponent.color)
					{
						imageComponent.color = color;
						setDirty();
					}
				}
				else if (textComponent != null)
				{
					if (color != textComponent.color)
					{
						textComponent.color = color;
						setDirty();
					}
				}
				else
				{
					Renderer renderer = this.GetComponent<Renderer>();
					if (renderer != null)
					{
						if (color != renderer.sharedMaterial.color)
						{
							renderer.sharedMaterial.color = color;
							setDirty();
						}
					}
					else
					{
						Debug.LogWarning("There is no compatible component with a color attached to this GameObject.", this.gameObject);
					}
				}
			}
		}

		private void setDirty()
		{
			#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(this.gameObject);
			#endif
		}

		public Color getColor()
		{
			Color color = default(Color);
			if (react == React.NONE)
			{
				return color;
			}

			ColorPalette colorPalette = getColorPalette();

			int finalColorIndex = colorIndex;
			if (finalColorIndex < 0)
			{
				finalColorIndex = 0;
			}
			else if (finalColorIndex > colorPalette.colorInfoList.Count - 1)
			{
				finalColorIndex = colorPalette.colorInfoList.Count - 1;
			}

			color = colorPalette.colorInfoList[finalColorIndex].color;

			if (overrideAlpha)
			{
				color = new Color(color.r, color.g, color.b, alpha);
			}

			return color;
		}

		public ColorPalette getColorPalette()
		{
			ColorPalette colorPalette = ColorPaletteData.Singleton.getCurrentPalette();

			if (react == React.CUSTOM_PALETTE)
			{
				if (customPaletteIndex > ColorPaletteData.Singleton.colorPaletteList.Count - 1)
				{
					customPaletteIndex = ColorPaletteData.Singleton.colorPaletteList.Count - 1;
				}

				colorPalette = ColorPaletteData.Singleton.colorPaletteList[customPaletteIndex];
			}

			return colorPalette;
		}

		private int getColorIndex(float colorPercentage, int length)
		{
			int index = (int)(colorPercentage * length);
			return Mathf.Clamp(index, 0, length - 1);
		}
	}
}