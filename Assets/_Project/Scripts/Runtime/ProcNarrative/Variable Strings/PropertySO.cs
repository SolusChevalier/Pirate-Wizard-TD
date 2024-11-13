using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PropertySO", menuName = "ScriptableObjects/PropertySO", order = 90)]
public class PropertySO : ScriptableObject
{
    #region FIELDS

    [SerializeField] private PropertyType Type;
    [SerializeField] private string CurrentValue;
    [SerializeField] private string DefaultValue;
    [SerializeField] private List<string> PossibleValues;

    #endregion FIELDS

    #region METHODS

    public PropertySO(PropertyType _type, string _defaultValue, List<string> _possibleValues)
    {
        Type = _type;
        CurrentValue = _defaultValue;
        DefaultValue = _defaultValue;
        PossibleValues = _possibleValues;
    }

    public PropertyType GetPropertyType()
    {
        return Type;
    }

    public string GetCurrentValue()
    {
        return CurrentValue;
    }

    public void SetCurrentValue(string _value)
    {
        CurrentValue = _value;
    }

    public string GetDefaultValue()
    {
        return DefaultValue;
    }

    public void SetCurrentRandom()
    {
        CurrentValue = PossibleValues[Random.Range(0, PossibleValues.Count)];
    }

    public string GetRandomValue()
    {
        return PossibleValues[Random.Range(0, PossibleValues.Count)];
    }

    public List<string> GetPossibleValues()
    {
        return PossibleValues;
    }

    #endregion METHODS
}