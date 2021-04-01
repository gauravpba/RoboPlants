using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[RequireComponent(typeof(SpriteRenderer))]
public abstract class Tile : NetworkBehaviour
{
    [HideInInspector] public int x;
    [HideInInspector] public int y;

    public GameObject pickupItemPrefab;

    public TileType tileType;

    public enum TileType
    {
        Empty,
        Transparent,
        Solid
    }

    public void Init()
    {
        x = (int)transform.position.x;
        y = (int)transform.position.y;

        GameManager.instance.UpdateGrid(this);
    }
}
