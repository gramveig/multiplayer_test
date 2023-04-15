using UnityEngine;
using System.Collections;

namespace AlexeyVlasyuk.MultiplayerTest.Utilities
{
	public static class ColorHelper
	{
		public static Color[] RainbowColors = new Color[]
		{
			Red,
			Orange,
			Yellow,
			Green,
			Navy,
			Blue,
			Purple
		};

		public static Color[] AllColors = new Color[]
		{
			Lime,
			Green,
			Aqua,
			Blue,
			Navy,
			Purple,
			Pink,
			Red,
			Orange,
			Yellow
		};

		public static string ColorText(string color, string text)
		{
			return "<color=" + color + ">" + text + "</color>";
		}

		private static ColorContainer colorContainer;

		private static ColorContainer ColorContainer
		{
			get
			{
				if (colorContainer == null)
					colorContainer = Resources.Load<ColorContainer>("Utility/ColorContainer");

				return colorContainer;
			}
		}

		public static Color Lime
		{
			get { return ColorContainer.Lime; }
		}

		public static Color Green
		{
			get { return ColorContainer.Green; }
		}

		public static Color Aqua
		{
			get { return ColorContainer.Aqua; }
		}

		public static Color Blue
		{
			get { return ColorContainer.Blue; }
		}

		public static Color Navy
		{
			get { return ColorContainer.Navy; }
		}

		public static Color Purple
		{
			get { return ColorContainer.Purple; }
		}

		public static Color Pink
		{
			get { return ColorContainer.Pink; }
		}

		public static Color Red
		{
			get { return ColorContainer.Red; }
		}

		public static Color Orange
		{
			get { return ColorContainer.Orange; }
		}

		public static Color Yellow
		{
			get { return ColorContainer.Yellow; }
		}

	}
}
