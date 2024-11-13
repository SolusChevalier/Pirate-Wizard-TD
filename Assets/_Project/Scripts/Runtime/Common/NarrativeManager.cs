using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NarrativeManager : MonoBehaviour
{
    #region FIELDS

    public static NarrativeManager Instance;
    public Dictionary<PropertyType, PropertySO> propertyDict = new Dictionary<PropertyType, PropertySO>();
    public PropertySO[] properties;
    public TemplateSO[] templates;
    public PropertySO Wizard_Adj, Wizard_Noun, Wizard_Name, Wizard_Title;
    public PropertySO Goblin_Adj, Goblin_Noun, Goblin_Name, Golbin_Title;
    public GameObject Wizard_Dialog, Goblin_Dialog, DialogButton;
    public TextMeshProUGUI Wizard_NameTxt, Wizard_DialogTxt, Goblin_NameTxt, Goblin_DialogTxt;

    #endregion FIELDS

    #region UNITY METHODS

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        Wizard_Dialog.SetActive(false);
        Goblin_Dialog.SetActive(false);
        DialogButton.SetActive(false);
        foreach (PropertySO property in properties)
        {
            propertyDict.Add(property.GetPropertyType(), property);
        }
        foreach (TemplateSO tmplate in templates)
        {
            tmplate.SetValues(new List<PropertyType>(propertyDict.Keys));
        }
        /*foreach (TemplateSO template in templates)
        {
            //Debug.Log(template.GetSentence());
        }*/
        Wizard_Name.SetCurrentRandom();
        Wizard_Title.SetCurrentRandom();
        Goblin_Name.SetCurrentRandom();
        Goblin_Noun.SetCurrentRandom();
    }

    public void OnEnable()
    {
        EventManager.OnWaveStart += SartDialog;
        EventManager.OnWaveCompleted += SartDialog;
    }

    public void OnDisable()
    {
        EventManager.OnWaveStart -= SartDialog;
        EventManager.OnWaveCompleted -= SartDialog;
    }

    public void SartDialog()
    {
        StartCoroutine(GetDialog());
    }

    public TemplateSO getRandomTemplate()
    {
        return templates[Random.Range(0, templates.Length)];
    }

    public void SetDialog(string _name, string _dialog, bool _isLeft)
    {
        if (_isLeft)
        {
            Wizard_NameTxt.text = _name;
            Wizard_DialogTxt.text = _dialog;
        }
        else
        {
            Goblin_NameTxt.text = _name;
            Goblin_DialogTxt.text = _dialog;
        }
    }

    public IEnumerator GetDialog()
    {
        Wizard_Dialog.SetActive(false);
        Goblin_Dialog.SetActive(true);
        Goblin_NameTxt.text = Golbin_Title.GetCurrentValue() + " " + Goblin_Name.GetCurrentValue();
        Goblin_DialogTxt.text = getRandomTemplate().GetRandomSentence();
        yield return new WaitForSeconds(3);
        StartCoroutine(GetWizardDialog());
    }

    public IEnumerator GetWizardDialog()
    {
        Goblin_Dialog.SetActive(false);
        Wizard_Dialog.SetActive(true);
        Wizard_NameTxt.text = Wizard_Title.GetCurrentValue() + " " + Wizard_Name.GetCurrentValue();
        Wizard_DialogTxt.text = getRandomTemplate().GetRandomSentence();
        yield return new WaitForSeconds(3);
        //DialogButton.SetActive(true);
        Wizard_Dialog.SetActive(false);
    }

    #endregion UNITY METHODS
}