using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleOneshot : MonoBehaviour
{
    #region Variables

    ParticleSystem ps;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        //If the particle has finished, destroy this gameObject
        if(!ps.IsAlive())
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Custom Methods



    #endregion
}
