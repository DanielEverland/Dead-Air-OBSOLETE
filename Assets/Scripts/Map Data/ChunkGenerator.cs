using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator {

    private static Texture2D chunkTexture;
    private static TileType currentTile;

	public static void RenderChunk(Chunk chunk)
    {
        chunkTexture = new Texture2D(Chunk.PIXELS_PER_TILE * Chunk.CHUNK_SIZE, Chunk.PIXELS_PER_TILE * Chunk.CHUNK_SIZE);

        for (int x = 0; x < Chunk.CHUNK_SIZE; x++)
        {
            for (int y = 0; y < Chunk.CHUNK_SIZE; y++)
            {
                currentTile = TileType.AllTypes[chunk.Tiles[x, y]];

                chunkTexture.SetPixels(x * Chunk.PIXELS_PER_TILE, y * Chunk.PIXELS_PER_TILE, Chunk.PIXELS_PER_TILE, Chunk.PIXELS_PER_TILE, currentTile.Texture.GetPixels());
            }
        }

        chunkTexture.Apply();

        GameObject obj = new GameObject();
        SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>();
        renderer.sprite = Sprite.Create(chunkTexture, new Rect(0, 0, chunkTexture.width, chunkTexture.height), Vector2.one / 2);
    }
}
