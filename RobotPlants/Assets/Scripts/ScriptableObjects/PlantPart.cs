using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlantPart", menuName = "ScriptableObjects/PlantParts", order = 1)]
public class PlantPart : ScriptableObject
{
    public char type;
    public Collider2D collider;

    public Sprite spr;
}
