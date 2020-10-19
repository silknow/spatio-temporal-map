using UnityEditor;
using UnityEngine;
using PygmyMonkey.ColorPalette.Utils;

namespace PygmyMonkey.ColorPalette
{
	public class ColorPaletteCreateRenderer
	{
		private ColorPaletteData m_paletteData;

		private ColorPalette m_generatedColorPalette;
		private int m_colorCount = 10;
		private Color m_colorReference = Color.red;
		private float m_colorOffset = 0.5f;
		private float m_saturation = 0.8f;
		private Color m_colorGradient1 = Color.red;
		private Color m_colorGradient2 = Color.blue;
		private Algorithm m_selectedAlgorithm;
		private enum Algorithm
		{
			RANDOM,
			RANDOM_PASTEL,
			RANDOM_VIVID,
			RANDOM_FROM_COLOR,
			RANDOM_GOLDEN_RATIO,
			GRADIENT,
		}

		public ColorPaletteCreateRenderer(ColorPaletteData paletteData)
		{
			m_paletteData = paletteData;
		}
		
		public void drawInspector()
		{
			if (GUIUtils.DrawHeader("Create palette"))
			{
				GUIUtils.BeginContents();
				{
					EditorGUILayout.BeginHorizontal();
					{
						m_selectedAlgorithm = (Algorithm)EditorGUILayout.EnumPopup(m_selectedAlgorithm);
						if (GUILayout.Button("Generate", EditorStyles.miniButton))
						{
							GUI.FocusControl(null);
							generatePalette();
						}
					}
					EditorGUILayout.EndHorizontal();

					m_colorCount = EditorGUILayout.IntSlider("Number of colors", m_colorCount, 1, 25);

					if (m_selectedAlgorithm == Algorithm.RANDOM_FROM_COLOR)
					{
						m_colorReference = EditorGUILayout.ColorField("Base color", m_colorReference);
						m_colorOffset = EditorGUILayout.FloatField("Color offset", m_colorOffset);
					}
					else if (m_selectedAlgorithm == Algorithm.RANDOM_GOLDEN_RATIO)
					{
						m_saturation = EditorGUILayout.Slider("Saturation", m_saturation, 0.0f, 1.0f);
					}
					else if (m_selectedAlgorithm == Algorithm.GRADIENT)
					{
						m_colorGradient1 = EditorGUILayout.ColorField("From color", m_colorGradient1);
						m_colorGradient2 = EditorGUILayout.ColorField("To color", m_colorGradient2);
					}

					if (m_generatedColorPalette != null)
					{
						if (GUI.changed)
						{
							generatePalette();
						}

						GUILayout.Space(3f);

						EditorGUILayout.BeginHorizontal();
						{
							for (int j = 0; j < m_generatedColorPalette.colorInfoList.Count; j++)
							{
								Rect rect = EditorGUILayout.GetControlRect(false, 20f, EditorStyles.colorField, GUILayout.Width(20f));
								EditorGUIUtility.DrawColorSwatch(rect, m_generatedColorPalette.colorInfoList[j].color);
							}
						}
						EditorGUILayout.EndHorizontal();

						GUILayout.Space(3f);

						if (GUILayout.Button("Add to my palettes"))
						{
							m_paletteData.colorPaletteList.Add(m_generatedColorPalette);
							m_generatedColorPalette = null;

							PaletteUtils.SavePalettes(m_paletteData);
						}
					}
				}
				GUIUtils.EndContents();
			}
		}

		private void generatePalette()
		{
			switch (m_selectedAlgorithm)
			{
			case Algorithm.RANDOM:
				m_generatedColorPalette = PaletteUtils.GetRandomPalette(m_colorCount);
				break;

			case Algorithm.RANDOM_PASTEL:
				m_generatedColorPalette = PaletteUtils.GetRandomPastelPalette(m_colorCount);
				break;

			case Algorithm.RANDOM_VIVID:
				m_generatedColorPalette = PaletteUtils.GetRandomVividPalette(m_colorCount);
				break;
				
			case Algorithm.RANDOM_FROM_COLOR:
				m_generatedColorPalette = PaletteUtils.GetRandomFromColorPalette(m_colorReference, m_colorOffset, m_colorCount);
				break;
				
			case Algorithm.RANDOM_GOLDEN_RATIO:
				m_generatedColorPalette = PaletteUtils.GetRandomGoldenRatioPalette(m_saturation, m_colorCount);
				break;
				
			case Algorithm.GRADIENT:
				m_generatedColorPalette = PaletteUtils.GetGradientPalette(m_colorGradient1, m_colorGradient2, m_colorCount);
				break;
			}
		}
	}
}
