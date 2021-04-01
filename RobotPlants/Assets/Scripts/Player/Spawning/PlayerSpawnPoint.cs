using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerSpawnPoint : NetworkBehaviour
{
    #region Variables



    #endregion

    #region Unity Methods

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawIcon(transform.position, "AvatarSelector@2x");
        UnityEditor.Handles.Label(transform.position, "Player Spawn Point");
    }
#endif

    #endregion

    #region Custom Methods



    #endregion
}
