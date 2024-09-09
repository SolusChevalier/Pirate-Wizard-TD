using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Tile : MonoBehaviour
{
    #region FIELDS

    public TileType type;
    public CoordStruct coord;
    public Transform TopSpawnPoint => GetTransformInChildrenWithTag("TopSpawnPoint");
    public GameObject _tileObject => gameObject;
    public Transform SideA => GetTransformInChildrenWithTag("TileSideA");
    public Transform SideB => GetTransformInChildrenWithTag("TileSideB");
    public Transform SideC => GetTransformInChildrenWithTag("TileSideC");
    public Transform SideD => GetTransformInChildrenWithTag("TileSideD");

    public bool isWalkable => type == TileType.Path;
    public bool isBuildable => type == TileType.PathBorder;

    #endregion FIELDS

    #region UNITY METHODS

    public void Start()
    {
    }

    public void OnMouseDown()
    {
        if (isBuildable)
        {
            EventManager.OnTileClicked?.Invoke(this);
        }
    }

    #endregion UNITY METHODS

    #region METHODS

    public void SetType(TileType newType)
    {
        type = newType;
    }

    private Transform GetTransformInChildrenWithTag(string tag)
    {
        foreach (Transform child in _tileObject.GetComponentsInChildren<Transform>())
        {
            if (child.tag == tag)
            {
                return child;
            }
        }

        return null;
    }

    #endregion METHODS
}