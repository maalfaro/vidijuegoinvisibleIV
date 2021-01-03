using System;
using System.Collections.Generic;
using System.Linq;

public static class Extensions {

	public static void Shuffle<T>(this IList<T> list) {
		for (int i = 0; i < list.Count; i++) {
			T temp = list[i];
			int randomIndex = UnityEngine.Random.Range(i, list.Count);
			list[i] = list[randomIndex];
			list[randomIndex] = temp;
		}
	}

	public static List<T> Clone<T>(this List<T> listToClone) where T : ICloneable {
		return listToClone.Select(item => (T)item.Clone()).ToList();
	}
}

