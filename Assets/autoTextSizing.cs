using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class autoTextSizing : MonoBehaviour
{
    private void Start()
    {
      
    }
    public void timeMarkerFontSize()
    {
        this.gameObject.GetComponent<TMP_Text>().fontSize = Screen.height / 20;

    }
}
