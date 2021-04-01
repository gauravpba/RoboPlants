using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeanStalkTop : Seed
{
    public GameObject stalk;


    public override void Grow(string name)
    {
        if (GameManager.instance.GetTileUp(x, y).IsEmpty())
        {

            gameObject.transform.SetPositionAndRotation(new Vector3(x + 0.5f, (y + height) + 0.5f, 0), Quaternion.identity);
            base.Start();
            //Create plant and init
            Plant stem = Instantiate(stalk, new Vector3(x + 0.5f, (y - 1) + 0.5f, 0), Quaternion.identity).GetComponent<Plant>();
            stem.Init();
            

            //Remove this seed
           
        }

    }

    public override void Remove()
    {
        base.Remove();
    }
}
