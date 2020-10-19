using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using PygmyMonkey.ColorPalette.Utils;

namespace PygmyMonkey.ColorPalette
{
	public class ColorPaletteImportRenderer
	{
		private ColorPaletteData m_paletteData;

		private int m_importFileIndex;
		private string[] m_importFileExtensionArray = new string[] { ".ase", ".aco", ".gpl", ".svg", "color presets" };

		public ColorPaletteImportRenderer(ColorPaletteData paletteData)
		{
			m_paletteData = paletteData;
		}
		
		public void drawInspector()
		{
			if (GUIUtils.DrawHeader("Import palette"))
			{
				GUIUtils.BeginContents();
				{
					EditorGUILayout.BeginHorizontal();
					{
						EditorGUILayout.LabelField("Import a", GUILayout.Width(50f));
						m_importFileIndex = EditorGUILayout.Popup(m_importFileIndex, m_importFileExtensionArray, GUILayout.Width(100f));
						EditorGUILayout.LabelField("file", GUILayout.Width(20f));
						if (GUILayout.Button("Import", EditorStyles.miniButton))
						{
							importPalette();
							GUI.FocusControl(null);
						}
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.Space();

					EditorGUILayout.BeginHorizontal();
					{
						GUILayout.Space(6f);
						drawDropPaletteArea();
						GUILayout.Space(6f);
					}
					EditorGUILayout.EndHorizontal();

					GUILayout.Space(15f);
					
					GUIUtils.BeginContents();
					{
						EditorGUILayout.LabelField("You can import palettes directly from websites, by just copying the URL and clicking the button below.\n\nCompatible sites are:" +
													"\n- colorhunt.co" +
													"\n- dribbble.com" +
													"\n- colrd.com", EditorStyles.wordWrappedLabel);

						GUILayout.Space(3f);

						bool isValidURL = isStringValidURL(EditorGUIUtility.systemCopyBuffer);
						if (!isValidURL)
						{
							GUI.color = Color.yellow;
							EditorGUILayout.LabelField("Currently don't have a valid URL in clipboard.", EditorStyles.wordWrappedMiniLabel);
							GUI.color = Color.white;
						}

						EditorGUILayout.BeginHorizontal();
						{
							GUI.enabled = isValidURL;
							if (GUILayout.Button(new GUIContent("Download from URL in clipboard", EditorGUIUtility.systemCopyBuffer), EditorStyles.miniButton))
							{
								downloadPaletteFromURL(EditorGUIUtility.systemCopyBuffer);
								GUI.FocusControl(null);
							}
							GUI.enabled = true;
						}
						EditorGUILayout.EndHorizontal();
					}
					GUIUtils.EndContents();
				}
				GUIUtils.EndContents();
			}
		}
			
		private bool isStringValidURL(string data)
		{
			if (data.Contains("\n"))
			{
				return false;
			}

			Uri uriResult;
			return Uri.TryCreate(data, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
		}

		private void drawDropPaletteArea()
		{
			Rect dropArea = GUILayoutUtility.GetRect(0.0f, 32.0f, GUILayout.ExpandWidth(true));
			GUI.Box(dropArea, "Drop a palette here to import it", LegacyGUIStyle.DropItemBoxStyle);
			
			switch (Event.current.type)
			{
			case EventType.DragUpdated:
			case EventType.DragPerform:
				if (!dropArea.Contains(Event.current.mousePosition))
				{
					return;
				}
				
				DragAndDrop.visualMode = DragAndDropVisualMode.Link;
				if (Event.current.type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();
					
					foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
					{
						importPaletteFromFile(Application.dataPath.Replace("Assets", string.Empty) + AssetDatabase.GetAssetPath(draggedObject));
					}
				}
				break;
			}
		}

		private void importPalette()
		{
			string extension = "";

			switch (m_importFileIndex)
			{
			case 0:
				extension = "ase";
				break;
			case 1:
				extension = "aco";
				break;
			case 2:
				extension = "gpl";
				break;
			case 3:
				extension = "svg";
				break;
			case 4:
				extension = "colors";
				break;
			default:
				throw new UnityException("We do not support this file type");
			}

			string filePath = EditorUtility.OpenFilePanel("Import palette", Application.dataPath, extension);
			importPaletteFromFile(filePath);
		}

		private void importPaletteFromFile(string filePath)
		{
			if (string.IsNullOrEmpty(filePath))
			{
				return;
			}

			string extension = Path.GetExtension(filePath);
			switch (extension)
			{
			case ".colors":
				m_paletteData.colorPaletteList.Add(ColorPresetFileImporter.Import(filePath));
				break;
				
			case ".svg":
				m_paletteData.colorPaletteList.Add(SVGFileImporter.Import(filePath));
				break;
			
			case ".ase":
				#if UNITY_WEBPLAYER
				throw new UnityException("Sorry you can't import a .ase palette if you're using WebPlayer");
				#elif UNITY_SAMSUNGTV
				throw new UnityException("Sorry you can't import a .ase palette if you're using Samsung TV");
				#else
				m_paletteData.colorPaletteList.Add(ASEFileImporter.Import(filePath));
				break;
				#endif
				
			case ".gpl":
				m_paletteData.colorPaletteList.Add(GPLFileImporter.Import(filePath));
				break;
				
			case ".aco":
				m_paletteData.colorPaletteList.Add(ACOFileImporter.Import(filePath));
				break;

			default:
				throw new UnityException("Sorry we do not support this file type. Please contact us at tools@pygmymonkey.com");
			}

			PaletteUtils.SavePalettes(m_paletteData);
		}

		private void downloadPaletteFromURL(string url)
		{
			EditorUtility.DisplayProgressBar("Downloading palette", url, 0.5f);

			Uri uri = new Uri(url);

			if (uri.Host.EndsWith("colorhunt.co"))
			{
				ColorHuntWebsiteImporter.Import(uri, colorPalette =>
				{
					onPaletteDownloaded(colorPalette);
				},
				errorMessage =>
				{
					EditorUtility.ClearProgressBar();
					throw new UnityException(errorMessage);
				});
			}
			else if (uri.Host.EndsWith("dribbble.com"))
			{
				DribbbleWebsiteImporter.Import(uri, colorPalette =>
				{
					onPaletteDownloaded(colorPalette);
				},
				errorMessage =>
				{
					EditorUtility.ClearProgressBar();
					throw new UnityException(errorMessage);
				});
			}
			else if (uri.Host.EndsWith("colrd.com"))
			{
				ColrdWebsiteImporter.Import(uri, colorPalette =>
				{
					onPaletteDownloaded(colorPalette);
				},
				errorMessage =>
				{
					EditorUtility.ClearProgressBar();
					throw new UnityException(errorMessage);
				});
			}
			else
			{
				EditorUtility.ClearProgressBar();
				throw new UnityException("Sorry we do not support downloading palettes from the website " + uri.Host + " for now. Please contact us at tools@pygmymonkey.com");
			}
		}

        private void onPaletteDownloaded(ColorPalette colorPalette)
        {
			if (colorPalette != null)
			{
				m_paletteData.colorPaletteList.Add(colorPalette);
				PaletteUtils.SavePalettes(m_paletteData);
			}

			EditorUtility.ClearProgressBar();
		}
	}
}
