using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameManager gameManager;
    public List<BuildingTile> tiles = new List<BuildingTile>();
    public Dictionary<CoordStruct, BuildingTile> PosTileDict = new Dictionary<CoordStruct, BuildingTile>();
    public Camera mainCamera;
    private Ray ray;
    private RaycastHit hit;
    public BuildingTile selectedTile;

    private void Start()
    {
    }

    private void Awake()
    {
        EventManager.OnMapGenerated += GameStart;
        StartCoroutine(WaitReset(0.75f));//waits untill the game is ready to reset the tiles
    }

    private void Update()
    {
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);//creates a ray from the camera to the mouse position
        foreach (BuildingTile tile in tiles)//loops through all the tiles
        {
            if (!tile.selectable)//if the tile is not selectable
            {
                tile.StopHover();//stop the hover
            }
            else//if the tile is selectable
            {
                tile.Hover();//start the hover
            }
        }

        if (Physics.Raycast(ray, out hit))//if the ray hits something
        {
            if (hit.collider.CompareTag("BorderTile") || hit.collider.CompareTag("Defender"))//if the thing hit is a tile
            {
                //Debug.Log(hit.collider.tag);
                BuildingTile tile;
                if (hit.collider.CompareTag("Defender"))
                    tile = GetTile(hit.collider.GetComponent<Defender>().Coord);//get the tile that was hit
                else
                    tile = hit.collider.GetComponent<BuildingTile>();//get the tile that was hit

                //Debug.Log(tile.properties.hover);
                if (Input.GetButtonDown("Fire1"))//if the left mouse button is clicked and the tile is selectable
                {
                    //Debug.Log("Tile Clicked");
                    if (selectedTile == null)//if there is no selected tile then select the tile
                    {
                        selectedTile = tile;
                        tile.selectable = true;
                        tile.Select();
                        EventManager.OnTileSelected?.Invoke(tile);
                    }
                    else if (selectedTile == tile)//if the selected tile is the same as the clicked tile then deselect the tile
                    {
                        selectedTile.selectable = false;
                        selectedTile = null;
                        tile.Select();
                        resetTiles();
                        EventManager.OnTileDeselect?.Invoke();
                    }
                    else//if the selected tile is not the same as the clicked tile set the clicked one as the target tile
                    {
                        selectedTile.selectable = false;
                        selectedTile = tile;
                        tile.selectable = true;
                        tile.Select();
                        EventManager.OnTileSelected?.Invoke(tile);
                    }
                }
                if (!tile.properties.hover || tile.selectable)//if the tile is not hovered or the tile is selectable
                {
                    tile.Hover();//start the hover
                }
            }
        }
        if (Input.GetButtonDown("Fire2"))//if the right mouse button is clicked
        {
            resetTiles();//reset all tiles
            EventManager.OnTileDeselect?.Invoke();
        }
    }

    public void GameStart()
    {
        //Debug.Log("Game Started");
        //Debug.Log(tiles.Count);
        foreach (BuildingTile tile in tiles)
        {
            tile.selectable = true;
        }
    }

    //this function will reset all tiles to their default state
    public void resetTiles()
    {
        foreach (BuildingTile tile in tiles)
        {
            tile.StopHover();
        }
        selectedTile = null;
    }

    //this function gets tiles
    public BuildingTile GetTile(CoordStruct pos)
    {
        return PosTileDict[pos];
    }

    public IEnumerator WaitReset(float time)
    {
        yield return new WaitForSeconds(time);
        resetTiles();
    }

    public CoordStruct KeyByValue(BuildingTile value)
    {
        foreach (KeyValuePair<CoordStruct, BuildingTile> entry in PosTileDict)
        {
            if (entry.Value == value)
            {
                return entry.Key;
            }
        }
        return new CoordStruct(-1, -1);
    }

    public void ResetTileSelectable()
    {
        foreach (BuildingTile tile in tiles)
        {
            tile.selectable = false;
        }
    }
}