using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class editorScript : MonoBehaviour
{
    public TMP_InputField inputText;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.parent.GetComponentInChildren<TMP_Text>().text = inputText.text;
        this.gameObject.GetComponentInParent<bigButton>().updateTimesBasedOnPosition();


    }
}
