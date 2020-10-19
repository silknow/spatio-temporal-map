using UnityEditor;
using UnityEngine;

namespace PygmyMonkey.ColorPalette
{
	[CanEditMultipleObjects]
	[CustomEditor(typeof(ColorPaletteObject))]
	public class ColorPaletteObjectEditor : Editor
	{
		private SerializedProperty m_colorIndexProperty;
		private SerializedProperty m_reactProperty;
		private SerializedProperty m_customPaletteIndexProperty;
		private SerializedProperty m_overrideAlphaProperty;
		private SerializedProperty m_alphaProperty;

		private ColorPaletteObject m_target;

		void OnEnable()
		{
			m_colorIndexProperty = serializedObject.FindProperty("colorIndex");
			m_reactProperty = serializedObject.FindProperty("react");
			m_customPaletteIndexProperty = serializedObject.FindProperty("customPaletteIndex");
			m_overrideAlphaProperty = serializedObject.FindProperty("overrideAlpha");
			m_alphaProperty = serializedObject.FindProperty("alpha");

			m_target = (ColorPaletteObject)target;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();


			// Draw the enum
			EditorGUILayout.PropertyField(m_reactProperty);


			// If NONE, draw nothing
			if (m_reactProperty.enumValueIndex == (int)ColorPaletteObject.React.NONE)
			{
				serializedObject.ApplyModifiedProperties();
				return;
			}


			// Draw the palette selection
			if (!m_reactProperty.hasMultipleDifferentValues)
			{
				if (m_reactProperty.enumValueIndex == (int)ColorPaletteObject.React.CUSTOM_PALETTE)
				{
					m_customPaletteIndexProperty.intValue = EditorGUILayout.Popup("Palette", m_customPaletteIndexProperty.intValue, ColorPaletteData.Singleton.getColorPaletteNameArray());
				}
				else if (m_reactProperty.enumValueIndex == (int)ColorPaletteObject.React.CURRENT_PALETTE)
				{
					EditorGUILayout.LabelField("Palette", ColorPaletteData.Singleton.getCurrentPalette().name);
				}
			}


			// Fix issue if a selected color palette has been deleted
			if (m_reactProperty.enumValueIndex == (int)ColorPaletteObject.React.CUSTOM_PALETTE)
			{
				if (m_customPaletteIndexProperty.intValue >= ColorPaletteData.Singleton.colorPaletteList.Count)
				{
					m_customPaletteIndexProperty.intValue = ColorPaletteData.Singleton.colorPaletteList.Count - 1;
					GUI.changed = true;
				}
			}


			// Draw the percentage slider
			EditorGUILayout.PropertyField(m_colorIndexProperty);

			// Draw the alpha properties
			EditorGUILayout.PropertyField(m_overrideAlphaProperty);
			if (m_overrideAlphaProperty.boolValue)
			{
				EditorGUILayout.PropertyField(m_alphaProperty);
			}

			GUILayout.Space(3f);

			// Draw the button colors
			int maxIndex = m_target.getColorPalette().colorInfoList.Count;

			EditorGUILayout.BeginHorizontal();
			{
				bool allObjectsSameColor = doSelectedGameObjectHaveSameColor();
				for (int i = 0; i < maxIndex; i++)
				{
					int swatchSize = isCurrentColor(i, allObjectsSameColor) ? 24 : 20;

					Rect rect = DrawCustomSwatch(m_target.getColorPalette().colorInfoList[i], swatchSize);

                    #if UNITY_2017_3_OR_NEWER
                    if (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseDown)
                    #else
                    if (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.mouseDown)
                    #endif
					{
						if (rect.Contains(Event.current.mousePosition))
						{
							m_colorIndexProperty.intValue = i;
							GUI.changed = true;
						}
					}
				}

				GUI.color = Color.white;
			}
			EditorGUILayout.EndHorizontal();
			
			serializedObject.ApplyModifiedProperties();


			// Display warnings if necessary
			if (m_colorIndexProperty.intValue < 0)
			{
				EditorGUILayout.HelpBox("You specified a negative color index value, we automatically apply index 0 in that case.", MessageType.Warning);
			}
			else if (m_colorIndexProperty.intValue >= maxIndex)
			{
				EditorGUILayout.HelpBox("You are using an index outside the length of the color list for the selected palette. The last color of the palette is applied instead", MessageType.Warning);
			}


			// Update data
			Object[] targetObjectArray = serializedObject.targetObjects;
			for (int i = 0; i < targetObjectArray.Length; i++)
			{
				// If a changed occured, we apply the modifications
				if (GUI.changed)
				{
					ColorPaletteObject colorPaletteObject = (ColorPaletteObject)targetObjectArray[i];
					colorPaletteObject.updateColor();
				}
			}
		}

		private bool isCurrentColor(int index, bool allObjectsSameColor)
		{
			if (!allObjectsSameColor)
			{
				return false;
			}
			else
			{
				return index == m_colorIndexProperty.intValue;
			}
		}

		private bool doSelectedGameObjectHaveSameColor()
		{
			Object[] targetObjectArray = serializedObject.targetObjects;
			Color referenceColor = ((ColorPaletteObject)targetObjectArray[0]).getColor();
			for (int i = 0; i < targetObjectArray.Length; i++)
			{
				ColorPaletteObject colorPaletteObject = (ColorPaletteObject)targetObjectArray[i];
				if (!referenceColor.Equals(colorPaletteObject.getColor()))
				{
					return false;
				}
			}

			return true;
		}
		
		public static Rect DrawCustomSwatch(ColorInfo colorInfo, int size)
		{
			Rect rect = EditorGUILayout.GetControlRect(false, size, EditorStyles.colorField, GUILayout.Width(size));
			GUI.enabled = false;
			GUI.Button(rect, new GUIContent("", colorInfo.ToString()));
			GUI.enabled = true;
			EditorGUIUtility.DrawColorSwatch(rect, colorInfo.color);
			
			return rect;
		}
	}
}