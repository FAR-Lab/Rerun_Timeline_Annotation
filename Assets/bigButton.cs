using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.UI;
using TMPro;
using System;
using Rerun;

public class bigButton : MonoBehaviour , IPointerClickHandler
{
    private bool resizeMode = false;
    private bool editTextMode = false;
    private RectTransform myRectTransform;

    public GameObject editor;
    public GameObject RerunManager;

    AnnotationData data;
    Guid m_guid; 
    void Start()
    {
        myRectTransform = this.gameObject.GetComponent<RectTransform>();

    }
    void Update()
    {
        //SINGLE CLICK RESIZING ON/OFF
        if (resizeMode)
        {
            foreach(Transform child in transform)
            {
                if (child.CompareTag("resizeButton")) {
                    child.gameObject.SetActive(false);
                }

            }
        }
        else
        {
            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }

        //DOUBLE CLICK TEXT EDIT
         if (editTextMode)
            {
                editor.SetActive(true);
            }
        else
            {
                editor.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            editTextMode = false;
            editor.SetActive(false);
            transform.parent.parent.parent.parent.gameObject.GetComponentInParent<RerunInputManager>().InputOpen = false;

        }



    }

    public void OnPointerClick(PointerEventData eventData)
    {
        int clickCount = eventData.clickCount;

        if (clickCount == 1)
            OnSingleClick();
        else if (clickCount == 2)
            OnDoubleClick();

    }

    void OnSingleClick()
    {
        //DELETE ANNOTATION
        if (transform.GetComponentInParent<timeline>().DeleteMode)
        {
            transform.GetComponentInParent<timeline>().DeleteAnnotation(m_guid);
            transform.GetComponentInParent<timeline>().ReInitalizeAllAnnotations();
        }
        resizeMode = !resizeMode;
    }

    void OnDoubleClick()
    {
        editTextMode = !editTextMode;
        resizeMode = !resizeMode;
        transform.parent.parent.parent.parent.gameObject.GetComponentInParent<RerunInputManager>().InputOpen = !transform.parent.parent.parent.parent.gameObject.GetComponentInParent<RerunInputManager>().InputOpen;

    }

    public void InitilizeData(AnnotationData anno,Guid associatedGuid)
    {
        m_guid = associatedGuid;
        myRectTransform = this.gameObject.GetComponent<RectTransform>();
        //TODO fix heights
        myRectTransform.anchoredPosition = new Vector2(((anno.startTime / GetComponentInParent<timeline>().localLength) * this.transform.parent.GetComponent<RectTransform>().rect.width), 0);
        myRectTransform.sizeDelta = new Vector2(((anno.stopTime - anno.startTime)/ GetComponentInParent<timeline>().localLength) * this.transform.parent.GetComponent<RectTransform>().rect.width, this.transform.parent.GetComponent<RectTransform>().rect.height / 2);
        this.gameObject.GetComponentInChildren<TMP_Text>().text = anno.annotationText;
        data = anno;
    }
    public void updateTimesBasedOnPosition()
    {
        data = new AnnotationData
        {
            startTime = myRectTransform.anchoredPosition.x / this.transform.parent.GetComponent<RectTransform>().rect.width * GetComponentInParent<timeline>().localLength,
            stopTime = ((myRectTransform.sizeDelta.x / this.transform.parent.GetComponent<RectTransform>().rect.width) * GetComponentInParent<timeline>().localLength) + (myRectTransform.anchoredPosition.x / this.transform.parent.GetComponent<RectTransform>().rect.width * GetComponentInParent<timeline>().localLength),
            annotationText = this.gameObject.transform.GetComponentInChildren<TMP_Text>().text,
            Categories = new List<string>(),
            type = data.type
        };
        transform.parent.GetComponent<timeline>().UpdateAnnotation(data, m_guid);
    }
}  
