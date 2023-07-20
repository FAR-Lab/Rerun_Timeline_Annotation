using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class updateSizeBasedOnResolution : MonoBehaviour
{

    int lastScreenWidth;
    int lastScreenHeight;
    // Start is called before the first frame update
    void Awake()
    {
        lastScreenHeight = Screen.width;
        lastScreenHeight = Screen.height;
    }


    void Update()
    {
        if (lastScreenWidth != Screen.width || lastScreenHeight != Screen.height)
        {
            lastScreenWidth = Screen.width;
            lastScreenHeight = Screen.height;
            OnScreenSizeChanged(); 
        }
    }

    void OnScreenSizeChanged()
    {
        this.transform.GetComponent<timeline>().updateTimelineSize();
        this.transform.GetComponent<timeline>().ReInitalizeAllAnnotations();
        this.transform.GetComponentInChildren<resizeSide>().updateSize();
        this.transform.GetComponent<autoGenerateTimeMarkers>().ReInstantiateMarkers(this.gameObject.GetComponent<timeline>().localLength);

        Debug.Log("SIZE CHANGE DETECED");
    }
}
