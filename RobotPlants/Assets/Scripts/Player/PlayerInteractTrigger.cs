using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractTrigger : MonoBehaviour
{
    #region Variables

    [Header("Component References")]
    PlayerBody playerBody;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        playerBody = GetComponentInParent<PlayerBody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collided with: " + other.gameObject.name);
        PickupItem pickupItem = other.gameObject.GetComponent<PickupItem>();
        if (pickupItem != null) playerBody.AddInteractable(pickupItem);
    }

    private void OnTriggerExit(Collider other)
    {
        PickupItem pickupItem = other.gameObject.GetComponent<PickupItem>();
        if (pickupItem != null) playerBody.RemoveInteractable(pickupItem);
    }

    #endregion

    #region Custom Methods

    public void SetSide(bool rightSide)
    {
        transform.localPosition = new Vector3(rightSide ? (1.5f) : (-1.5f), 0f, 0f);
    }

    #endregion
}
