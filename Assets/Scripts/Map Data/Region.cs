using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region
{
    public Region()
    {
        _id = System.Guid.NewGuid();
        
        SetDirty(false);
    }
    public Region(Vector2 position)
    {
        _anchor = Utility.WorldPositionToRegionPosition(position);
        _id = System.Guid.NewGuid();

        SetDirty(false);
    }

    public const int MAX_SIZE = 16;
    
    private static Queue<Region> _regionsToInitialize = new Queue<Region>();
    private static readonly List<Vector2> _directions = new List<Vector2> { Vector2.left, Vector2.right, Vector2.down, Vector2.up };

    public IEnumerable<Entity> Entities { get { return _entities; } }
    public Rect Bounds { get { return _bounds; } }
    public Vector2 Position { get { return Bounds.position; } }
    public int CellCount { get { return _ownedPositions.Count; } }
    public IEnumerable<Vector2> OwnedPositions { get { return _ownedPositions; } }
    public IEnumerable<Connection> Connections { get { return _connections; } }
    public int ConnectionCount { get { return _connections.Count; } }
    public IEnumerable<Region> Neighbors
    {
        get
        {
            if (_neighbors == null)
                CacheNeighbors();

            return _neighbors;
        }
    }

    private readonly Vector2 _anchor;

    private List<Entity> _entities = new List<Entity>();
    private HashSet<Connection> _connections = new HashSet<Connection>();
    private HashSet<Vector2> _ownedPositions = new HashSet<Vector2>();
    private List<Region> _neighbors;
    private HashSet<Vector2> blackList;
    private Rect _bounds;
    private bool _isDirty = false;
    private System.Guid _id;

    public void AddEntity(Entity entity)
    {
        _entities.Add(entity);
    }
    public void RemoveEntity(Entity entity)
    {
        _entities.Remove(entity);
    }
    public void Allocate(Vector2 position)
    {
        Vector2 delta = position - _anchor;

        if (delta.x < 0 || delta.y < 0 || delta.x >= MAX_SIZE || delta.y >= MAX_SIZE)
            throw new System.IndexOutOfRangeException(delta + " - " + position);

        _ownedPositions.Add(position);
    }
    public bool ContainsCell(Vector2 cellPosition)
    {
        cellPosition = cellPosition.ToCellPosition();

        return _ownedPositions.Contains(cellPosition);
    }
    public bool ContainsBounds(Vector2 worldPosition)
    {
        Vector2 delta = worldPosition - Position;

        return delta.x >= 0 && delta.y >= 0 && delta.x < MAX_SIZE && delta.y < MAX_SIZE;
    }
    public void SetDirty(bool flagNeighbors)
    {
        if(!_regionsToInitialize.Contains(this) && !_isDirty)
        {
            _isDirty = true;
            _regionsToInitialize.Enqueue(this);
        }        

        if (flagNeighbors)
        {
            foreach (Region neighbor in Neighbors)
            {
                neighbor.SetDirty(false);
            }
        }
    }
    public void Initialize()
    {
        RecalculateBounds();
        RecalculateConnections();
    }
    private void Clean()
    {
        RecalculateBounds();
        RecalculateConnections();
        CacheNeighbors();

        _isDirty = false;
    }
    private void CacheNeighbors()
    {
        _neighbors = new List<Region>();

        foreach (int connection in _connections)
        {
            foreach (Region region in RegionConnectionManager.GetRegions(connection))
            {
                if (region != this && !_neighbors.Contains(region))
                    _neighbors.Add(region);
            }
        }
    }
    private void RecalculateConnections()
    {
        RegionConnectionManager.Remove(this);
        _connections = new HashSet<Connection>();
        blackList = new HashSet<Vector2>();

        foreach (Vector2 position in _ownedPositions)
        {
            for (int i = 0; i < _directions.Count; i++)
            {
                if (!_ownedPositions.Contains(position + _directions[i]) && IsValid(position + _directions[i]))
                    CreateConnection(position, _directions[i]);
            }
        }

        RegionConnectionManager.Add(this);
    }
    private void CreateConnection(Vector2 startPosition, Vector2 edgeDirection)
    {
        if (blackList.Contains(startPosition + edgeDirection))
            return;

        HashSet<Vector2> usedPositions = new HashSet<Vector2>();

        Vector2 offset = GetOffset(edgeDirection);
        Connection.Direction regionDirection = GetRegionDirection(edgeDirection);
        Vector2 travelDirection = GetTravelDirection(edgeDirection);

        Vector2? currentPosition = startPosition;
        
        Vector2 min = Vector2.one * MapData.MAPSIZE;
        byte length = 0;

        while (currentPosition.HasValue)
        {
            usedPositions.Add(currentPosition.Value);
            blackList.Add(currentPosition.Value + edgeDirection);

            if (currentPosition.Value.magnitude < min.magnitude)
                min = currentPosition.Value;

            length++;

            Vector2 newPosition = currentPosition.Value + travelDirection;

            if(!usedPositions.Contains(newPosition) && IsValid(newPosition, edgeDirection))
            {
                currentPosition = newPosition;
            }
            else if(!usedPositions.Contains(startPosition - travelDirection) && IsValid(startPosition - travelDirection, edgeDirection))
            {
                currentPosition = startPosition - travelDirection;
                travelDirection = -travelDirection;
            }
            else
            {
                currentPosition = null;
            }
        }

        AddConnection(new Connection((short)(min.x + offset.x), (short)(min.y + offset.y), length, regionDirection));
    }
    private Connection.Direction GetRegionDirection(Vector2 edgeDirection)
    {
        if (edgeDirection == Vector2.right || edgeDirection == Vector2.left)
            return Connection.Direction.Up;
        if (edgeDirection == Vector2.up || edgeDirection == Vector2.down)
            return Connection.Direction.Right;

        throw new System.Exception("Cannot get region directions");
    }
    private bool IsValid(Vector2 position, Vector2 edgeDirection)
    {
        return _ownedPositions.Contains(position) && IsValid(position + edgeDirection);
    }
    private bool IsValid(Vector2 position)
    {
        return !_ownedPositions.Contains(position) && MapData.IsPassable(position) && MapData.IsValidPosition(position);
    }
    private Vector2 GetTravelDirection(Vector2 edgeDirection)
    {
        if (edgeDirection == Vector2.right || edgeDirection == Vector2.left)
            return Vector2.up;
        if (edgeDirection == Vector2.up || edgeDirection == Vector2.down)
            return Vector2.right;

        throw new System.Exception("Cannot get travel directions");
    }
    private Vector2 GetOffset(Vector2 edgeDirection)
    {
        if (edgeDirection == Vector2.right)
            return Vector2.right;
        else if (edgeDirection == Vector2.up)
            return Vector2.up;

        return Vector2.zero;
    }
    private void AddConnection(Connection regionConnection)
    {
        if(!_connections.Contains(regionConnection))
            _connections.Add(regionConnection);
    }
    private void RecalculateBounds()
    {
        if (_ownedPositions.Count <= 0)
            return;

        float xMin = _ownedPositions.Min(x => x.x);
        float yMin = _ownedPositions.Min(x => x.y);
        float xMax = _ownedPositions.Max(x => x.x) + 1;
        float yMax = _ownedPositions.Max(x => x.y) + 1;

        _bounds = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
    }
    public static void CleanDirtyRegions()
    {
        while (_regionsToInitialize.Count > 0)
        {
            _regionsToInitialize.Dequeue().Clean();
        }
    }
    public override bool Equals(object obj)
    {
        if (obj is Region)
        {
            Region other = obj as Region;

            return _id.Equals(other._id);
        }

        return false;
    }
    public override int GetHashCode()
    {
        return _id.GetHashCode();
    }
    public override string ToString()
    {
        return string.Format("{0} - ({1}) - [{2}]", Bounds, _ownedPositions.Count, _id);
    }
    public struct Connection
    {
        public Connection(short x, short y, byte length, Direction direction)
        {
            _hash = new { x, y, length, direction }.GetHashCode();
            _x = x;
            _y = y;
            _length = length;
            _direction = direction;
        }

        public Vector2 Position { get { return new Vector2(_x, _y); } }
        public int Length { get { return _length; } }
        public Vector2 Normal { get { return (_direction == Direction.Right) ? Vector2.right : Vector2.up; } }

        private readonly int _hash;
        private readonly short _x;
        private readonly short _y;
        private readonly byte _length;
        private readonly Direction _direction;

        public override int GetHashCode()
        {
            return _hash;
        }
        public enum Direction : byte
        {
            Right = 0,
            Up = 1,
        }
        public static implicit operator int(Connection connection)
        {
            return connection._hash;
        }
    }
}
