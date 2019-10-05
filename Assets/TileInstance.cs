using System;
using UnityEngine;

public class TileInstance
{
    protected GameObject tilePrefab;
    public GameObject TilePrefab
    {
        get { return tilePrefab; }
    }
    protected Vector2Int position;
    public Vector2Int Position
    {
        get { return position; }
    }
    protected TileData baseData;
    public TileData BaseData
    {
        get { return baseData; }
    }

    public TileInstance(Vector2Int pos, GameObject tilePrefab, TileData baseData)
    {
        this.position = pos;
        this.tilePrefab = tilePrefab;
        this.baseData = baseData;
    }
}
