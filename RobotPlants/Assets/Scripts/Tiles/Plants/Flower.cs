using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : Plant
{
    #region Variables

    public List<Sprite> flowerSprites;

    #endregion

    #region Unity Methods

    protected override void Start()
    {
        base.Start();

        GetComponent<SpriteRenderer>().sprite = flowerSprites[Random.Range(0, flowerSprites.Count)];
    }

    #endregion

    #region Custom Methods

    public override void Grow(string name)
    {
        //Do nothing. This flower does not grow anymore.
    }
    /*
    public override void GrowClient()
    {
        //Do nothing. This flower does not grow anymore.
    }
    */
    #endregion

}
