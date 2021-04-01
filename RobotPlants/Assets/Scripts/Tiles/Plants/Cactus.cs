using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cactus : Seed
{
    public int distanceFromCenter = 0;

    //5 as "not set yet" value
    int branchDirection = 5;
    protected override void Start()
    {
        minGrowTime = 2;
        maxGrowTime = 5;
        base.Start();

    }


    public override void Grow(string name)
    {
        Debug.Log("are we starting to grow");
        if (GameManager.instance.GetTileUp(x, y).IsEmpty())
        {
            //Create plant and init
            if (GameManager.instance.GetTileUp(x, y).IsEmpty() && height < 6)
            {
                if ((height == 3 || height == 4))
                {
                    BranchOut();
                }

                if (distanceFromCenter == 0)
                {
                    Cactus newCactus = Instantiate(plantTileToCreate, new Vector3(x + 0.5f, (y + 1) + 0.5f, 0), Quaternion.identity).GetComponent<Cactus>();
                    newCactus.Init();
                    //Increase plant height attribute
                    newCactus.branchDirection = branchDirection;
                    newCactus.height = this.CaclulateHeight();
                    newCactus.distanceFromCenter = 0;
                    newCactus.plantTileToCreate = plantTileToCreate;
                    newCactus.hasBeenWatered = true;
                  
                }
            }
            
   
        }
    }

    private void BranchOut()
    {
        if((branchDirection == 5 || branchDirection == 0) && distanceFromCenter == 0)
        {
            branchDirection = Random.Range(-1, 2);
        } else if (distanceFromCenter == 0 && branchDirection != 5)
        {
            branchDirection = branchDirection *= -1;
        }

        switch(branchDirection)
        {
            case -1:
                if(GameManager.instance.GetTileLeft(x, y).IsEmpty() && distanceFromCenter > -2)
                {
                    Debug.Log("it should go left ... ");
                    Cactus newCactus = Instantiate(plantTileToCreate, new Vector3((x - 1) + 0.5f, y  + 0.5f, 0), Quaternion.identity).GetComponent<Cactus>();
                    newCactus.Init();
                    newCactus.plantTileToCreate = plantTileToCreate;
                    newCactus.branchDirection = branchDirection;
                    //Increase plant height attribute
                    newCactus.height = this.CaclulateHeight();
                    newCactus.distanceFromCenter = distanceFromCenter - 1;
                    newCactus.hasBeenWatered = true;
                } else if (GameManager.instance.GetTileUp(x, y).IsEmpty() && distanceFromCenter <= -2)
                {
                    Debug.Log("it should go left up... ");
                    Cactus newCactus = Instantiate(plantTileToCreate, new Vector3(x + 0.5f, (y + 1) + 0.5f, 0), Quaternion.identity).GetComponent<Cactus>();
                    newCactus.Init();
                    newCactus.plantTileToCreate = plantTileToCreate;
                    newCactus.branchDirection = branchDirection;
                    //Increase plant height attribute
                    newCactus.height = this.CaclulateHeight();
                    newCactus.distanceFromCenter = distanceFromCenter;
                    newCactus.hasBeenWatered = true;
                }
                break;
            case 1:
                if (GameManager.instance.GetTileLeft(x, y).IsEmpty() && distanceFromCenter < 2)
                {
                    Debug.Log("it should go right... ");
                    Cactus newCactus = Instantiate(plantTileToCreate, new Vector3((x + 1) + 0.5f, y + 0.5f, 0), Quaternion.identity).GetComponent<Cactus>();
                    newCactus.Init();
                    newCactus.plantTileToCreate = plantTileToCreate;
                    newCactus.branchDirection = branchDirection;
                    //Increase plant height attribute
                    newCactus.height = this.CaclulateHeight();
                    newCactus.distanceFromCenter = distanceFromCenter + 1;
                    newCactus.hasBeenWatered = true;
                }
                else if (GameManager.instance.GetTileUp(x, y).IsEmpty() && distanceFromCenter >= 2)
                {
                    Debug.Log("it should go right up... ");
                    Cactus newCactus = Instantiate(plantTileToCreate, new Vector3(x + 0.5f, (y + 1) + 0.5f, 0), Quaternion.identity).GetComponent<Cactus>();
                    newCactus.Init();
                    newCactus.plantTileToCreate = plantTileToCreate;
                    newCactus.branchDirection = branchDirection;
                    //Increase plant height attribute
                    newCactus.height = this.CaclulateHeight();
                    newCactus.distanceFromCenter = distanceFromCenter;
                    newCactus.hasBeenWatered = true;
                }
                break;
            case 0:
                break;
            default:
                break;
        }

    }

    private int CaclulateHeight()
    {
        Tile currentTile = this;
        while ((GameManager.instance.GetTileDown(currentTile.x, currentTile.y).objectTile is Cactus))
        {
            height += 1;
            currentTile = GameManager.instance.GetTileDown(currentTile.x, currentTile.y).objectTile;
        }
        return height;
    }

    public override void Remove()
    {
        base.Remove();
    }
}
