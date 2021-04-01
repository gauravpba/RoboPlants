using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTile : TransparentEnvironmentTile
{
    #region Variables

    Animator animator;
    float animTime = 0f;

    #endregion

    #region Unity Methods

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    protected override void Start()
    {
        //Determine sprite animation
        SetColored(isColored);
    }

    private void Update()
    {
        //Animation syncing
        animator.SetFloat("animTime", animTime);
        animTime = ((animTime + Time.deltaTime) % 3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("REFILL THE PLAYER'S WATER");
            other.gameObject.GetComponent<PlayerBody>().RefillWater();
        }
    }

    #endregion

    #region Custom Methods

    public override void SetColored(bool coloredValue)
    {
        //Change currentSpriteList 
        if (coloredValue) currentSpriteList = coloredSprites;
        else currentSpriteList = uncoloredSprites;

        isColored = coloredValue;

        animator.SetBool("isColored", isColored);
    }

    #endregion
}
