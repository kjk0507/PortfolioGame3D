using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject[] prefabTile;

    Dictionary<Vector2Int, GameObject> tiles;

    public void Init(int width, int height)
    {
        width = width / 50;
        height = height / 50;
        //width = height / 50;
        //height = width / 50;

        tiles = new Dictionary<Vector2Int, GameObject>(width * height);
        for (int y = 0; y < height ; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int idx = x + y * width;
                int nTileIdx = 0;
                if (idx % 2 == 0) nTileIdx = 1;
                GameObject objTile = Instantiate(prefabTile[nTileIdx], this.transform);
                Vector2Int vTilePos = new Vector2Int(50 * x, 50 * y);
                objTile.name = vTilePos.ToString();
                objTile.transform.localPosition = new Vector3(50 * x - 365, 50 * y - 120, 0);
                tiles.Add(vTilePos, objTile);
            }
        }
    }
}
