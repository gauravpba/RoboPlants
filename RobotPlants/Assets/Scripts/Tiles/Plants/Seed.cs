using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Seed : Plant
{
    protected override void Start()
    {
        base.Start();

        Debug.Log("who's there?????");

        this.Init();

        //Wait for range growtime amount of seconds, then grow if watered
        //If grow was called, stop the timer
        TimerUtility timer = gameObject.AddComponent<TimerUtility>();
        timer.OnTimeExpired.AddListener(delegate {
            Debug.Log("time has expired");
            //If the plant has been watered, grow
            Grow(plantTileToCreate.name);
        });

        //Start timer with range growtime
        growTime = Random.Range(minGrowTime, maxGrowTime);
        timer.StartIntervalConditionalTimer(growTime, delegate { return hasBeenWatered; });
    }


    public override void Grow(string name)
    {


        //If the tile above this tile is empty
        if (GameManager.instance.GetTileUp(x,y).IsEmpty()) 
        {
            //Create plant and init
            Plant newPlant;
            
            newPlant = Instantiate(plantTileToCreate,
                    new Vector3(x + 0.5f, (y + 1) + 0.5f, 0), Quaternion.identity).GetComponent<Plant>();
            newPlant.Init();

            newPlant.plantTileToCreate = plantTileToCreate;

            //Increase plant height attribute
            newPlant.height = height + 1;
            

            //Notify clients of new server object (make sure spawned object is in NetManager spawnPrefabs
            //NetworkServer.Spawn(newPlant.gameObject);

            //Remove this seed
            Remove();
        }
    }

    /*
    public override void GrowClient()
    {
        //If the tile above this tile is empty
        if (GameManager.instance.GetTileUp(x, y).IsEmpty())
        {
            //Create plant and init
            Plant newPlant = Instantiate(plantTileToCreate, new Vector3(x + 0.5f, (y + height) + 0.5f, 0), Quaternion.identity).GetComponent<Plant>();
            newPlant.Init();

            //Increase plant height attribute
            newPlant.height = height + 1;

            //Remove this seed
            Remove();
        }
    }*/
}
