using UnityEngine;
using System.Collections.Generic;

namespace PygmyMonkey.ColorPalette
{
	public static class PaletteFirstUseUtils
	{
		public static List<ColorPalette> GetDefaultPalette()
		{
			return new List<ColorPalette>()
			{
				new ColorPalette("01 - Red", new List<ColorInfo>() {
					new ColorInfo("Red 50", new Color(255, 235, 238, 255)),
					new ColorInfo("Red 100", new Color(255, 205, 210, 255)),
					new ColorInfo("Red 200", new Color(238, 153, 153, 255)),
					new ColorInfo("Red 300", new Color(228, 114, 114, 255)),
					new ColorInfo("Red 400", new Color(238, 83, 80, 255)),
					new ColorInfo("Red 500", new Color(243, 67, 54, 255)),
					new ColorInfo("Red 600", new Color(228, 57, 53, 255)),
					new ColorInfo("Red 700", new Color(210, 47, 47, 255)),
					new ColorInfo("Red 800", new Color(197, 40, 40, 255)),
					new ColorInfo("Red 900", new Color(182, 28, 28, 255)),
				}),
				
				new ColorPalette("02 - Pink", new List<ColorInfo>() {
					new ColorInfo("Pink 50", new Color(251, 227, 235, 255)),
					new ColorInfo("Pink 100", new Color(247, 186, 207, 255)),
					new ColorInfo("Pink 200", new Color(243, 142, 176, 255)),
					new ColorInfo("Pink 300", new Color(239, 98, 145, 255)),
					new ColorInfo("Pink 400", new Color(235, 64, 121, 255)),
					new ColorInfo("Pink 500", new Color(232, 30, 99, 255)),
					new ColorInfo("Pink 600", new Color(215, 27, 96, 255)),
					new ColorInfo("Pink 700", new Color(193, 24, 91, 255)),
					new ColorInfo("Pink 800", new Color(172, 20, 87, 255)),
					new ColorInfo("Pink 900", new Color(135, 14, 79, 255)),
				}),
				
				new ColorPalette("03 - Purple", new List<ColorInfo>() {
					new ColorInfo("Purple 50", new Color(242, 228, 244, 255)),
					new ColorInfo("Purple 100", new Color(224, 189, 230, 255)),
					new ColorInfo("Purple 200", new Color(205, 146, 215, 255)),
					new ColorInfo("Purple 300", new Color(185, 104, 199, 255)),
					new ColorInfo("Purple 400", new Color(170, 71, 187, 255)),
					new ColorInfo("Purple 500", new Color(155, 39, 175, 255)),
					new ColorInfo("Purple 600", new Color(141, 36, 169, 255)),
					new ColorInfo("Purple 700", new Color(122, 31, 161, 255)),
					new ColorInfo("Purple 800", new Color(106, 27, 153, 255)),
					new ColorInfo("Purple 900", new Color(74, 20, 139, 255)),
				}),
				
				new ColorPalette("04 - Deep Purple", new List<ColorInfo>() {
					new ColorInfo("Deep Purple 50", new Color(236, 230, 245, 255)),
					new ColorInfo("Deep Purple 100", new Color(208, 195, 232, 255)),
					new ColorInfo("Deep Purple 200", new Color(178, 156, 218, 255)),
					new ColorInfo("Deep Purple 300", new Color(148, 116, 204, 255)),
					new ColorInfo("Deep Purple 400", new Color(125, 87, 193, 255)),
					new ColorInfo("Deep Purple 500", new Color(103, 58, 182, 255)),
					new ColorInfo("Deep Purple 600", new Color(94, 53, 176, 255)),
					new ColorInfo("Deep Purple 700", new Color(81, 45, 167, 255)),
					new ColorInfo("Deep Purple 800", new Color(69, 39, 159, 255)),
					new ColorInfo("Deep Purple 900", new Color(49, 27, 145, 255)),
				}),
				
				new ColorPalette("05 - Indigo", new List<ColorInfo>() {
					new ColorInfo("Indigo 50", new Color(231, 233, 245, 255)),
					new ColorInfo("Indigo 100", new Color(196, 201, 232, 255)),
					new ColorInfo("Indigo 200", new Color(158, 167, 217, 255)),
					new ColorInfo("Indigo 300", new Color(120, 133, 202, 255)),
					new ColorInfo("Indigo 400", new Color(92, 107, 191, 255)),
					new ColorInfo("Indigo 500", new Color(63, 81, 180, 255)),
					new ColorInfo("Indigo 600", new Color(57, 73, 170, 255)),
					new ColorInfo("Indigo 700", new Color(48, 63, 158, 255)),
					new ColorInfo("Indigo 800", new Color(40, 53, 146, 255)),
					new ColorInfo("Indigo 900", new Color(26, 35, 125, 255)),
				}),
				
				new ColorPalette("06 - Blue", new List<ColorInfo>() {
					new ColorInfo("Blue 50", new Color(226, 241, 252, 255)),
					new ColorInfo("Blue 100", new Color(186, 221, 250, 255)),
					new ColorInfo("Blue 200", new Color(143, 201, 248, 255)),
					new ColorInfo("Blue 300", new Color(100, 180, 245, 255)),
					new ColorInfo("Blue 400", new Color(66, 164, 244, 255)),
					new ColorInfo("Blue 500", new Color(33, 149, 242, 255)),
					new ColorInfo("Blue 600", new Color(30, 135, 228, 255)),
					new ColorInfo("Blue 700", new Color(25, 117, 209, 255)),
					new ColorInfo("Blue 800", new Color(21, 101, 191, 255)),
					new ColorInfo("Blue 900", new Color(13, 71, 160, 255)),
				}),
				
				new ColorPalette("07 - Light Blue", new List<ColorInfo>() {
					new ColorInfo("Light Blue 50", new Color(224, 244, 253, 255)),
					new ColorInfo("Light Blue 100", new Color(178, 228, 251, 255)),
					new ColorInfo("Light Blue 200", new Color(128, 211, 249, 255)),
					new ColorInfo("Light Blue 300", new Color(79, 194, 246, 255)),
					new ColorInfo("Light Blue 400", new Color(41, 181, 245, 255)),
					new ColorInfo("Light Blue 500", new Color(3, 168, 243, 255)),
					new ColorInfo("Light Blue 600", new Color(3, 154, 228, 255)),
					new ColorInfo("Light Blue 700", new Color(2, 135, 208, 255)),
					new ColorInfo("Light Blue 800", new Color(2, 118, 188, 255)),
					new ColorInfo("Light Blue 900", new Color(1, 87, 154, 255)),
				}),
				
				new ColorPalette("08 - Cyan", new List<ColorInfo>() {
					new ColorInfo("Cyan 50", new Color(223, 246, 249, 255)),
					new ColorInfo("Cyan 100", new Color(177, 234, 241, 255)),
					new ColorInfo("Cyan 200", new Color(127, 221, 233, 255)),
					new ColorInfo("Cyan 300", new Color(77, 207, 224, 255)),
					new ColorInfo("Cyan 400", new Color(38, 197, 217, 255)),
					new ColorInfo("Cyan 500", new Color(0, 187, 211, 255)),
					new ColorInfo("Cyan 600", new Color(0, 171, 192, 255)),
					new ColorInfo("Cyan 700", new Color(0, 150, 166, 255)),
					new ColorInfo("Cyan 800", new Color(0, 130, 142, 255)),
					new ColorInfo("Cyan 900", new Color(0, 96, 100, 255)),
				}),
				
				new ColorPalette("09 - Teal", new List<ColorInfo>() {
					new ColorInfo("Teal 50", new Color(223, 241, 240, 255)),
					new ColorInfo("Teal 100", new Color(177, 222, 218, 255)),
					new ColorInfo("Teal 200", new Color(127, 202, 195, 255)),
					new ColorInfo("Teal 300", new Color(77, 181, 171, 255)),
					new ColorInfo("Teal 400", new Color(38, 165, 153, 255)),
					new ColorInfo("Teal 500", new Color(0, 149, 135, 255)),
					new ColorInfo("Teal 600", new Color(0, 136, 122, 255)),
					new ColorInfo("Teal 700", new Color(0, 120, 107, 255)),
					new ColorInfo("Teal 800", new Color(0, 105, 92, 255)),
					new ColorInfo("Teal 900", new Color(0, 77, 64, 255)),
				}),
				
				new ColorPalette("10 - Green", new List<ColorInfo>() {
					new ColorInfo("Green 50", new Color(232, 245, 233, 255)),
					new ColorInfo("Green 100", new Color(200, 230, 201, 255)),
					new ColorInfo("Green 200", new Color(165, 214, 167, 255)),
					new ColorInfo("Green 300", new Color(129, 199, 132, 255)),
					new ColorInfo("Green 400", new Color(102, 187, 106, 255)),
					new ColorInfo("Green 500", new Color(76, 175, 80, 255)),
					new ColorInfo("Green 600", new Color(67, 160, 71, 255)),
					new ColorInfo("Green 700", new Color(56, 142, 60, 255)),
					new ColorInfo("Green 800", new Color(46, 125, 50, 255)),
					new ColorInfo("Green 900", new Color(27, 94, 32, 255)),
				}),
				
				new ColorPalette("11 - Light Green", new List<ColorInfo>() {
					new ColorInfo("Light Green 50", new Color(241, 248, 233, 255)),
					new ColorInfo("Light Green 100", new Color(220, 237, 200, 255)),
					new ColorInfo("Light Green 200", new Color(197, 225, 165, 255)),
					new ColorInfo("Light Green 300", new Color(174, 213, 129, 255)),
					new ColorInfo("Light Green 400", new Color(156, 204, 101, 255)),
					new ColorInfo("Light Green 500", new Color(139, 195, 74, 255)),
					new ColorInfo("Light Green 600", new Color(124, 179, 66, 255)),
					new ColorInfo("Light Green 700", new Color(104, 159, 56, 255)),
					new ColorInfo("Light Green 800", new Color(85, 139, 47, 255)),
					new ColorInfo("Light Green 900", new Color(51, 105, 30, 255)),
				}),
				
				new ColorPalette("12 - Lime", new List<ColorInfo>() {
					new ColorInfo("Lime 50", new Color(249, 251, 231, 255)),
					new ColorInfo("Lime 100", new Color(240, 244, 195, 255)),
					new ColorInfo("Lime 200", new Color(230, 238, 156, 255)),
					new ColorInfo("Lime 300", new Color(220, 231, 117, 255)),
					new ColorInfo("Lime 400", new Color(212, 225, 87, 255)),
					new ColorInfo("Lime 500", new Color(205, 220, 57, 255)),
					new ColorInfo("Lime 600", new Color(192, 202, 51, 255)),
					new ColorInfo("Lime 700", new Color(175, 180, 43, 255)),
					new ColorInfo("Lime 800", new Color(158, 157, 36, 255)),
					new ColorInfo("Lime 900", new Color(130, 119, 23, 255)),
				}),
				
				new ColorPalette("13 - Yellow", new List<ColorInfo>() {
					new ColorInfo("Yellow 50", new Color(255, 253, 231, 255)),
					new ColorInfo("Yellow 100", new Color(255, 249, 196, 255)),
					new ColorInfo("Yellow 200", new Color(255, 245, 157, 255)),
					new ColorInfo("Yellow 300", new Color(255, 241, 118, 255)),
					new ColorInfo("Yellow 400", new Color(255, 238, 88, 255)),
					new ColorInfo("Yellow 500", new Color(255, 235, 59, 255)),
					new ColorInfo("Yellow 600", new Color(253, 216, 53, 255)),
					new ColorInfo("Yellow 700", new Color(251, 192, 45, 255)),
					new ColorInfo("Yellow 800", new Color(249, 168, 37, 255)),
					new ColorInfo("Yellow 900", new Color(245, 127, 23, 255)),
				}),
				
				new ColorPalette("14 - Amber", new List<ColorInfo>() {
					new ColorInfo("Amber 50", new Color(255, 248, 225, 255)),
					new ColorInfo("Amber 100", new Color(255, 236, 179, 255)),
					new ColorInfo("Amber 200", new Color(255, 224, 130, 255)),
					new ColorInfo("Amber 300", new Color(255, 213, 79, 255)),
					new ColorInfo("Amber 400", new Color(255, 202, 40, 255)),
					new ColorInfo("Amber 500", new Color(255, 193, 7, 255)),
					new ColorInfo("Amber 600", new Color(255, 179, 0, 255)),
					new ColorInfo("Amber 700", new Color(255, 160, 0, 255)),
					new ColorInfo("Amber 800", new Color(255, 143, 0, 255)),
					new ColorInfo("Amber 900", new Color(255, 111, 0, 255)),
				}),
				
				new ColorPalette("15 - Orange", new List<ColorInfo>() {
					new ColorInfo("Orange 50", new Color(255, 243, 224, 255)),
					new ColorInfo("Orange 100", new Color(255, 224, 178, 255)),
					new ColorInfo("Orange 200", new Color(255, 204, 128, 255)),
					new ColorInfo("Orange 300", new Color(255, 183, 77, 255)),
					new ColorInfo("Orange 400", new Color(255, 167, 38, 255)),
					new ColorInfo("Orange 500", new Color(255, 152, 0, 255)),
					new ColorInfo("Orange 600", new Color(251, 140, 0, 255)),
					new ColorInfo("Orange 700", new Color(245, 124, 0, 255)),
					new ColorInfo("Orange 800", new Color(239, 108, 0, 255)),
					new ColorInfo("Orange 900", new Color(230, 81, 0, 255)),
				}),
				
				new ColorPalette("16 - Deep Orange", new List<ColorInfo>() {
					new ColorInfo("Deep Orange 50", new Color(251, 233, 231, 255)),
					new ColorInfo("Deep Orange 100", new Color(255, 204, 188, 255)),
					new ColorInfo("Deep Orange 200", new Color(255, 171, 145, 255)),
					new ColorInfo("Deep Orange 300", new Color(255, 138, 101, 255)),
					new ColorInfo("Deep Orange 400", new Color(255, 112, 67, 255)),
					new ColorInfo("Deep Orange 500", new Color(255, 87, 34, 255)),
					new ColorInfo("Deep Orange 600", new Color(244, 81, 30, 255)),
					new ColorInfo("Deep Orange 700", new Color(230, 74, 25, 255)),
					new ColorInfo("Deep Orange 800", new Color(216, 67, 21, 255)),
					new ColorInfo("Deep Orange 900", new Color(191, 54, 12, 255)),
				}),
				
				new ColorPalette("17 - Brown", new List<ColorInfo>() {
					new ColorInfo("Brown 50", new Color(239, 235, 233, 255)),
					new ColorInfo("Brown 100", new Color(215, 204, 200, 255)),
					new ColorInfo("Brown 200", new Color(188, 170, 164, 255)),
					new ColorInfo("Brown 300", new Color(161, 136, 127, 255)),
					new ColorInfo("Brown 400", new Color(141, 110, 99, 255)),
					new ColorInfo("Brown 500", new Color(121, 85, 72, 255)),
					new ColorInfo("Brown 600", new Color(109, 76, 65, 255)),
					new ColorInfo("Brown 700", new Color(93, 64, 55, 255)),
					new ColorInfo("Brown 800", new Color(78, 52, 46, 255)),
					new ColorInfo("Brown 900", new Color(62, 39, 35, 255)),
				}),

				new ColorPalette("18 - Grey", new List<ColorInfo>() {
					new ColorInfo("Grey 50", new Color(249, 249, 249, 255)),
					new ColorInfo("Grey 100", new Color(244, 244, 244, 255)),
					new ColorInfo("Grey 200", new Color(238, 238, 238, 255)),
					new ColorInfo("Grey 300", new Color(224, 224, 224, 255)),
					new ColorInfo("Grey 400", new Color(189, 189, 189, 255)),
					new ColorInfo("Grey 500", new Color(158, 158, 158, 255)),
					new ColorInfo("Grey 600", new Color(117, 117, 117, 255)),
					new ColorInfo("Grey 700", new Color(97, 97, 97, 255)),
					new ColorInfo("Grey 800", new Color(66, 66, 66, 255)),
					new ColorInfo("Grey 900", new Color(33, 33, 33, 255)),
				}),
				
				new ColorPalette("19 - Blue Grey", new List<ColorInfo>() {
					new ColorInfo("Blue Grey 50", new Color(236, 239, 241, 255)),
					new ColorInfo("Blue Grey 100", new Color(207, 216, 220, 255)),
					new ColorInfo("Blue Grey 200", new Color(176, 190, 197, 255)),
					new ColorInfo("Blue Grey 300", new Color(144, 164, 174, 255)),
					new ColorInfo("Blue Grey 400", new Color(120, 144, 156, 255)),
					new ColorInfo("Blue Grey 500", new Color(96, 125, 139, 255)),
					new ColorInfo("Blue Grey 600", new Color(84, 110, 122, 255)),
					new ColorInfo("Blue Grey 700", new Color(69, 90, 100, 255)),
					new ColorInfo("Blue Grey 800", new Color(55, 71, 79, 255)),
					new ColorInfo("Blue Grey 900", new Color(38, 50, 56, 255)),
				}),
				
				new ColorPalette("Smiley", new List<ColorInfo>() {
					new ColorInfo("Eyes", new Color(255, 255, 255, 255)),
					new ColorInfo("Face", new Color(255, 252, 36, 255)),
					new ColorInfo("Mouth", new Color(181, 0, 83, 255)),
					new ColorInfo("Contour", new Color(0, 0, 0, 255)),
				}),
				
				new ColorPalette("House", new List<ColorInfo>() {
					new ColorInfo("Walls", new Color(255, 255, 255, 255)),
					new ColorInfo("Door", new Color(0, 72, 255, 255)),
					new ColorInfo("Roof", new Color(51, 114, 0, 255)),
					new ColorInfo("Contour", new Color(0, 0, 0, 255)),
				}),
			};
		}
	}
}