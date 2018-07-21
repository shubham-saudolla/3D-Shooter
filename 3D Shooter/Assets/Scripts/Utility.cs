/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility : MonoBehaviour
{
	// implementing the Fisher-Yates shuffle method
	public static T[] ShuffleArray<T>(T[] array, int seed)
	{
		System.Random prng = new System.Random(seed); 		// pseudo-random number generator

		for(int i = 0; i < array.Length - 1; i++)
		{
			int randomIndex = prng.Next(i, array.Length);
			T tempItem = array[randomIndex];
			array[randomIndex] = array[i];
			array[i] = tempItem;
		}

		return array;
	}
}
