using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class resizeSide : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform LargeButtonTransform;
    private Vector3 offset;
    private RectTransform rectTransform;
    private float initialPos;
    private float initialScrubberPos;

    private float initialWidth;
    private float amountMoved;
    [HideInInspector]
    public float scrubberTime;
    [HideInInspector]
    public float scrubberTimeNormalized;
    public enum resizeSliderPosition {  LEFT,RIGHT,TOP,BOTTOM,TIMELINE, BIGBUTTON};
    public resizeSliderPosition myPosition;

    private RectTransform rectT;
    private float sideButtonSizeFactor = 100;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        switch (myPosition)
        {
            case resizeSliderPosition.LEFT:
                LargeButtonTransform = transform.parent.GetComponent<RectTransform>();
                rectTransform.offsetMax = new Vector2(Screen.width / sideButtonSizeFactor, 0);
                this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / sideButtonSizeFactor, this.gameObject.GetComponent<RectTransform>().sizeDelta.y);
                break;
            case resizeSliderPosition.RIGHT:
                LargeButtonTransform = transform.parent.GetComponent<RectTransform>();
                this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / sideButtonSizeFactor, this.gameObject.GetComponent<RectTransform>().sizeDelta.y);
                break;
            case resizeSliderPosition.BIGBUTTON:
                LargeButtonTransform = transform.gameObject.GetComponent<RectTransform>();
                break;
            case resizeSliderPosition.TIMELINE:
                updateSize();
                break;
        }

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        offset = rectTransform.position - Input.mousePosition;
        amountMoved = eventData.delta.x;

        switch (myPosition)
        {
            case resizeSliderPosition.LEFT:
            case resizeSliderPosition.RIGHT:
            case resizeSliderPosition.BIGBUTTON:
                initialWidth = LargeButtonTransform.rect.width;
                initialPos = LargeButtonTransform.anchoredPosition.x;
                break;
            case resizeSliderPosition.TIMELINE:
                initialScrubberPos = this.gameObject.GetComponent<RectTransform>().anchoredPosition.x;
                break;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        amountMoved += eventData.delta.x;
        // Only update the x position.
        switch (myPosition)
        {

            case resizeSliderPosition.RIGHT:
                rectTransform.position = new Vector3(Input.mousePosition.x + offset.x, rectTransform.position.y, rectTransform.position.z);
                LargeButtonTransform.sizeDelta = new Vector2(amountMoved + initialWidth, LargeButtonTransform.sizeDelta.y);
                //snap small button to big button
                //left and bottom
                rectTransform.offsetMin = new Vector2(Screen.width/sideButtonSizeFactor, 0);
                //right and top
                rectTransform.offsetMax = new Vector2(0, 0);
                rectTransform.sizeDelta = new Vector2(Screen.width / sideButtonSizeFactor, 0);
                break;
            case resizeSliderPosition.LEFT:
                rectTransform.position = new Vector3(Input.mousePosition.x + offset.x, rectTransform.position.y, rectTransform.position.z);
                LargeButtonTransform.sizeDelta = new Vector2(initialWidth - amountMoved, LargeButtonTransform.sizeDelta.y);
                LargeButtonTransform.anchoredPosition = new Vector2(initialPos + amountMoved, LargeButtonTransform.anchoredPosition.y);
                //snap small button to big button
                //left and bottom
                rectTransform.offsetMin = new Vector2(0, 0);
                //right and top
                rectTransform.offsetMax = new Vector2(Screen.width/sideButtonSizeFactor, 0);
                rectTransform.sizeDelta = new Vector2(Screen.width / sideButtonSizeFactor, 0);
                break;
            case resizeSliderPosition.BIGBUTTON:
                LargeButtonTransform.anchoredPosition = new Vector2(initialPos + amountMoved, LargeButtonTransform.anchoredPosition.y);
                break;
            case resizeSliderPosition.TIMELINE:
                this.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(initialScrubberPos + amountMoved, this.gameObject.GetComponent<RectTransform>().anchoredPosition.y);
                break;
        };




    }

    public void OnEndDrag(PointerEventData eventData)
    {
        switch (myPosition)
        {

            case resizeSliderPosition.RIGHT:
                //snap small button to big button
                //left and bottom
                rectTransform.offsetMin = new Vector2(Screen.width/sideButtonSizeFactor, 0);
                //right and top
                rectTransform.offsetMax = new Vector2(0, 0);
                rectTransform.sizeDelta = new Vector2(Screen.width / sideButtonSizeFactor, 0);
                offset = Vector3.zero;
                LargeButtonTransform.GetComponent<bigButton>().updateTimesBasedOnPosition();
                break;

            case resizeSliderPosition.LEFT:
                //snap small button to big button
                //left and bottom
                rectTransform.offsetMin = new Vector2(0, 0);
                //right and top
                rectTransform.offsetMax = new Vector2(Screen.width/sideButtonSizeFactor, 0);
                rectTransform.sizeDelta = new Vector2(Screen.width / sideButtonSizeFactor, 0);
                offset = Vector3.zero;
                LargeButtonTransform.GetComponent<bigButton>().updateTimesBasedOnPosition();
                break;
            case resizeSliderPosition.BIGBUTTON:
                LargeButtonTransform.GetComponent<bigButton>().updateTimesBasedOnPosition();
                break;


        };
    }

    void Update()
    {
        switch (myPosition)
        {
            case resizeSliderPosition.TIMELINE:
                //calculate time in scenario based on where scrubber relative to the timeline
                scrubberTime = (this.gameObject.GetComponent<RectTransform>().localPosition.x / this.transform.parent.GetComponent<RectTransform>().rect.width) * GetComponentInParent<timeline>().localLength;
                scrubberTimeNormalized = (this.gameObject.GetComponent<RectTransform>().localPosition.x / this.transform.parent.GetComponent<RectTransform>().rect.width);

                //make impossible for scrubber to go outside of timeline
                if (this.gameObject.GetComponent<RectTransform>().localPosition.x < 0)
                {
                    this.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, this.gameObject.GetComponent<RectTransform>().localPosition.y, this.gameObject.GetComponent<RectTransform>().localPosition.z);
                }
                else if(this.gameObject.GetComponent<RectTransform>().localPosition.x > this.transform.parent.GetComponent<RectTransform>().rect.width)
                {
                    this.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(this.transform.parent.GetComponent<RectTransform>().rect.width, this.gameObject.GetComponent<RectTransform>().localPosition.y, this.gameObject.GetComponent<RectTransform>().localPosition.z);
                }
                break;
        }



    }

    public void updateSize()
    {
        switch (myPosition)
        {
            case resizeSliderPosition.TIMELINE:
                //height of scrubber same on every res
                rectT = this.gameObject.GetComponent<RectTransform>();
                rectT.sizeDelta = new Vector2(Screen.height / 8, Screen.height / 6);
                break;
            case resizeSliderPosition.LEFT:
            case resizeSliderPosition.RIGHT:
                this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width / sideButtonSizeFactor, this.gameObject.GetComponent<RectTransform>().sizeDelta.y);
                break;
        }

    }

}



