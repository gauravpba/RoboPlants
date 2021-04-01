using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerInstantiator : MonoBehaviour
{
    #region Variables

    public GameObject prefabToServerSpawn;

    #endregion

    #region Unity Methods



    #endregion

    #region Custom Methods

    //This is called in the GameManager Start()
    public void SpawnPrefab()
    {
        //Instantiate prefab


        //Spawn on server


        //Destroy this object
        Destroy(gameObject);
    }

    #endregion
}
