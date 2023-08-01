using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rectangleMasterScript : MonoBehaviour
{
    [HideInInspector] public Guid associatedGUID;
    [HideInInspector] public string jsonString;
    private GameObject a;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void updateRectSize(string jsonstring)
    {
        bigButton[] buttons = FindObjectsOfType<bigButton>();
        foreach (bigButton button in buttons)
        {
            if (button.GetComponent<bigButton>().m_guid == associatedGUID)
            {
                a = button.gameObject;
                a.GetComponent<bigButton>().setAnnoRectDimensions(jsonstring);

            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
