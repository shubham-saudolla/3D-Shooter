/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	public Transform tilePrefab;
	public Vector2 mapSize;

	[Range(0,1)]
	public float outlinePercent;

	void Start()
	{
		GenerateMap();
	}

	public void GenerateMap()
	{
		// the previous tiles need to be deleted before generating new ones, hence 
		string holderName = "Generator Map";							// the string which will store the name of the gameobject
		if(transform.Find(holderName))
		{
			// using DestroyImmediate() because it needs to be called from the editor
			DestroyImmediate(transform.Find(holderName).gameObject);	// destroy the gameobject associated with the holdername string
		}

		Transform mapHolder = new GameObject(holderName).transform;
		mapHolder.parent = transform;									//parenting the new mapholder to the parent script

		for(int x = 0; x < mapSize.x; x++)
		{
			for(int y = 0; y < mapSize.y; y++)
			{
				Vector3 tilePosition = new Vector3(-mapSize.x/2 + 0.5f + x, 0, -mapSize.y/2 + 0.5f + y);

				// the tile or quad has to be rotated on the x axis by 90 degrees as it faces us
				Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right*90)) as Transform;

				newTile.localScale = Vector3.one * (1 - outlinePercent);
				newTile.parent = mapHolder;								//parenting the new tile to the mapholder, hierarchy matters, I guess...
			}
		}
	}
}
