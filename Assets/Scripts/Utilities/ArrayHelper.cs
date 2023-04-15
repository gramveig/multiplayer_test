using System;
using System.Collections.Generic;

namespace AlexeyVlasyuk.MultiplayerTest.Utilities
{
	public class ArrayHelper
	{
		public static void ShuffleArray<T>(T[] arr)
		{
			for (int i = arr.Length - 1; i > 0; i--)
			{
				int r;
				r = UnityEngine.Random.Range(0, i + 1);

				T tmp = arr[i];
				arr[i] = arr[r];
				arr[r] = tmp;
			}
		}
		
		public static void ShuffleList<T>(List<T> lst)
		{
			for (int i = lst.Count - 1; i > 0; i--)
			{
				int r;
				r = UnityEngine.Random.Range(0, i + 1);

				T tmp = lst[i];
				lst[i] = lst[r];
				lst[r] = tmp;
			}
		}
		
		public static int[] GetRandomizedIndexes(int to)
		{
			int[] idxs = new int[to];
			for (int i = 0; i < to; i++)
			{
				idxs[i] = i;
			}

			ShuffleArray(idxs);

			return idxs;
		}
	}
}
