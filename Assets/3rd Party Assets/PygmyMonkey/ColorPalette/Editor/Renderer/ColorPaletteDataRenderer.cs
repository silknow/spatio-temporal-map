using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PygmyMonkey.ColorPalette.Utils;

namespace PygmyMonkey.ColorPalette
{
	public class ColorPaletteDataRenderer
	{
		private ColorPaletteData m_paletteData;
		private ColorPaletteDetailRenderer m_colorPaletteDetailRenderer;

		public ColorPaletteDataRenderer(ColorPaletteData paletteData)
		{
			m_paletteData = paletteData;
			m_colorPaletteDetailRenderer = new ColorPaletteDetailRenderer();
		}
		
		public void drawInspector()
		{
			if (m_paletteData.colorPaletteList == null || m_paletteData.colorPaletteList.Count == 0)
			{
				m_paletteData.restoreDefaultPalettes();
			}

			for (int i = 0; i < m_paletteData.colorPaletteList.Count; i++)
			{
				ColorPalette colorPalette = m_paletteData.colorPaletteList[i];

				GUI.backgroundColor = ColorPaletteData.Singleton.currentPaletteIndex == i ? Color.green : Color.white;
				if (GUIUtils.DrawHeader(colorPalette.name))
				{
					GUI.backgroundColor = Color.white;
					GUIUtils.BeginContents();
					{
						if (!colorPalette.showDetails)
						{
							EditorGUILayout.BeginHorizontal();
							{
								GUILayout.Space(4f);
								for (int j = 0; j < colorPalette.colorInfoList.Count; j++)
								{
									ColorPaletteObjectEditor.DrawCustomSwatch(colorPalette.colorInfoList[j], 20);
								}
							}
							EditorGUILayout.EndHorizontal();
						}
						else
						{
							m_colorPaletteDetailRenderer.drawInspector(colorPalette, m_paletteData);
						}

						GUILayout.Space(5f);

						EditorGUILayout.BeginHorizontal();
						{
							if (GUILayout.Button(new GUIContent("Set current", "Apply this palette to all the ColorPaletteObject objects in the current scene"), EditorStyles.miniButton, GUILayout.Width(65f)))
							{
								ColorPaletteData.Singleton.setCurrentPalette(i);
								GUI.FocusControl(null);
							}
							
							/*if (GUILayout.Button(new GUIContent("Apply to color picker", "Use this palette for the color picker"), EditorStyles.miniButton, GUILayout.Width(115f)))
							{
								//TODO: Add this palette to the color picker
								System.Type colorPickerType = System.Type.GetType("UnityEditor.ColorPicker,UnityEditor");
								ScriptableObject colorPickerInstance = ScriptableObject.CreateInstance(colorPickerType);

								// Call the Show method of ColorPicker
								System.Reflection.MethodInfo method = colorPickerType.GetMethod("Show", new System.Type[] { System.Type.GetType("UnityEditor.GUIView,UnityEditor"), typeof(Color)});
								method.Invoke(null, new object[] { null, Color.white });

								// Set the new palette path
								System.String palettePath = "Assets/PygmyMonkey/ColorPalette/Example/Editor/Color Presets/20 - Text and Detail"; //TODO: Find a way to save that as a real preset and use this new file as the path
								System.Reflection.PropertyInfo property = colorPickerType.GetProperty("currentPresetLibrary");
								//Debug.Log (property.GetValue(colorPickerInstance, null));
								property.SetValue(colorPickerInstance, System.Convert.ChangeType(palettePath, property.PropertyType), null);

								GUI.FocusControl(null);
							}*/

							if (GUILayout.Button(new GUIContent(colorPalette.showDetails ? "Hide details" : "Show details", "Edit the palette"), EditorStyles.miniButton, GUILayout.Width(75f)))
							{
								colorPalette.showDetails = !colorPalette.showDetails;
								GUI.FocusControl(null);

								PaletteUtils.SavePalettes(m_paletteData);
							}

							EditorGUILayout.Space();

							GUI.changed = false;

							if (GUILayout.Button(new GUIContent("+", "duplicate"), EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
							{
								m_paletteData.colorPaletteList.Insert(i + 1, colorPalette.Copy());

								if (m_paletteData.currentPaletteIndex > i)
								{
									m_paletteData.currentPaletteIndex++;
								}

								GUI.FocusControl(null);
								PaletteUtils.SavePalettes(m_paletteData);
								return;
							}

							GUI.enabled = i > 0;
							if (GUILayout.Button(new GUIContent("\u2191", "move up"), EditorStyles.miniButtonMid, GUILayout.Width(20f)))
							{
								m_paletteData.colorPaletteList.Remove(colorPalette);
								m_paletteData.colorPaletteList.Insert(i - 1, colorPalette);

								if (m_paletteData.currentPaletteIndex == i)
								{
									m_paletteData.currentPaletteIndex--;
								}
								else if (m_paletteData.currentPaletteIndex == i - 1)
								{
									m_paletteData.currentPaletteIndex++;
								}

								GUI.FocusControl(null);
							}
							GUI.enabled = true;

							GUI.enabled = i < m_paletteData.colorPaletteList.Count - 1;
							if (GUILayout.Button(new GUIContent("\u2193", "move down"), EditorStyles.miniButtonMid, GUILayout.Width(20f)))
							{
								m_paletteData.colorPaletteList.Remove(colorPalette);
								m_paletteData.colorPaletteList.Insert(i + 1, colorPalette);

								if (m_paletteData.currentPaletteIndex == i)
								{
									m_paletteData.currentPaletteIndex++;
								}
								else if (m_paletteData.currentPaletteIndex == i + 1)
								{
									m_paletteData.currentPaletteIndex--;
								}

								GUI.FocusControl(null);
							}
							GUI.enabled = true;

							GUI.enabled = m_paletteData.colorPaletteList.Count > 1;
							if (GUILayout.Button(new GUIContent("-", "delete"), EditorStyles.miniButtonRight, GUILayout.Width(20f)))
							{
								m_paletteData.colorPaletteList.Remove(colorPalette);

								if (m_paletteData.currentPaletteIndex > i)
								{
									m_paletteData.currentPaletteIndex--;
								}
								else if (m_paletteData.currentPaletteIndex == i)
								{
									m_paletteData.currentPaletteIndex = 0;
								}

								GUI.FocusControl(null);
								PaletteUtils.SavePalettes(m_paletteData);
								return;
							}
							GUI.enabled = true;
						}
						EditorGUILayout.EndHorizontal();
					}
					GUIUtils.EndContents();

					if (GUI.changed)
					{
						PaletteUtils.SavePalettes(m_paletteData);
					}
				}

				EditorGUILayout.Space();
			}
		}
	}
}
