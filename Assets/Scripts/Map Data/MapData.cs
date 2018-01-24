using System.Collections.Generic;
using UnityEngine;

public class MapData {

    public MapData()
    {
        instance = this;

        _entityQuadtree = new Quadtree<Entity>(MAPSIZE);
        _entityCommunicationHash = new RadialSpatialHash<Entity>(COMMUNICATION_BUCKET_SIZE);
        _dirtyChunks = new Queue<Chunk>();

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
    private RadialSpatialHash<Entity> _entityCommunicationHash;
    private Queue<Chunk> _dirtyChunks;

    /// <summary>
    /// Size of the buckets in the communication spatial hash
    /// </summary>
    public const int COMMUNICATION_BUCKET_SIZE = 8;

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
    /// Minimum amount of zombies per horde
    /// </summary>
    private const int HORDES_ZOMBIE_AMOUNT_MIN = 10;

    /// <summary>
    /// Maximum amount of zombies per horde
    /// </summary>
    private const int HORDES_ZOMBIE_AMOUNT_MAX = 50;

    /// <summary>
    /// Minimum amount of hordes per map
    /// </summary>
    private const int HORDES_AMOUNT_PER_MAP_MIN = 2;

    /// <summary>
    /// Maximum amount of hordes per map
    /// </summary>
    private const int HORDES_AMOUNT_PER_MAP_MAX = 5;

    /// <summary>
    /// How closely should colonists be placed? 1 leaves no extra space, 2 will leave as much space as there are colonists and so forth.
    /// </summary>
    private const float COLONIST_DENSITY = 5;

    public static void Update()
    {
        instance._entityQuadtree = new Quadtree<Entity>(MAPSIZE);
        instance._entityCommunicationHash = new RadialSpatialHash<Entity>(COMMUNICATION_BUCKET_SIZE);

        foreach (Entity entity in Entities)
        {
            PollForDataInsertion(entity);
        }

        DrawDataStructures();

        CleanChunks();
    }
    public static bool IsPassable(Vector2 position)
    {
        if (IsValidPosition(position))
        {
            Chunk chunk = Utility.WorldPositionToChunk(position);
            Vector2 localPosition = Utility.WorldPositionToLocalChunkPosition(position);

            return TileType.AllTypes[chunk.GetChunkTile((byte)localPosition.x, (byte)localPosition.y)].IsPassable;
        }

        return false;
    }
    public static void RemoveWallTile(Vector2 position)
    {
        if (IsValidPosition(position))
        {
            Chunk chunk = Utility.WorldPositionToChunk(position);
            Vector2 localPosition = Utility.WorldPositionToLocalChunkPosition(position);

            chunk.RemoveWallTile(localPosition);
        }
    }
    public static void SetWallTile(Vector2 position, byte tileIndex)
    {
        if (IsValidPosition(position))
        {
            Chunk chunk = Utility.WorldPositionToChunk(position);
            Vector2 localPosition = Utility.WorldPositionToLocalChunkPosition(position);

            chunk.SetWallTile(tileIndex, localPosition);
            RegionManager.SetDirty(position);
        }
    }
    public static void SetDirty(Chunk chunk)
    {
        if(!instance._dirtyChunks.Contains(chunk))
            instance._dirtyChunks.Enqueue(chunk);
    }
    public static List<Entity> GetAllVisibleEntities(Vector2 center, float radius)
    {
        return instance._entityCommunicationHash.Get(center, radius);
    }
    private static void PollForDataInsertion(Entity entity)
    {
        //Quadtree
        instance._entityQuadtree.Insert(entity.Rect, entity);

        //Communication Spatial Hash
        if(entity is IVisibleEntity)
        {
            IVisibleEntity communicableEntity = entity as IVisibleEntity;

            instance._entityCommunicationHash.Insert(entity, entity.transform.position, communicableEntity.SizeRadius);
        }
    }
    private static void CleanChunks()
    {
        while (instance._dirtyChunks.Count > 0)
        {
            ChunkGenerator.RenderChunk(instance._dirtyChunks.Dequeue());
        }
    }
    private static void DrawDataStructures()
    {
        EntityQuadtree.Draw();
        instance._entityCommunicationHash.Draw();
        RegionManager.Draw();
    }
    public static Entity CreateEntity<T>() where T : Entity
    {
        Entity entity = Entity.CreateEntity<T>();
        entity.transform.SetParent(World.Entities.transform);

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

        byte tileType = chunk.TerrainTiles[(byte)localPos.x, (byte)localPos.y];

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


        return;
        CreateColonists();
        CreateZombies();
    }
    private void CreateZombies()
    {
        int amountOfHordesToSpawn = Random.Range(HORDES_AMOUNT_PER_MAP_MIN, HORDES_AMOUNT_PER_MAP_MAX);

        for (int i = 0; i < amountOfHordesToSpawn; i++)
        {
            int amountOfZombiesToSpawn = Random.Range(HORDES_ZOMBIE_AMOUNT_MIN, HORDES_ZOMBIE_AMOUNT_MAX);

            List<Zombie> horde = SpawnInCluster<Zombie>(amountOfZombiesToSpawn);

            #region debug. remove me
            horde[0].AssignWork(new MoveDirectionWork(Utility.DegreesToVector2(Random.Range(0, 360))));
            #endregion
        }        
    }
    private void CreateColonists()
    {
        int amountOfColonistsToSpawn = Random.Range(COLONIST_AMOUNT_MIN, COLONIST_AMOUNT_MAX);

        SpawnInCluster<Colonist>(amountOfColonistsToSpawn);
    }
    private List<T> SpawnInCluster<T>(int amount) where T : Entity
    {
        return SpawnInCluster<T>(amount, GetSpawnAnchor());
    }
    private List<T> SpawnInCluster<T>(int amount, Vector2 anchor) where T : Entity
    {
        List<Vector2> possiblePositions = GetPossibleSpawnPositions(anchor, amount);
        List<T> toReturn = new List<T>();

        for (int i = 0; i < amount; i++)
        {
            int index = Random.Range(0, possiblePositions.Count - 1);

            toReturn.Add(CreateEntityAtPosition<T>(possiblePositions[index]) as T);

            possiblePositions.RemoveAt(index);
        }

        return toReturn;
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
    private Entity CreateEntityAtPosition<T>(Vector2 position) where T : Entity
    {
        Entity entity = MapData.CreateEntity<T>();

        entity.transform.position = position;

        return entity;
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
