using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class Plant : TileObject
{
    public bool hasBeenWatered;
    public GameObject plantTileToCreate;
    [SerializeField]
    public int height;

    public float minGrowTime = 20f;
    public float maxGrowTime = 60f;
    public float growTime;

    protected virtual void Start()
    {
        hasBeenWatered = false;
        ColorTiles();
        //x = (int)transform.position.x;
        //y = (int)transform.position.y;
    }

    public void WaterPlant()
    {
        Debug.Log("Plant has been watered at " + x + ", " + y);
        hasBeenWatered = true;
    }

    public virtual void Grow(string name)
    {
        
    }

    void ColorTiles()
    {
        int leftbound = Mathf.Max(x - 2, 0);
        int rightBound = Mathf.Min(x + 2, GameManager.instance.GetXBound() - 1);
        int upBound = Mathf.Max(y - 2, 0);
        int downBound = Mathf.Min(y + 2, GameManager.instance.GetYBound() - 1);

        for (int i = leftbound; i <= rightBound; i++)
        {
            for(int j = upBound; j <= downBound; j++)
            {
                if(!GameManager.instance.GetGridDataAtLocation(i, j).IsEmpty())
                {
                    if (GameManager.instance.GetGridDataAtLocation(i, j).environmentTile != null)
                    {
                        GameManager.instance.GetGridDataAtLocation(i, j).environmentTile.SetColored(true);
                    }
                }
                
            }
        }
    }
    
   /*
    public virtual void GrowClient()
    {
        //Grow the plant
    }*/

    public virtual void Remove()
    {
        //Notify Grid that this object is being destroyed
        GameManager.instance.RemoveTile(this);
        //RemoveInClient();
        //Destroy itself
        Destroy(gameObject);
    }

    /*
    [ClientRpc]
    public virtual void RemoveInClient()
    {
        if (isServer) return;

        GameManager.instance.RemoveTile(this);

        //Destroy itself
        Destroy(gameObject);
    }
    */

    public virtual void RemoveWithItemDrop()
    {
        //TODO: Figure out how to store self item reference
        ////TODO: Make random chance of item drop --------------------
        ////Drop Item into world
        //PickupItem pi = Instantiate(pickupItemPrefab, transform.position + (Vector3.up * 0.5f), Quaternion.identity).GetComponent<PickupItem>();
        //pi.Initialize(selfPrefabReference);

        //Notify Grid that this object is being destroyed
        GameManager.instance.RemoveTile(this);

        //Destroy itself
        Destroy(gameObject);
    }
}
