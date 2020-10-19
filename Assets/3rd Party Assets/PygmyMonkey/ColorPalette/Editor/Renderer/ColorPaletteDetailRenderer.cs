using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PygmyMonkey.ColorPalette.Utils;

namespace PygmyMonkey.ColorPalette
{
	public class ColorPaletteDetailRenderer
	{
		public void drawInspector(ColorPalette colorPalette, ColorPaletteData colorPaletteData)
		{
			colorPalette.name = EditorGUILayout.TextField("Palette name", colorPalette.name);

			EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.Space();

				for (int i = 0; i < colorPalette.colorInfoList.Count; i++)
				{
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.LabelField("#" + ColorUtils.ColorToHex(colorPalette.colorInfoList[i].color), GUILayout.Width(55f));
						colorPalette.colorInfoList[i].name = EditorGUILayout.TextField(colorPalette.colorInfoList[i].name);
						
						colorPalette.colorInfoList[i].color = EditorGUILayout.ColorField(colorPalette.colorInfoList[i].color);

						GUI.changed = false;

						if (GUILayout.Button(new GUIContent("+", "duplicate"), EditorStyles.miniButtonLeft, GUILayout.Width(20f)))
						{
							colorPalette.colorInfoList.Insert(i + 1, colorPalette.colorInfoList[i].Copy());
							GUI.FocusControl(null);
							PaletteUtils.SavePalettes(colorPaletteData);
							return;
						}
						
						GUI.enabled = i > 0;
						if (GUILayout.Button(new GUIContent("\u2191", "move up"), EditorStyles.miniButtonMid, GUILayout.Width(20f)))
						{
							ColorInfo tmpColor = colorPalette.colorInfoList[i];
							colorPalette.colorInfoList.RemoveAt(i);
							colorPalette.colorInfoList.Insert(i - 1, tmpColor);
							GUI.FocusControl(null);
						}
						GUI.enabled = true;
						
						GUI.enabled = i < colorPalette.colorInfoList.Count - 1;
						if (GUILayout.Button(new GUIContent("\u2193", "move down"), EditorStyles.miniButtonMid, GUILayout.Width(20f)))
						{
							ColorInfo tmpColor = colorPalette.colorInfoList[i];
							colorPalette.colorInfoList.RemoveAt(i);
							colorPalette.colorInfoList.Insert(i + 1, tmpColor);
							GUI.FocusControl(null);
						}
						GUI.enabled = true;

						GUI.enabled = colorPalette.colorInfoList.Count > 1;
						if (GUILayout.Button(new GUIContent("-", "delete"), EditorStyles.miniButtonRight, GUILayout.Width(20f)))
						{
							colorPalette.colorInfoList.RemoveAt(i);
							GUI.FocusControl(null);
							PaletteUtils.SavePalettes(colorPaletteData);
							return;
						}
						GUI.enabled = true;

						if (GUI.changed)
						{
							PaletteUtils.SavePalettes(colorPaletteData);
						}
					}
					EditorGUILayout.EndHorizontal();
				}

				EditorGUILayout.Space();
			}
			EditorGUILayout.EndVertical();
		}
	}
}
