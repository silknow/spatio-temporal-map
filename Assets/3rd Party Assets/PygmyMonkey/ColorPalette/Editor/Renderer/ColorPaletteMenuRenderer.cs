using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PygmyMonkey.ColorPalette.Utils;

namespace PygmyMonkey.ColorPalette
{
	public class ColorPaletteMenuRenderer
	{
		private ColorPaletteData m_paletteData;

		public ColorPaletteMenuRenderer(ColorPaletteData paletteData)
		{
			m_paletteData = paletteData;
		}

		public void drawInspector()
		{
			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			{
				if (GUILayout.Button("Palettes", EditorStyles.toolbarPopup, GUILayout.MinWidth(50f)))
				{
					GenericMenu menu = new GenericMenu();
					menu.AddItem(new GUIContent("Clear palettes"), false, () => clearPalettes());
					menu.AddItem(new GUIContent("Restore default palettes"), false, () => restoreDefaultPalettes());
					menu.ShowAsContext();
				}
				
				if (GUILayout.Button(new GUIContent("Update scene", "Update Color Palette Objects in scene"), EditorStyles.toolbarButton, GUILayout.MinWidth(50f)))
				{
					PaletteUtils.UpdatePaletteObjectsInCurrentScene();
				}
				
				/*if (GUILayout.Button("View mode", EditorStyles.toolbarPopup, GUILayout.MinWidth(50f)))
				{
					GenericMenu menu = new GenericMenu();
					menu.AddItem(new GUIContent("Small"), false, () => { Debug.Log("Small"); });
					menu.AddItem(new GUIContent("Default"), false, () => { Debug.Log("Default"); });
					menu.AddItem(new GUIContent("Big"), false, () => { Debug.Log("Big"); });
					menu.AddItem(new GUIContent("Compact"), false, () => { Debug.Log("Compact"); });
					menu.ShowAsContext();
				}*/
				
				GUILayout.FlexibleSpace();

				if (GUILayout.Button("Help", EditorStyles.toolbarButton))
				{
					Application.OpenURL("http://www.pygmymonkey.com/tools/color-palette/");
				}
			}
			GUILayout.EndHorizontal();
		}
		
		private void clearPalettes()
		{
			if (EditorUtility.DisplayDialog("Clear palettes", "This will remove ALL palettes except the first one!\nAre you sure you want to do this?", "Validate", "Cancel"))
			{
				m_paletteData.clearPalettes();
				PaletteUtils.SavePalettes(m_paletteData);
			}
		}

		private void restoreDefaultPalettes()
		{
			if (EditorUtility.DisplayDialog("Restore default palettes", "This will remove all your current palettes!\nAre you sure you want to do this?", "Validate", "Cancel"))
			{
				m_paletteData.restoreDefaultPalettes();
				PaletteUtils.SavePalettes(m_paletteData);
			}
		}
	}
}
