using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentTile : Tile
{
    public bool canInteract;

    public List<Sprite> coloredSprites;
    public List<Sprite> uncoloredSprites;
    protected List<Sprite> currentSpriteList; //Holds a reference to either coloredSPrites or uncoloredSprites
    protected int spriteIndex; //Index of the used sprite
    protected bool isColored = false;

    protected SpriteRenderer spriteRenderer;

    private void Awake()
    {
        GetComponent<SpriteRenderer>();
    }
    protected virtual void Start()
    {
        //Determine sprite
        spriteIndex = Random.Range(0, coloredSprites.Count);
        SetColored(isColored);
    }

    public virtual void SetColored(bool coloredValue)
    {
        //Change currentSpriteList 
        if (coloredValue) currentSpriteList = coloredSprites;
        else currentSpriteList = uncoloredSprites;

        isColored = coloredValue;

        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        if (currentSpriteList[spriteIndex] != null)
            spriteRenderer.sprite = currentSpriteList[spriteIndex];
    }
}
