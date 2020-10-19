using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

namespace PygmyMonkey.ColorPalette
{
	// With help from: http://www.cyotek.com/blog/reading-photoshop-color-swatch-aco-files-using-csharp
	// And: http://www.adobe.com/devnet-apps/photoshop/fileformatashtml/#50577411_pgfId-1055819
	public static class ACOFileImporter
	{
		public static ColorPalette Import(string filePath)
		{
			ColorPalette colorPalette = ImportColorPalette(filePath);
			colorPalette.name = Path.GetFileNameWithoutExtension(filePath);

			if (colorPalette == null || colorPalette.colorInfoList == null || colorPalette.colorInfoList.Count == 0)
			{
				throw new UnityException("Error parsing the .aco file at path: " + filePath + ". Are you sure you selected a valid file?");
			}
			
			return colorPalette;
		}

		public static ColorPalette ImportColorPalette(string filePath)
		{
			List<ColorInfo> colorInfoList = new List<ColorInfo>();
			
			using (Stream stream = File.OpenRead(filePath))
			{
				FileVersion version = (FileVersion)ReadInt16(stream);
				
				if (version != FileVersion.Version1 && version != FileVersion.Version2)
				{
					throw new UnityException("The file " + filePath + " doesn't seem to be in ACO format.");
				}
				
				colorInfoList = ReadSwatches(stream, version);
				if (version == FileVersion.Version1)
				{
					// At the end of a version 1 file is the version 2 information.
					version = (FileVersion)ReadInt16(stream);
					if (version == FileVersion.Version2)
					{
						colorInfoList = ReadSwatches(stream, version);
					}
				}
			}
			
			return new ColorPalette(null, colorInfoList);
		}
			
		private static List<ColorInfo> ReadSwatches(Stream stream, FileVersion version)
		{
			List<ColorInfo> colorInfoList = new List<ColorInfo>();
			
			int colorCount = ReadInt16(stream);
			for (int i = 0; i < colorCount; i++)
			{
				ColorSpace colorSpace = (ColorSpace)ReadInt16(stream);

				int value1 = ReadInt16(stream);
				int value2 = ReadInt16(stream);
				int value3 = ReadInt16(stream);
				/*int value4 = */ReadInt16(stream); // Useful for colour spaces using 4 values (like CMYK)
				
				string colorName = null;
				if (version == FileVersion.Version2)
				{
					int length = ReadInt32(stream);
          			colorName = ReadString(stream, length);
				}
				
				switch (colorSpace)
				{
					case ColorSpace.RGB:
					// The first 3 values in the color data are red, green and blue.
					// They are full unsigned 16-bit values. Pure red = 65535, 0, 0
					
					int r = value1 / 256; // Convert to 0-255
					int g = value2 / 256; // Convert to 0-255
					int b = value3 / 256; // Convert to 0-255
					
					colorInfoList.Add(new ColorInfo(colorName, new Color(r, g, b, 255.0f)));
					break;
					
					default:
					Debug.LogWarning("The color #" + i + " is in " + colorSpace.ToString() + " but Color Palette only does RGB for the time being. Skipping this color.");
					break;
				}
			}
			
			return colorInfoList;
		}
		
		private enum FileVersion
		{
			Version1 = 1,
			Version2,
		}

		private enum ColorSpace
		{
			RGB = 0,
			HSB = 1,
			CMYK = 2,
			LAB = 7,
			GRAYSCALE = 8,
		}
		
		/// <summary>
		/// Reads a 16bit unsigned integer in big-endian format.
		/// </summary>
		/// <param name="stream">The stream to read the data from.</param>
		/// <returns>The unsigned 16bit integer cast to an <c>Int32</c>.</returns>
		private static int ReadInt16(Stream stream)
		{
			return (stream.ReadByte() << 8) | (stream.ReadByte() << 0);
		}
		
		/// <summary>
		/// Reads a 32bit unsigned integer in big-endian format.
		/// </summary>
		/// <param name="stream">The stream to read the data from.</param>
		/// <returns>The unsigned 32bit integer cast to an <c>Int32</c>.</returns>
		private static int ReadInt32(Stream stream)
		{
			return ((byte)stream.ReadByte() << 24) | ((byte)stream.ReadByte() << 16) | ((byte)stream.ReadByte() << 8) | ((byte)stream.ReadByte() << 0);
		}
		
		/// <summary>
		/// Reads a unicode string of the specified length.
		/// </summary>
		/// <param name="stream">The stream to read the data from.</param>
		/// <param name="length">The number of characters in the string.</param>
		/// <returns>The string read from the stream.</returns>
		private static string ReadString(Stream stream, int length)
		{
			byte[] buffer = new byte[length * 2];
			stream.Read(buffer, 0, buffer.Length);
			return Encoding.BigEndianUnicode.GetString(buffer);
		}
	}
}