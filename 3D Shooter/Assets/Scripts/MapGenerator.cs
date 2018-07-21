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
	public Transform obstaclePrefab;
	public Vector2 mapSize;

	[Range(0,1)]
	public float outlinePercent;
	[Range(0,1)]
	public float obstaclePercent;

	private List<Coord> allTileCoords;
	private Queue<Coord> shuffledTileCoords;

	[SerializeField]
	public int seed = 13;

	void Start()
	{
		GenerateMap();
	}

	public void GenerateMap()
	{
		allTileCoords = new List<Coord>();
		for(int x = 0; x < mapSize.x; x++)
		{
			for(int y = 0; y < mapSize.y; y++)
			{
				allTileCoords.Add(new Coord(x, y));
			}
		}
		shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));

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
				Vector3 tilePosition = CoordToPosition(x, y);

				// the tile or quad has to be rotated on the x axis by 90 degrees as it faces us
				Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right*90)) as Transform;

				newTile.localScale = Vector3.one * (1 - outlinePercent);
				newTile.parent = mapHolder;								// parenting the new tile to the mapholder, hierarchy matters, I guess...
			}
		}

		int obstacleCount = (int) (mapSize.x * mapSize.y * obstaclePercent);

		for(int i = 0; i < obstacleCount; i++)
		{
			Coord randomCoord = getRandomCoord();
			Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);

			if(obstaclePrefab == null)
			{
				Debug.LogError("No reference to an obstacle prefab");
			}
			else
			{
				Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity) as Transform;
				newObstacle.parent = mapHolder;
			}
		}
	}

	Vector3 CoordToPosition(int x, int y)
	{
		return new Vector3(-mapSize.x/2 + 0.5f + x, 0, -mapSize.y/2 + 0.5f + y);
	}

	public Coord getRandomCoord()
	{
		Coord randomCoord = shuffledTileCoords.Dequeue();
		shuffledTileCoords.Enqueue(randomCoord);
		Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);

		return randomCoord;
	}

	public struct Coord
	{
		public int x;
		public int y;

		public Coord(int _x, int _y)									// constructor
		{
			x = _x;
			y = _y;
		}
	}
}
