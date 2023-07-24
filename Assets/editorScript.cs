using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class editorScript : MonoBehaviour
{
    public TMP_InputField inputText;
    public TMP_Dropdown dropdown;
    public GameObject ForwardLineUIElement;
    public GameObject TwoObjectLineUIElement;
    void Start()
    {
        dropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(dropdown); });
        
    }

    void DropdownValueChanged(TMP_Dropdown change)
    {
        switch (change.value)
        {
            case 0:
                ForwardLineUIElement.SetActive(false);
                TwoObjectLineUIElement.SetActive(false);
                break;
            case 1:
                ForwardLineUIElement.SetActive(true);
                TwoObjectLineUIElement.SetActive(false);
                this.gameObject.GetComponentInParent<bigButton>().setAnnoAsForwardline();
                break;
            case 2:
                ForwardLineUIElement.SetActive(false);
                TwoObjectLineUIElement.SetActive(true);
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.parent.GetComponentInChildren<TMP_Text>().text = inputText.text;
        this.gameObject.GetComponentInParent<bigButton>().updateTimesBasedOnPosition();


    }
}
