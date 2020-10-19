using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

#if !UNITY_WEBPLAYER && !UNITY_SAMSUNGTV
namespace PygmyMonkey.ColorPalette
{
	// From: http://www.syihosting.com/blogassets/ASEConverter/
	// And: http://www.selapa.net/swatches/colors/fileformats.php#adobe_ase
	public static class ASEFileImporter
	{
		public static ColorPalette Import(string filePath)
		{
			ColorPalette colorPalette = new ColorPalette();
			colorPalette.name = Path.GetFileNameWithoutExtension(filePath);

			byte[] data = File.ReadAllBytes(filePath);
			if (data.Length < 12 || data[0] != 'A' || data[1] != 'S' || data[2] != 'E' || data[3] != 'F')
			{
				throw new UnityException("The file " + filePath + " doesn't seem to be in Adobe Swatch Exchange (ASE) format.");
			}

			UInt32 blocks = GetInt32(data, 8);
			
			int offset = 12;
			for (int b = 0; b < blocks; b++)
			{
				UInt16 blockType = GetInt16(data,offset);
				offset += sizeof(UInt16);
				
				UInt32 blockLength = GetInt32(data,offset);
				offset += sizeof(UInt32);
				
				switch (blockType)
				{
				case 0xC001: // Group Start Block (ignored)
					break;
					
				case 0xC002: // Group End Block (ignored)
					break;
					
				case 0x0001: // Color
					ParserColorResult colorResult = ReadColor(data, offset, b);
					if (colorResult.success)
					{
						colorPalette.colorInfoList.Add(new ColorInfo(colorResult.name, colorResult.color));
					}
					break;
					
				default:
					throw new UnityException("Warning: Block " + b + " is of an unknown type 0x" + blockType.ToString("X") + " (file corrupt?)");
				}
				
				offset += (int) blockLength;
			}

			if (colorPalette.colorInfoList == null || colorPalette.colorInfoList.Count == 0)
			{
				throw new UnityException("Error parsing the .ase file at path: " + filePath + ". Are you sure you selected a valid file?");
			}

			return colorPalette;
		}

		private static ParserColorResult ReadColor(byte[] data, int offset, int block)
		{
			UInt16 lengthName = GetInt16(data, offset); 
			offset += sizeof(UInt16);            
			
			lengthName *= 2; // turn into a count of bytes, not 16-bit characters
			
			string name = Encoding.BigEndianUnicode.GetString(data, offset, lengthName - 2).Trim();
			offset += lengthName;
			
			string colorModel = Encoding.ASCII.GetString(data, offset, 4).Trim();
			offset += 4;
			
			if (!colorModel.Equals("RGB"))
			{
				Debug.LogWarning("One of the color is in " + colorModel + " but Color Palette only does RGB for the time being. Skipping this color.");
				return new ParserColorResult(name, Color.white, false);
			}
			
			int r = (int)Math.Ceiling(255f * GetFloat32(data, offset));
			offset += sizeof(Single);
			
			int g = (int)Math.Ceiling(255f * GetFloat32(data, offset));
			offset += sizeof(Single);
			
			int b = (int)Math.Ceiling(255f * GetFloat32(data, offset));
			offset += sizeof(Single);

			return new ParserColorResult(name, new Color(r/255f, g/255f, b/255f), true);
		}
		
		private static UInt16 GetInt16(byte[] data, int offset)
		{
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(data, offset, sizeof(UInt16));
			}
			
			return BitConverter.ToUInt16(data, offset);
		}
		
		private static UInt32 GetInt32(byte[] data, int offset)
		{
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(data, offset, sizeof(UInt32));
			}
			
			return BitConverter.ToUInt32(data, offset);
		}
		
		private static Single GetFloat32(byte[] data, int offset)
		{
			if (BitConverter.IsLittleEndian)
			{
				Array.Reverse(data, offset, sizeof(Single));
			}
			
			return BitConverter.ToSingle(data, offset);
		}
	}
}
#endif