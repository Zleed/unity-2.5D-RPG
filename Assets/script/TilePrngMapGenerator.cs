using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

namespace script
{
    public class TilePrngMapGenerator : MonoBehaviour
    {
        public GameObject map;

        public TileMap[] maps;
        public int mapIndex;

        public bool autoUpdate;
        public TerrainType[] regions;

        private TileMap _currentMap;

        private float _height;
        
        private bool _flag;

        private float[,] _noiseMap;


        private void Start()
        {
            GenerateMap();
        }
         
        private void GenerateMap()
        {
            _currentMap = maps[mapIndex];
            var prng = new Random();
            _noiseMap = Noise.GenerateNoiseMap(_currentMap.mapSize.x, _currentMap.mapSize.y, prng.Next(1999999999),
                _currentMap.noiseScale, prng.Next(3)+2, _currentMap.persistence, _currentMap.lacunarity,
                _currentMap.offset);

            var grid = map.GetComponent<Grid>();
            var tilemaps = grid.GetComponentsInChildren<Tilemap>();
            for (var z = 0; z < tilemaps.Length; z++)
                tilemaps[z].transform.position = new Vector3(0, z / 2f, 0);

            for (var x = 0; x < _currentMap.mapSize.x; x++)
            for (var y = 0; y < _currentMap.mapSize.y; y++)
            {
                var tileHeight = _noiseMap[x, y];
                for (var i = 0; i < regions.Length; i++)
                {
                    if (tileHeight > regions[i].terrainHeight) continue;
                    var evaluatedHeight = EvaluateHeight(tileHeight);
                    int z;
                    for (z = 0; z < evaluatedHeight; z++)
                    {
                        if (z < Depth(x, y, evaluatedHeight) || z >= regions[i].terrainHeight) continue;
                        tilemaps[z].SetTile(new Vector3Int(x, y, z), regions[i].tile);
                    }

                    if (!(prng.Next(0, 100) < _currentMap.foliageRate * 100)) continue;
                    var foliageRandomNumber = prng.Next(100) * .01f;
                    for (var f = 0; f < regions[i].Foliage.Length; f++)
                    {
                        if (foliageRandomNumber > regions[i].Foliage[f].cumulativeWeight || z < 3 || z > 8) continue;
                        var tilemap = grid.GetComponentsInChildren<Tilemap>()[z];
                        tilemap.SetTile(new Vector3Int(x, y, f), regions[i].Foliage[f].tile);
                    }
                }
            }
        }

        private int EvaluateHeight(float point)
        {
            return Mathf.FloorToInt(_currentMap.tileHeightCurve.Evaluate(point) * 10);
        }
        
        private int Depth(int x, int y, int evaluatedHeight)
        {
            var depths = new List<int>();
            if (x < _currentMap.mapSize.x - 1)
                depths.Add(EvaluateHeight(_noiseMap[x + 1, y]));
            if (x > 0)
                depths.Add(EvaluateHeight(_noiseMap[x - 1, y]));
            if (y < _currentMap.mapSize.y - 1)
                depths.Add(EvaluateHeight(_noiseMap[x, y + 1]));
            if (y > 0)
                depths.Add(EvaluateHeight(_noiseMap[x, y - 1]));
            return depths.Min() < evaluatedHeight ? depths.Min() : depths.Min() - 1;
        }

        [Serializable]
        public struct Coord
        {
            public int x;
            public int y;

            public Coord(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
        }

        [Serializable]
        public class TileMap
        {
            public Coord mapSize;
            public float noiseScale;
            public int octaves;

            [Range(0, 1)] public float persistence;

            public float lacunarity;
            public Vector2 offset;
            public AnimationCurve tileHeightCurve;
            public int seed;

            [Range(0, .65f)] public float heightMultiplier = .5f;

            [Range(0, 1)] public float foliageRate = .5f;

            public Coord MapCenter => new Coord(mapSize.x / 2, mapSize.y / 2);
        }

        [Serializable]
        public struct TerrainType
        {
            public string name;
            public float terrainHeight;
            public Tile tile;
            public FoliageType[] Foliage;
        }

        [Serializable]
        public struct FoliageType
        {
            public string name;
            public float cumulativeWeight;
            public Tile tile;
        }
    }
}