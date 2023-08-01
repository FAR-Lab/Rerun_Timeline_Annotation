using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using Rerun;

public struct rectData
{
    public float X { get; set; }
    public float Y { get; set; }
    public float width { get; set; }
    public float height { get; set; }
}
public class resizeRectangle : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    rectData rectData = new rectData();
    public enum resizeButtons { topresize, rightresize, bottomresize, leftresize };
    public resizeButtons resizeType;
    private RectTransform buttonRectTransform;
    private RectTransform UITransform;
    private float amountMovedX;
    private float amountMovedY;
    private float initialWidth;
    private float initialHeight;
    private float initialPosX;
    private float initialPosY;

    public void OnBeginDrag(PointerEventData eventData)
    {
        amountMovedX = eventData.delta.x;
        amountMovedY = eventData.delta.y;
        initialWidth = UITransform.rect.width;
        initialHeight = UITransform.rect.height;
        initialPosX = UITransform.anchoredPosition.x;
        initialPosY = UITransform.anchoredPosition.y;
    }
    public void OnDrag(PointerEventData eventData)
    {
        amountMovedX += eventData.delta.x;
        amountMovedY += eventData.delta.y;
        switch (resizeType)
        {
            case resizeButtons.leftresize:
                UITransform.sizeDelta = new Vector2(initialWidth - amountMovedX, UITransform.sizeDelta.y);
                UITransform.anchoredPosition = new Vector2(initialPosX + (amountMovedX/2), UITransform.anchoredPosition.y);
                break;
            case resizeButtons.rightresize:
                UITransform.sizeDelta = new Vector2(amountMovedX + initialWidth, UITransform.sizeDelta.y);
                UITransform.anchoredPosition = new Vector2(initialPosX + (amountMovedX / 2), UITransform.anchoredPosition.y);
                break;
            case resizeButtons.topresize:
                UITransform.sizeDelta = new Vector2(UITransform.sizeDelta.x, initialHeight + amountMovedY);
                UITransform.anchoredPosition = new Vector2(UITransform.anchoredPosition.x, initialPosY + (amountMovedY/2));
                break;
            case resizeButtons.bottomresize:
                UITransform.sizeDelta = new Vector2(UITransform.sizeDelta.x, initialHeight - amountMovedY);
                UITransform.anchoredPosition = new Vector2(UITransform.anchoredPosition.x, initialPosY + (amountMovedY / 2));
                break;
        }
        //TODO MAKE THIS DATA ALWAYS THE 0 VIEW DATA (use if statements and get currentview from RerunLayoutManager)
        if(this.gameObject.GetComponentInParent<RerunLayoutManager>().currentView == 0)
        {
            rectData.X = UITransform.anchoredPosition.x;
            rectData.width = UITransform.sizeDelta.x;
            rectData.Y = UITransform.anchoredPosition.y;
            rectData.height = UITransform.sizeDelta.y;
        }
        else
        {
            if(this.transform.parent.name == "ARectangle")
            {
                // Reverse size operation
                rectData.X = ((UITransform.anchoredPosition.x / 2) - Screen.width / 4);
                rectData.width = UITransform.rect.width / 2;
                rectData.Y = ((UITransform.anchoredPosition.y / 2) + (Screen.height / 4));
                rectData.height = UITransform.rect.height / 2;
            }
            else if (this.transform.parent.name == "BRectangle")
            {
                // Reverse size operation
                rectData.X = ((UITransform.anchoredPosition.x / 2) + Screen.width / 4);
                rectData.width = UITransform.rect.width / 2;
                rectData.Y = ((UITransform.anchoredPosition.y / 2) + (Screen.height / 4));
                rectData.height = UITransform.rect.height / 2;
            }
        }
        string jsonString = JsonConvert.SerializeObject(rectData);
        Debug.Log(jsonString);
        this.transform.parent.GetComponent<rectangleMasterScript>().updateRectSize(jsonString);
    }
    // Start is called before the first frame update
    void Start()
    {
        buttonRectTransform = this.gameObject.GetComponent<RectTransform>();
        UITransform = this.transform.parent.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
