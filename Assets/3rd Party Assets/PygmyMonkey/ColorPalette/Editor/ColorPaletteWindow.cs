using UnityEditor;
using UnityEngine;
using PygmyMonkey.ColorPalette.Utils;

namespace PygmyMonkey.ColorPalette
{
	public class ColorPaletteWindow : PMEditorWindow
	{
		public static string ProductName = "Color Palette";
		public static string VersionName = "1.1.9";

		[MenuItem("Window/PygmyMonkey/Color Palette")]
		private static void ShowWindow()
		{
			EditorWindow window = createWindow<ColorPaletteWindow>(ProductName);
			window.minSize = new Vector2(280, 500);
		}

		public override string getProductName()
		{
			return ProductName;
		}

		public override string getVersionName()
		{
			return VersionName;
		}

		public override string getAssetStoreID()
		{
			return "32189";
		}

		/*
		* Data
		*/
		private ColorPaletteData m_paletteData;

		/*
		* Renderers
		*/
		private ColorPaletteMenuRenderer m_paletteMenuRenderer;
		private ColorPaletteDataRenderer m_paletteDataRenderer;
		private ColorPaletteCreateRenderer m_paletteCreateRenderer;
		private ColorPaletteImportRenderer m_paletteImportRenderer;

		/*
		* Init
		*/
		public override void init()
		{
			loadScriptableObject();
		}

		void Update()
		{
			if (m_paletteData == null)
			{
				loadScriptableObject();
				return;
			}
		}

		/*
		* Drawing
		*/
		public override void drawBegin()
		{
			m_paletteMenuRenderer.drawInspector();
		}

		public override void drawContent()
		{
			m_paletteDataRenderer.drawInspector();

			EditorGUILayout.Separator();

			m_paletteCreateRenderer.drawInspector();
			m_paletteImportRenderer.drawInspector();
		}

		private void loadScriptableObject()
		{
			m_paletteData = PMUtils.CreateScriptableObject<ColorPaletteData>("Assets/PygmyMonkey/ColorPalette/Resources/ColorPaletteData.asset");
			m_paletteData.init();

			m_paletteMenuRenderer = new ColorPaletteMenuRenderer(m_paletteData);
			m_paletteDataRenderer = new ColorPaletteDataRenderer(m_paletteData);
			m_paletteCreateRenderer = new ColorPaletteCreateRenderer(m_paletteData);
			m_paletteImportRenderer = new ColorPaletteImportRenderer(m_paletteData);
		}
	}
}