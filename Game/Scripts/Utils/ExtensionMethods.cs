using System;
using System.Collections.Generic;
using Godot;

public static class ExtensionMethods
{
	public static int FindIndex<T>(this T[] array, Func<T, bool> func)
	{
		for(int i = 0; i < array.Length; i++)
		{
			if(func(array[i]))
			{
				return i;
			}
		}

		return -1;
	}

	public static int IndexOf<T>(this T[] array, T item)
	{
		for(int i = 0; i < array.Length; i++)
		{
			if(Equals(array[i], item))
			{
				return i;
			}
		}

		return -1;
	}

	public static void Shuffle<T>(this IList<T> list, RandomNumberGenerator rng)
	{
		int n = list.Count;
		while(n > 1)
		{
			n--;
			int k = rng.RandiRange(0, n);
			(list[k], list[n]) = (list[n], list[k]);
		}
	}

	public static T PickRandom<T>(this IList<T> list, RandomNumberGenerator rng)
	{
		return list[rng.RandiRange(0, list.Count - 1)];
	}

	public static bool AddIfNew<T>(this List<T> list, T item)
	{
		if(list.Contains(item))
		{
			return false;
		}

		list.Add(item);
		return true;
	}

	public static bool RemoveLast<T>(this IList<T> list)
	{
		if(list.Count == 0)
		{
			return false;
		}

		list.RemoveAt(list.Count - 1);
		return true;
	}

	/// <summary>
	/// Returns a Rect that has been expanded by the specified amount.
	/// </summary>
	/// <param name="rect">The original Rect.</param>
	/// <param name="expand">The desired expansion.</param>
	public static Rect2 Expand(this Rect2 rect, float expand)
	{
		rect.Position -= expand * Vector2.One;
		rect.Size += 2 * expand * Vector2.One;
		return rect;
	}

	public static Vector3 X0Y(this Vector2 vector2, float newY = 0f)
	{
		return new Vector3(vector2.X, newY, vector2.Y);
	}

	public static Vector2 XZ(this Vector3 vector3)
	{
		return new Vector2(vector3.X, vector3.Z);
	}

	public static Vector2I Add(this Vector2I coords, Direction direction)
	{
		return coords + Map.NeighbourOffsets[(int)direction];
	}
}