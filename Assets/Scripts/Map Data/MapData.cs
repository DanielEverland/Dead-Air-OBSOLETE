using System.Collections.Generic;
using UnityEngine;

public class MapData {

    public MapData()
    {
        instance = this;

        _entityQuadtree = new Quadtree<Entity>(MAPSIZE);

        PopulateMap();
    }

    private static MapData instance;

    public static IEnumerable<Entity> Entities { get { return instance._entities; } }

    public static IEnumerable<Chunk> ChunkObjects { get { return instance._chunks.Values; } }
    public static IEnumerable<Vector2> ChunkPositions { get { return instance._chunks.Keys; } }
    public static IDictionary<Vector2, Chunk> Chunks { get { return instance._chunks; } }

    public static Quadtree<Entity> EntityQuadtree { get { return instance._entityQuadtree; } }

    private Dictionary<Vector2, Chunk> _chunks;
    private List<Entity> _entities;
    private Quadtree<Entity> _entityQuadtree;

    /// <summary>
    /// How many tiles wide and high should the map be?
    /// </summary>
    public const int MAPSIZE = 512;

    /// <summary>
    /// Minimum amount of colonists to spawn in new map
    /// </summary>
    private const int COLONIST_AMOUNT_MIN = 3;

    /// <summary>
    /// Maximum amount of colonists to spawn in new map
    /// </summary>
    private const int COLONIST_AMOUNT_MAX = 10;

    /// <summary>
    /// Radius in which to spawn colonists of each other
    /// </summary>
    private const float COLONIST_SPAWN_RADIUS = 10;

    public static void RefreshQuadtree()
    {
        instance._entityQuadtree = new Quadtree<Entity>(MAPSIZE);

        for (int i = 0; i < instance._entities.Count; i++)
        {
            instance._entityQuadtree.Insert(instance._entities[i].Rect, instance._entities[i]);
        }
    }
    public static Entity CreateEntity<T>() where T : Entity
    {
        Entity entity = Entity.CreateEntity<T>();

        AddEntity(entity);

        return entity;
    }
    public static void AddEntity(Entity entity)
    {
        instance._entities.Add(entity);
    }
    public static bool IsValidPosition(Vector2 position)
    {
        return position.x >= 0 && position.y >= 0
            &&
            position.x < MAPSIZE && position.y < MAPSIZE;
    }
    public static TileType GetTile(Vector2 position)
    {
        if(!IsValidPosition(position))
        {
            throw new System.NullReferenceException("Position is invalid " + position);
        }

        Vector2 chunkPosition = Utility.WorldPositionToChunkPosition(position);

        if (!Chunks.ContainsKey(chunkPosition))
        {
            throw new System.NullReferenceException("There is no chunk at " + chunkPosition + ". Input: " + position);
        }

        Chunk chunk = Chunks[chunkPosition];

        Vector2 localPos = Utility.WorldPositionToLocalChunkPosition(position);

        if(!Chunk.IsValidLocalPosition(localPos))
        {
            throw new System.NullReferenceException("Local position " + localPos + " is invalid. Input: " + position);
        }

        byte tileType = chunk.Tiles[(byte)localPos.x, (byte)localPos.y];

        return TileType.AllTypes[tileType];
    }
    private void PopulateMap()
    {
        CreateChunks();
        CreateEntities();
    }
    private void CreateEntities()
    {
        _entities = new List<Entity>();

        CreateColonists();
    }
    private void CreateColonists()
    {
        int amountOfColonistsToSpawn = Random.Range(COLONIST_AMOUNT_MIN, COLONIST_AMOUNT_MAX);

        Vector2 spawnpoint = GetColonistSpawnPoint(amountOfColonistsToSpawn);
        int radiusHalf = Mathf.RoundToInt(COLONIST_SPAWN_RADIUS / 2);
        HashSet<Vector2> usedPositions = new HashSet<Vector2>();

        while (amountOfColonistsToSpawn > 0)
        {
            Vector2 point = new Vector2()
            {
                x = Random.Range(-radiusHalf, radiusHalf) + spawnpoint.x,
                y = Random.Range(-radiusHalf, radiusHalf) + spawnpoint.y,
            };

            if (Vector2.Distance(point, spawnpoint) > COLONIST_SPAWN_RADIUS || !IsValidPosition(point) || usedPositions.Contains(point))
                continue;

            if (MapData.GetTile(point).IsSpawnable)
            {
                amountOfColonistsToSpawn--;

                CreateEntityAtPosition<Colonist>(point);

                usedPositions.Add(point);
            }
        }
    }
    private void CreateEntityAtPosition<T>(Vector2 position) where T : Entity
    {
        Entity entity = MapData.CreateEntity<T>();

        entity.transform.position = position;
    }
    private Vector2 GetColonistSpawnPoint(int amountOfColonistsToSpawn)
    {
        Vector2 point = new Vector2()
        {
            x = Random.Range(0, MAPSIZE),
            y = Random.Range(0, MAPSIZE),
        };

        if (IsViableSpawnPoint(point, amountOfColonistsToSpawn))
        {
            return point;
        }
        else
        {
            return GetColonistSpawnPoint(amountOfColonistsToSpawn);
        }
    }
    private bool IsViableSpawnPoint(Vector2 point, int amountOfColonistsToSpawn)
    {
        int radiusHalf = Mathf.RoundToInt(COLONIST_SPAWN_RADIUS / 2);
        int viableSpawnPoints = 0;

        for (int x = -radiusHalf; x < radiusHalf; x++)
        {
            for (int y = -radiusHalf; y < radiusHalf; y++)
            {
                if (viableSpawnPoints >= amountOfColonistsToSpawn)
                    return true;

                Vector2 currentPoint = new Vector2(point.x + x, point.y + y);

                if (Vector2.Distance(currentPoint, point) > COLONIST_SPAWN_RADIUS || !IsValidPosition(currentPoint))
                    continue;

                if (MapData.GetTile(currentPoint).IsSpawnable)
                {
                    viableSpawnPoints++;
                }
            }
        }

        return false;
    }
    private void CreateChunks()
    {
        _chunks = new Dictionary<Vector2, Chunk>();

        if (((float)MAPSIZE / (float)Chunk.CHUNK_SIZE) % 1 != 0)
        {
#pragma warning disable 
            Debug.LogError("Selected mapsize is unvalid. Must be a multiple of chunk size (" + Chunk.CHUNK_SIZE + "). Map size is (" + MAPSIZE + ", " + MAPSIZE + ")");
#pragma warning restore
        }

        int chunkCount = (int)((float)MAPSIZE / (float)Chunk.CHUNK_SIZE);

        for (int x = 0; x < chunkCount; x++)
        {
            for (int y = 0; y < chunkCount; y++)
            {
                Vector2 chunkPosition = new Vector2(x, y);

                Chunk newChunk = new Chunk(chunkPosition);

                _chunks.Add(chunkPosition, newChunk);
            }
        }
    }
}
