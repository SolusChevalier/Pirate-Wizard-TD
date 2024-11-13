using System.Collections;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using UnityEngine;

[CreateAssetMenu(fileName = "TemplateSO", menuName = "ScriptableObjects/TemplateSO", order = 90)]
public class TemplateSO : ScriptableObject
{
    #region FIELDS

    [SerializeField] private List<string> sentence;
    private List<PropertyType> possibleValues;

    #endregion FIELDS

    #region METHODS

    public string GetSentence()
    {
        string result = "";
        foreach (string line in sentence)
        {
            result += line + "\n";
        }
        foreach (PropertyType property in possibleValues)
        {
            result = result.Replace("{" + property.ToString() + "}", NarrativeManager.Instance.propertyDict[property].GetCurrentValue());
        }
        return result;
    }

    public string GetRandomSentence()
    {
        string result = "";
        foreach (string line in sentence)
        {
            result += line + "\n";
        }
        foreach (PropertyType property in possibleValues)
        {
            result = result.Replace("{" + property.ToString() + "}", NarrativeManager.Instance.propertyDict[property].GetRandomValue());
        }
        return result;
    }

    public void SetValues(List<PropertyType> _possibleValues)
    {
        possibleValues = _possibleValues;
    }

    #endregion METHODS
}