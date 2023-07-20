using Rerun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotkeysScreen : MonoBehaviour
{
    public GameObject rerunmanager;
    public GameObject hotkeyscreen;
    private bool alreadyOpen = true;
    // Update is called once per frame
    void Update()
    {
        alreadyOpen = hotkeyscreen.activeInHierarchy;
        if(Input.GetKeyDown(KeyCode.H) && !rerunmanager.GetComponent<RerunInputManager>().InputOpen && !alreadyOpen)
        {
            hotkeyscreen.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.H) && !rerunmanager.GetComponent<RerunInputManager>().InputOpen && alreadyOpen)
        {
            hotkeyscreen.SetActive(false);
        }
    }
}
