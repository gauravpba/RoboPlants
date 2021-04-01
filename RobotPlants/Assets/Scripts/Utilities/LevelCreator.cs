using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.SceneManagement;
#endif

public class LevelCreator : MonoBehaviour
{
    #region Variables

    [Header("Prefab Palettes")]
    public List<GameObject> tileObjectPalette;
    public List<GameObject> environmentTilePalette;

    [HideInInspector] public GameObject selectedPrefab; //Currently selected prefab
    Vector3 targetLocation;

    #endregion

    #region Unity Methods



    #endregion

    #region Custom Methods

    public void PlacePrefab(Vector3 targetPosition)
    {
#if UNITY_EDITOR
        //If targetPosition is out of bounds (min only), don't do anything
        if (targetPosition.x < 0 || targetPosition.y < 0) return;

        Tile[] tileList = FindObjectsOfType<Tile>();

        for (int i = 0; i < tileList.Length; i++)
        {
            //If a tile of the same type exists at the targetPosition
            if (tileList[i].transform.position.Equals(targetPosition))
            {
                if((tileList[i] is TileObject && selectedPrefab.GetComponent<Tile>() is TileObject) //Check for both plants
                    || ((tileList[i] is EnvironmentTile && selectedPrefab.GetComponent<Tile>() is EnvironmentTile))) return; //Check for both EnvironmentTiles
            }
        }

        //Otherwise, instantiate the prefab
        GameObject instantiatedPrefab = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab, SceneManager.GetActiveScene());
        instantiatedPrefab.transform.position = targetPosition;
#endif
    }

    public void RemovePrefab(Vector3 targetPosition)
    {
#if UNITY_EDITOR
        //If targetPosition is out of bounds (min only), don't do anything
        if (targetPosition.x < 0 || targetPosition.y < 0) return;

        Tile[] tileList = FindObjectsOfType<Tile>();

        for (int i = 0; i < tileList.Length; i++)
        {
            //If this is the targeted tile
            if(tileList[i].transform.position.Equals(targetPosition))
            {
                //Destroy it
                DestroyImmediate(tileList[i].gameObject);
                return;
            }
        }
#endif
    }

    #endregion
}
