using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    #region Variables

    [Header("Camera Values")]
    Transform followTarget;
    float smoothTime = 0.2f;
    float maxSpeed = 400f;
    Vector3 camVelocity;

    [Header("Object References")]
    public Camera activeCam;
    [HideInInspector] public PlayerBody playerBody; //Set by the playerBodySetFollow

    #endregion

    #region Unity Methods

    private void Awake()
    {
        activeCam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        UpdatePosition();
    }

    #endregion

    #region Custom Methods

    public void UpdatePosition()
    {
        //Smoothdamp position towards followTarget x/y
        Vector3 targetPosition = Vector3.SmoothDamp(transform.position, new Vector3(followTarget.position.x, followTarget.position.y, -10f), ref camVelocity, smoothTime, maxSpeed, Time.deltaTime);

        float targetPositionWithinBoundsX = Mathf.Min(Mathf.Max(targetPosition.x, 10), 118);
        float targetPositionWithinBoundsY = Mathf.Min(Mathf.Max(targetPosition.y, 5), 27);

        Vector3 targetPositionWithinBounds = new Vector3(targetPositionWithinBoundsX, targetPositionWithinBoundsY, -10f);

        transform.position = targetPositionWithinBounds;
    }

    public void SetFollowTarget(Transform t)
    {
        followTarget = t;
    }

    #endregion
}
