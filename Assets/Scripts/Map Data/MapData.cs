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
    private const int COLONIST_AMOUNT_MIN = 4;

    /// <summary>
    /// Maximum amount of colonists to spawn in new map
    /// </summary>
    private const int COLONIST_AMOUNT_MAX = 10;

    /// <summary>
    /// How closely should colonists be placed? 1 leaves no extra space, 2 will leave as much space as there are colonists and so forth.
    /// </summary>
    private const float COLONIST_DENSITY = 5;

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
        Vector2 spawnPosition = GetSpawnAnchor();

        List<Vector2> possiblePositions = GetPossibleSpawnPositions(spawnPosition, amountOfColonistsToSpawn);

        for (int i = 0; i < amountOfColonistsToSpawn; i++)
        {
            int index = Random.Range(0, possiblePositions.Count - 1);

            CreateEntityAtPosition<Colonist>(possiblePositions[index]);

            possiblePositions.RemoveAt(index);
        }
    }
    private Vector2 GetSpawnAnchor()
    {
        Vector2 position = new Vector2(Random.Range(0, MapData.MAPSIZE - 1), Random.Range(0, MapData.MAPSIZE - 1));

        if (MapData.GetTile(position).IsSpawnable)
        {
            return position;
        }

        return GetSpawnAnchor();
    }
    private List<Vector2> GetPossibleSpawnPositions(Vector2 position, int amountOfColonists)
    {
        HashSet<Vector2> closedPositions = new HashSet<Vector2>();
        List<Vector2> openPositions = new List<Vector2>();
        List<Vector2> potentialPositionQueue = new List<Vector2>();

        openPositions.Add(position);

        while (potentialPositionQueue.Count < amountOfColonists * COLONIST_DENSITY)
        {
            int index = Random.Range(0, openPositions.Count - 1);

            PollPositionAsViableSpawnPoint(openPositions[index], ref closedPositions, ref openPositions, ref potentialPositionQueue);

            openPositions.RemoveAt(index);
        }        

        return potentialPositionQueue;
    }
    private void PollPositionAsViableSpawnPoint(Vector2 position, ref HashSet<Vector2> closedPositions, ref List<Vector2> openPositions, ref List<Vector2> potentialPositionQueue)
    {
        closedPositions.Add(position);

        if (MapData.GetTile(position).IsSpawnable)
        {
            potentialPositionQueue.Add(position);
        }

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2 neighborPosition = new Vector2(x, y) + position;

                if (closedPositions.Contains(neighborPosition) || !MapData.IsValidPosition(neighborPosition))
                    continue;

                openPositions.Add(neighborPosition);
            }
        }
    }
    private void CreateEntityAtPosition<T>(Vector2 position) where T : Entity
    {
        Entity entity = MapData.CreateEntity<T>();

        entity.transform.position = position;
    }
    private void CreateChunks()
    {
        _chunks = new Dictionary<Vector2, Chunk>();

        if (((float)MAPSIZE / (float)Chunk.CHUNK_SIZE) % 1 != 0)
        {
#pragma warning disable 
            Debug.LogError("Selected mapsize is invalid. Must be a multiple of chunk size (" + Chunk.CHUNK_SIZE + "). Map size is (" + MAPSIZE + ", " + MAPSIZE + ")");
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
