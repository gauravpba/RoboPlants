using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Mirror;

public class GameManager: NetworkBehaviour
{
    GridData[,] grid;
    int xBound = 128;
    int yBound = 32;

    [SerializeField]List<GameObject> pickups = new List<GameObject>();

    public class GridData
    {
        public TileObject objectTile = null;
        public EnvironmentTile environmentTile = null;

        public bool IsEmpty()
        {
            bool result = true;
            if(objectTile != null) result &= (objectTile.tileType == Tile.TileType.Empty);
            if(environmentTile != null) result &= (environmentTile.tileType == Tile.TileType.Empty);
            return result;
        }

        public bool IsSolid()
        {
            bool result = false;
            if (objectTile != null) result |= (objectTile.tileType == Tile.TileType.Solid);
            if (environmentTile != null) result |= (environmentTile.tileType == Tile.TileType.Solid);
            return result;
        }
    }

   

    public static GameManager instance = null; //Singleton object reference

    private void Awake()
    {
        #region Singleton

        //If instance doesn't already exist
        if (instance == null)
        {
            //Set instance to this
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (instance != this) //Else if instance already exists and it's not this
        {
            //Destroy this
            Destroy(gameObject);
        }

        #endregion
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("are we doing this??");
        Tile[] tiles = FindObjectsOfType<Tile>();
        Debug.Log("are we doing this??" + tiles.Length) ;
        grid = new GridData[xBound, yBound];

        //Initialize GridData
        for (int i = 0; i < xBound; i++)
        {
            for (int j = 0; j < yBound; j++)
            {
                grid[i, j] = new GridData();
            }
        }

        //Initialize Tiles
        for (int i = 0; i < tiles.Length; i++)
        {
            tiles[i].Init();
        }

        //Initialize canInteract values for EnvironmentTiles
        for (int i = 0; i < xBound; i++)
        {
            for (int j = 0; j < yBound; j++)
            {
                //If the environment tile
                if(grid[i, j].environmentTile != null)
                {
                    //If environment tile is solid and the one above it is empty
                    grid[i, j].environmentTile.canInteract = (!(IsInvalidGridPosition(i,j)) && !(IsInvalidGridPosition(i, j + 1)) && grid[i, j].IsSolid() && grid[i, j + 1].IsEmpty());
                }
            }
        }

        Transform[] allChildren = GameObject.Find("SpawnPoints").GetComponentsInChildren<Transform>();
        foreach (Transform obj in allChildren)
        {

            int index = Random.Range(0, pickups.Count);

            Instantiate(pickups[index], obj.transform.position, Quaternion.identity);


        }


    }

    #region Relative Tile Getters

    public GridData GetTileUp(int x, int y)
    {
        if (IsInvalidGridPosition(x, y)) return null;
        Debug.Log("x,y: " + x + " ," + (y-1));
        return grid[x, y + 1];
    }

    public GridData GetTileDown(int x, int y)
    {
        if (IsInvalidGridPosition(x, y)) return null;
        return grid[x, y - 1];
    }

    public GridData GetTileLeft(int x, int y)
    {
        if (IsInvalidGridPosition(x, y)) return null;
        return grid[x - 1, y];
    }

    public GridData GetTileRight(int x, int y)
    {
        if (IsInvalidGridPosition(x, y)) return null;
        return grid[x + 1, y];
    }

    #endregion

    public void UpdateGrid(Tile newTile)
    {
        //If the tile is null, do nothing
        if (newTile == null) return;

        if (newTile is TileObject)
        {
            grid[newTile.x, newTile.y].objectTile = (TileObject)newTile;
        }
        else if (newTile is EnvironmentTile)
        {
            //Debug.Log("xy: " + newTile.x + ", " + newTile.y);
            grid[newTile.x, newTile.y].environmentTile = (EnvironmentTile)newTile;
            //Debug.Log("envTile: " + grid[newTile.x, newTile.y].environmentTile != null);
        }
    }

    public void RemoveTile(Tile tileToRemove)
    {
        //If the tile is null, do nothing
        if (tileToRemove == null) return;

        if (tileToRemove is TileObject)
        {
            grid[tileToRemove.x, tileToRemove.y].objectTile = null;
        }
        else if (tileToRemove is EnvironmentTile)
        {
            grid[tileToRemove.x, tileToRemove.y].environmentTile = null;
        }
    }

    public int GetXBound()
    {
        return xBound;
    }

    public int GetYBound()
    {
        return yBound;
    }

    public GridData GetGridDataAtLocation(int x, int y)
    {
        if(IsInvalidGridPosition(x, y)) return null;

        return grid[x, y];
    }

    bool IsInvalidGridPosition(int x, int y)
    {
        //Return null if the coords are out of grid bounds
        return (x < 0 || x >= xBound || y < 0 || y >= yBound);
    }
}
