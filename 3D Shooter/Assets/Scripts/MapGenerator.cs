/*
Copyright (c) Shubham Saudolla
https://github.com/shubham-saudolla
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
	public Map[] maps;
	public int mapIndex;
	public Transform tilePrefab;
	public float tileSize;
	public Transform obstaclePrefab;
	public Transform navmeshFloor;
	public Transform navMeshMaskPrefab;
	public Vector2 maxMapSize;


	[Range(0,1)]
	public float outlinePercent;

	private List<Coord> allTileCoords;
	private Queue<Coord> shuffledTileCoords;

	Map currentMap;

	void Start()
	{
		GenerateMap();
	}

	public void GenerateMap()
	{
		currentMap = maps[mapIndex];
		System.Random prng = new System.Random(currentMap.seed);

		// generating coords
		allTileCoords = new List<Coord>();
		for(int x = 0; x < currentMap.mapSize.x; x++)
		{
			for(int y = 0; y < currentMap.mapSize.y; y++)
			{
				allTileCoords.Add(new Coord(x, y));
			}
		}
		shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));

		// the previous tiles need to be deleted before generating new ones, hence 
		string holderName = "Generator Map";							// the string which will store the name of the gameobject
		if(transform.Find(holderName))
		{
			// using DestroyImmediate() because it needs to be called from the editor
			DestroyImmediate(transform.Find(holderName).gameObject);	// destroy the gameobject associated with the holdername string
		}

		Transform mapHolder = new GameObject(holderName).transform;
		mapHolder.parent = transform;									//parenting the new mapholder to the parent script

		for(int x = 0; x < currentMap.mapSize.x; x++)
		{
			for(int y = 0; y < currentMap.mapSize.y; y++)
			{
				Vector3 tilePosition = CoordToPosition(x, y);

				// the tile or quad has to be rotated on the x axis by 90 degrees as it faces us
				Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right*90)) as Transform;

				newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
				newTile.parent = mapHolder;								// parenting the new tile to the mapholder, hierarchy matters, I guess...
			}
		}

		bool [,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];
 
		int obstacleCount = (int) (currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent);
		int currentObstacleCount = 0;

		for(int i = 0; i < obstacleCount; i++)
		{
			Coord randomCoord = getRandomCoord();
			obstacleMap[randomCoord.x, randomCoord.y] = true;
			currentObstacleCount++;

			if(randomCoord != currentMap.mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
			{
				float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float) prng.NextDouble());
				Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
				Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight / 2, Quaternion.identity) as Transform;
				newObstacle.parent = mapHolder;
				newObstacle.localScale = new Vector3(((1 - outlinePercent) * tileSize), obstacleHeight, ((1 - outlinePercent) * tileSize));

				Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
				Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
				float colorPercent = randomCoord.y / (float)currentMap.mapSize.y;
				obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPercent);
				obstacleRenderer.sharedMaterial = obstacleMaterial;
			}
			else
			{
				obstacleMap[randomCoord.x, randomCoord.y] = false;
				currentObstacleCount--;
			}
		}

		Transform maskLeft = Instantiate(navMeshMaskPrefab, Vector3.left *  (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
		maskLeft.parent = mapHolder;
		maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

		Transform maskRight = Instantiate(navMeshMaskPrefab, Vector3.right *  (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
		maskRight.parent = mapHolder;
		maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

		Transform maskTop = Instantiate(navMeshMaskPrefab, Vector3.forward *  (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
		maskTop.parent = mapHolder;
		maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

		Transform maskBot = Instantiate(navMeshMaskPrefab, Vector3.back *  (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
		maskBot.parent = mapHolder;
		maskBot.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

		navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
		
	}

	// a flood-fill algorithm
	bool MapIsFullyAccessible(bool [,] obstacleMap, int currentObstacleCount)
	{
		bool [,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
		Queue<Coord> queue = new Queue<Coord>();
		queue.Enqueue(currentMap.mapCentre);
		mapFlags[currentMap.mapCentre.x, currentMap.mapCentre.y] = true;
		
		int accessibleTileCount = 1;

		while(queue.Count > 0)
		{
			Coord tile = queue.Dequeue();


			for(int x = -1; x <= 1; x++)
			{
				for(int y = -1; y <= 1; y++)
				{
					int neighbourX = tile.x + x;
					int neighbourY = tile.y + y;

					if(x==0 || y == 0)
					{
						if(neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
						{
							if(!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
							{
								mapFlags[neighbourX, neighbourY] = true;
								queue.Enqueue(new Coord(neighbourX, neighbourY));
								accessibleTileCount++;
							}
						}
					}
				}
			}
		}

		int targetAccessibleTileCount = (int) (currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
		return targetAccessibleTileCount == accessibleTileCount;
	}

	Vector3 CoordToPosition(int x, int y)
	{
		return new Vector3(-currentMap.mapSize.x/2f + 0.5f + x, 0, -currentMap.mapSize.y/2f + 0.5f + y) * tileSize;
	}

	public Coord getRandomCoord()
	{
		Coord randomCoord = shuffledTileCoords.Dequeue();
		shuffledTileCoords.Enqueue(randomCoord);
		Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);

		return randomCoord;
	}

	[System.Serializable]
	public struct Coord
	{
		public int x;
		public int y;

		public Coord(int _x, int _y)									// constructor
		{
			x = _x;
			y = _y;
		}

		public static bool operator == (Coord c1, Coord c2)
		{
			return c1.x == c2.x && c1.y == c2.y;
		}

		public static bool operator != (Coord c1, Coord c2)
		{
			return !(c1 == c2);
		}
	}

	[System.Serializable]
	public class Map
	{
		public Coord mapSize;
		[Range(0,1)]
		public float obstaclePercent;
		public int seed;
		public float minObstacleHeight;
		public float maxObstacleHeight;
		public Color foregroundColor;
		public Color backgroundColor;

		public Coord mapCentre
		{
			get
			{
				return new Coord(mapSize.x / 2, mapSize.y / 2);
			}
		}
	}
}
