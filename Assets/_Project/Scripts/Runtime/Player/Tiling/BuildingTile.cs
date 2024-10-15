using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BuildingTile : MonoBehaviour
{
    public TileProperties properties;
    public Transform placementPoint;
    private Vector3 _progression;
    private readonly Vector3 _speedIncrement = Vector3.one;
    private Vector3 _motionEquation;
    private Vector3 _HoverAnchorLow, _HoverTarget, _HoverAnchorHigh, _HoverOverPos;
    public bool selectable = false;
    public SelectionState selectionState = SelectionState.Inert;
    public HoverState hoverState = HoverState.Static;
    private bool _Home = true;

    private void Awake()//starting properties
    {
        properties = new TileProperties();
        properties.startProps(transform.position, placementPoint);
        _motionEquation = new Vector3(Mathf.Floor(UnityEngine.Random.Range(0, 3)), Mathf.Floor(UnityEngine.Random.Range(0, 3)), Mathf.Floor(UnityEngine.Random.Range(0, 3)));
        _HoverAnchorLow = properties.StartPos + new Vector3(0, 0.2f, 0);
        _HoverAnchorHigh = properties.StartPos + new Vector3(0, 0.4f, 0);
        _HoverOverPos = properties.StartPos + new Vector3(0, 0.3f, 0);
        _HoverTarget = _HoverAnchorLow;
        selectionState = SelectionState.Inert;
        hoverState = HoverState.Static;
        selectable = false;
    }

    private void Update()
    {
        _progression += Time.deltaTime * _speedIncrement;
        //Movement();
    }

    public void Hover()//starts the tile hover
    {
        if (!properties.canHover)
            return;
        properties.hover = true;
        StartCoroutine(HoverCo());
    }

    public void flipHoverHeight()//flips the hover height
    {
        properties.highLow = !properties.highLow;

        if (properties.highLow)
            HoverHigh();
        else
            HoverLow();
    }

    public void HoverLow()//tile hovers low when selectable
    {
        if (!properties.canHover)
            return;
        _HoverTarget = _HoverAnchorLow;
        //selectable = false;
    }

    public void HoverHigh()//tile hovers high when selected
    {
        if (!properties.canHover)
            return;
        _HoverTarget = _HoverAnchorHigh;
        //selectable = true;
    }

    public void StopHover()//stops the hover
    {
        properties.hover = false;
        //HoverLow();
        selectable = false;
        StopCoroutine(HoverCo());
        transform.position = Vector3.Lerp(transform.position, properties.StartPos, Time.deltaTime * 5f);
        HoverLow();
    }

    public IEnumerator HoverCo()//tile hover state, will be implemented later
    {
        while (properties.hover && properties.canHover)
        {
            //lerp to hover target - which bobs slightly
            transform.position = Vector3.Lerp(transform.position, new Vector3(
                    _HoverTarget.x, _HoverTarget.y + TrigMotionEquations((int)_motionEquation.y, _progression.y, 0.15f, 0.05f), _HoverTarget.z), Time.deltaTime * 5f);
            /*transform.position = Vector3.Lerp(transform.position, new Vector3(
                    HoverTarget.x + TrigMotionEquations((int)_motionEquation.x, _progression.x, 0.2f,
                        0.2f),
                    HoverTarget.y + TrigMotionEquations((int)_motionEquation.y, _progression.y, 0.2f,
                        0.2f),
                    HoverTarget.z + TrigMotionEquations((int)_motionEquation.z, _progression.z, 0.2f,
                        0.2f)), Time.deltaTime * 5f);*/
            yield return null;
        }
    }

    public void SetSelectionSate(SelectionState state)//tile selection state will be implemented later
    {
        if (state == SelectionState.Inert)
        {
            selectionState = SelectionState.Inert;
            hoverState = HoverState.Static;
        }
        if (state == SelectionState.NotSelectable)
        {
            selectionState = SelectionState.NotSelectable;
            hoverState = HoverState.Locked;
        }
        if (state == SelectionState.Selectable)
        {
            selectionState = SelectionState.Selectable;
            hoverState = HoverState.Low;
        }
        if (state == SelectionState.Selected)
        {
            selectionState = SelectionState.Selected;
            hoverState = HoverState.High;
        }
    }

    public void Movement()//tile hover state, will be implemented later
    {
        if (hoverState == HoverState.Locked | hoverState == HoverState.Static)
        {
            if (_Home)
                return;
            transform.position = Vector3.Lerp(transform.position, properties.StartPos, Time.deltaTime * 5f);
            _Home = true;
            return;
        }
        if (hoverState == HoverState.HoverOver)
        {
            if (_Home)
                _Home = false;
            transform.position = Vector3.Lerp(transform.position, new Vector3(
                _HoverOverPos.x, _HoverOverPos.y + TrigMotionEquations((int)_motionEquation.y, _progression.y, 0.15f, 0.05f), _HoverOverPos.z),
                Time.deltaTime * 5f);
        }
        if (hoverState == HoverState.Low)
        {
            if (_Home)
                _Home = false;
            transform.position = Vector3.Lerp(transform.position, new Vector3(
                _HoverAnchorLow.x, _HoverAnchorLow.y + TrigMotionEquations((int)_motionEquation.y, _progression.y, 0.15f, 0.05f), _HoverAnchorLow.z),
                Time.deltaTime * 5f);
        }
        if (hoverState == HoverState.High)
        {
            if (_Home)
                _Home = false;
            transform.position = Vector3.Lerp(transform.position, new Vector3(
                _HoverAnchorHigh.x, _HoverAnchorHigh.y + TrigMotionEquations((int)_motionEquation.y, _progression.y, 0.15f, 0.05f), _HoverAnchorHigh.z),
                Time.deltaTime * 5f);
        }
    }

    public void instantiateUnit(GameObject DefenderPrefab)//instantiates a unit
    {
        GameObject unit = null;
        Defender tmpUnit = null;

        unit = Instantiate(DefenderPrefab, placementPoint.position, placementPoint.rotation);

        tmpUnit = unit.GetComponent<Defender>();
        properties.OccupyingUnit = tmpUnit;
        tmpUnit.Coord = properties.Coord;
        properties.Occupied = true;
        tmpUnit.transform.SetParent(placementPoint);
        tmpUnit.Tile = this;
        PlayerManager.defenders.Add(tmpUnit);
    }

    public void SellDefender()//sells a unit
    {
        if (properties.OccupyingUnit == null)
            return;
        PlayerManager.defenders.Remove(properties.OccupyingUnit);
        Destroy(properties.OccupyingUnit.gameObject);
        properties.OccupyingUnit = null;
        properties.Occupied = false;
    }

    public void UpgradeDefender()//upgrades a unit
    {
        if (properties.OccupyingUnit == null)
            return;
        properties.OccupyingUnit.OnUpgrade();
    }

    public void Select()//sellects tile
    {
        /*if (selectionState == SelectionState.NotSelectable | selectionState == SelectionState.Inert)
            return;
        if (selectionState == SelectionState.Selected)
        {
            SetSelectionSate(SelectionState.Selectable);
            return;
        }
        if (selectionState == SelectionState.Selectable)
        {
            SetSelectionSate(SelectionState.Selected);
            return;
        }*/

        properties.highLow = !properties.highLow;//flips the hover height

        if (properties.highLow)
            HoverHigh();
        else
            HoverLow();
    }

    //trig motion equations - they are random equations and are used to create a bobbing effect
    private float TrigMotionEquations(int equation, float progression, float frequency, float amplitude)
    {
        float result = 0f;
        switch (equation)
        {
            case 0:
                result = Mathf.Sin(1.8f * Mathf.Sin(Mathf.Cos(progression * 0.13f)) * Mathf.Cos((progression - 3f) * frequency)) * amplitude;
                break;

            case 1:
                result = Mathf.Sin((Mathf.Sin(progression * 0.8f) * 0.5f) * (Mathf.Cos(progression * 0.2f) * frequency)) * amplitude;
                break;

            case 2:
                result = (Mathf.Sin(progression * frequency)) * (Mathf.Sin((progression * 0.4f) * 0.3f)) * amplitude;
                break;
        }
        return result;
    }

    public BuildingTile Clone()
    {
        BuildingTile clone = Instantiate(this);
        clone.properties = properties.Clone();
        clone.placementPoint = placementPoint;
        return clone;
    }
}