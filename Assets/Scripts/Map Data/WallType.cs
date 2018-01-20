using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WallType : TileType {

    public new static List<WallType> AllTypes
    {
        get
        {
            if (allTileTypes == null)
                LoadAllTileTypes();

            return allTileTypes;
        }
    }
    private static List<WallType> allTileTypes;

    public new static void LoadAllTileTypes()
    {
        allTileTypes = new List<WallType>(TileType.AllTypes.Where(x => x is WallType).Select(x => x as WallType));
    }
#if UNITY_EDITOR
    [MenuItem("Assets/Create/Wall Type")]
    private static void CreateNewTileType()
    {
        if (!Utility.GetCurrentlySelectedFolder().Contains("Resources/Tile Types"))
        {
            Debug.LogWarning(@"You've created a tile type outside of its resources folder. It will not be accessible during playtime. Please create it in ""Resources/Tile Type""");
        }

        WallType newWall = Utility.CreateAssetAndRename<WallType>();

        newWall.IsSpawnable = false;
    }
#endif
}
