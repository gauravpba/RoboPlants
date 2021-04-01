using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    #region Variables

    [Header("Parallax Bound Floats")]
    float leftX = 34.1f;
    float bottomY = 16.39f;
    float rightX = 26f;
    float topY = 16.39f;

    [Header("Player Bound Floats")]
    float playerLeftX = 0f;
    float playerbottomY = 0f;
    float playerRightX = 127f;
    float playerTopY = 31f;

    PlayerBody playerBody;

    #endregion

    #region Unity Methods

    private void LateUpdate()
    {
        Parallax();
    }

    #endregion

    #region Custom Methods

    void Parallax()
    {
        if (playerBody == null)
        {
            playerBody = FindObjectOfType<PlayerBody>();
            Debug.LogWarning("NO PLAYERBODY FOUND");
            if (playerBody == null) return; //If you didn't find the playerbody, stop
        }

        Vector3 playerPos = playerBody.transform.position;

        float targetXPos = Mathf.Lerp(leftX, rightX, playerPos.x / (playerRightX - playerLeftX));
        float targetYPos = Mathf.Lerp(bottomY, topY, playerPos.y / (playerTopY - playerbottomY));

        transform.position = new Vector3(targetXPos, targetYPos, 0f);
    }

    #endregion
}
