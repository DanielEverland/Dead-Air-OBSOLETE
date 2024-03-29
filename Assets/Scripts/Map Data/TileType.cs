using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileType : ScriptableObject {
    
    public static List<TileType> AllTypes
    {
        get
        {
            if (allTileTypes == null)
                LoadAllTileTypes();

            return allTileTypes;
        }
    }
    private static List<TileType> allTileTypes;

    public static bool LoadAllTileTypes()
    {
        allTileTypes = new List<TileType>(Resources.LoadAll<TileType>("Tile Types"));

        for (byte i = 0; i < allTileTypes.Count; i++)
        {
            allTileTypes[i].ID = i;
        }

        return true;
    }
    
    public bool IsPassable { get { return _isPassable; } set { _isPassable = value; } }
    public bool IsSpawnable { get { return _isSpawnable; } set { _isSpawnable = value; } }
    public byte ID { get; private set; }
    public Sprite Sprite { get { return _appearance; } }
    public Texture2D Texture
    {
        get
        {
            if (texture == null)
                CreateTexture();

            return texture;
        }        
    }
    public Material Material
    {
        get
        {
            if (material == null)
                CreateMaterial();

            return material;
        }
    }

    private Texture2D texture;
    private Material material;

    [SerializeField]
    private Sprite _appearance;
    [SerializeField]
    private bool _isSpawnable;
    [SerializeField]
    private bool _isPassable;

    private void Load()
    {
        CreateTexture();
        CreateMaterial();
    }
    private void CreateMaterial()
    {
        material = new Material(GlobalObjects.GetObject<Material>("ChunkMaterial"));
        material.mainTexture = Texture;
    }
    private void CreateTexture()
    {
        Rect spriteRect = Sprite.textureRect;
        texture = new Texture2D(Mathf.RoundToInt(spriteRect.width), Mathf.RoundToInt(spriteRect.height));

        Color[] colors = Sprite.texture.GetPixels(Mathf.RoundToInt(spriteRect.x), Mathf.RoundToInt(spriteRect.y), Mathf.RoundToInt(spriteRect.width), Mathf.RoundToInt(spriteRect.height));
        texture.SetPixels(colors);
        texture.Apply();
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create/Tile Type")]
    private static void CreateNewTileType()
    {
        if (!Utility.GetCurrentlySelectedFolder().Contains("Resources/Tile Types"))
        {
            Debug.LogWarning(@"You've created a tile type outside of its resources folder. It will not be accessible during playtime. Please create it in ""Resources/Tile Type""");
        }

        Utility.CreateAssetAndRename<TileType>();
    }
#endif
}
