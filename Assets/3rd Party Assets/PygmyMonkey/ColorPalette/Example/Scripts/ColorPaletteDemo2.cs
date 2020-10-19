using UnityEngine;
using UnityEngine.UI;
using System;

namespace PygmyMonkey.ColorPalette
{
	public class ColorPaletteDemo2 : MonoBehaviour
	{
		[SerializeField] private InputField m_inputURL = null;
		[SerializeField] private Text m_textInfo = null;
		[SerializeField] private GameObject[] m_cubeArray = null;

		void Awake()
		{
			m_textInfo.text = "";
		}

		public void onButtonDownloadClicked()
		{
			downloadPalette();
        }

        public void OnButton1Clicked()
        {
            m_inputURL.text = "https://colorhunt.co/palette/195720";
            onButtonDownloadClicked();
        }

        public void OnButton2Clicked()
        {
            m_inputURL.text = "http://colrd.com/palette/24070/";
            onButtonDownloadClicked();
        }

        public void OnButton3Clicked()
        {
            m_inputURL.text = "https://dribbble.com/shots/240412-The-Elephant-is-King";
            onButtonDownloadClicked();
        }

		private void downloadPalette()
		{
			m_textInfo.text = "Downloading palette...";

			Uri uri = new Uri(m_inputURL.text);
				
			if (uri.Host.EndsWith("colorhunt.co"))
			{
				ColorHuntWebsiteImporter.Import(uri,
                colorPalette2 =>
				{
					onPaletteDownloaded(colorPalette2);
				},
                errorMessage =>
				{
					m_textInfo.text = errorMessage;
				});
			}
			else if (uri.Host.EndsWith("dribbble.com"))
			{
				DribbbleWebsiteImporter.Import(uri, colorPalette2 =>
				{
					onPaletteDownloaded(colorPalette2);
				},
				errorMessage =>
				{
					m_textInfo.text = errorMessage;
				});
			}
            else if (uri.Host.EndsWith("colrd.com"))
            {
                ColrdWebsiteImporter.Import(uri, colorPalette2 =>
				{
					onPaletteDownloaded(colorPalette2);
				},
				errorMessage =>
				{
					m_textInfo.text = errorMessage;
				});
			}
			else
			{
				m_textInfo.text = "Sorry we do not support downloading palettes from the website " + uri.Host + " for now.";
			}
		}

        private void onPaletteDownloaded(ColorPalette colorPalette)
        {
			if (colorPalette != null)
			{
				m_textInfo.text = "Palette downloaded (found " + colorPalette.colorInfoList.Count + " colors).";
				applyColorPaletteToCubes(colorPalette);
			}
		}

		private void applyColorPaletteToCubes(ColorPalette colorPalette)
		{
			if (colorPalette.colorInfoList == null || colorPalette.colorInfoList.Count == 0)
			{
				return;
			}

			for (int i = 0; i < m_cubeArray.Length; i++)
			{
				int colorIndex = i;
				if (colorIndex >= colorPalette.colorInfoList.Count) // If there is less color than cubes, we take the last color
				{
					colorIndex = colorPalette.colorInfoList.Count - 1;
				}

                Debug.Log(colorPalette.colorInfoList[colorIndex].color);
				m_cubeArray[i].GetComponent<Renderer>().material.color = colorPalette.colorInfoList[colorIndex].color;
			}
		}
	}
}