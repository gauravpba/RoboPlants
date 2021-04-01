using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelCreator))]
public class LevelCreatorEditor : Editor
{
    bool placeMode = false;
    Vector2 mousePosition;
    Vector3 mouseWorldPos;
    LevelCreator levelCreator;

    void OnSceneGUI()
    {
        if(placeMode)
        {
            TrackMousePos();

            bool leftMouseDown = false;
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                //Debug.Log("Left-Mouse Down");
                leftMouseDown = true;
            }

            bool rightMouseDown = false;
            if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
            {
                //Debug.Log("Right-Mouse Down");
                rightMouseDown = true;
            }

            //Place a prefab (if it exists) with left click
            if (levelCreator.selectedPrefab != null && leftMouseDown) levelCreator.PlacePrefab(new Vector3(Mathf.Floor(mouseWorldPos.x) + 0.5f, Mathf.Floor(mouseWorldPos.y) + 0.5f, 0f));

            //Remove tile with right click
            if (rightMouseDown) levelCreator.RemovePrefab(new Vector3(Mathf.Floor(mouseWorldPos.x) + 0.5f, Mathf.Floor(mouseWorldPos.y) + 0.5f, 0f));

            //Stop mouse input in place mode
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(placeMode ? FocusType.Passive : FocusType.Keyboard));
        }
    }

    void TrackMousePos()
    {
        // Update mouse grid position.
        Event e = Event.current;
        mousePosition = e.mousePosition;
        mouseWorldPos = HandleUtility.GUIPointToWorldRay(mousePosition).origin;
        mouseWorldPos = new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0f);
        //Debug.Log(mouseWorldPos);
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        //Get script reference
        levelCreator = (LevelCreator)target;

        //Toggle
        placeMode = GUILayout.Toggle(placeMode, "Place Mode");

        //Selected prefab label
        EditorGUILayout.HelpBox("Selected prefab: " + (levelCreator.selectedPrefab == null ? "NULL" : (levelCreator.selectedPrefab.gameObject.name)), MessageType.Info);

        //TileObject buttons
        GUILayout.Label("TileObject Palette");
        for (int i = 0; i < levelCreator.tileObjectPalette.Count; i++)
        {
            if (GUILayout.Button(levelCreator.tileObjectPalette[i].gameObject.name))
            {
                int val = 0 + i;
                levelCreator.selectedPrefab = levelCreator.tileObjectPalette[val];
            }
        }

        //Environment tile buttons
        GUILayout.Label("Environment Palette");
        for (int i = 0; i < levelCreator.environmentTilePalette.Count; i++)
        {
            if (GUILayout.Button(levelCreator.environmentTilePalette[i].gameObject.name))
            {
                int val = 0 + i;
                levelCreator.selectedPrefab = levelCreator.environmentTilePalette[val];
            }
        }
    }
}
