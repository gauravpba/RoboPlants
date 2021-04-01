using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickupItem : NetworkBehaviour
{
    #region Variables

    [Tooltip("The prefab of the Tile object that should be placed when the PickupItem is used by the player.")]
    public GameObject tilePrefab;

    #endregion

    #region Unity Methods



    #endregion

    #region Custom Methods

    public void Initialize(GameObject newTilePrefab)
    {
        tilePrefab = newTilePrefab;
    }

    #endregion
}
