using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Rerun;
using Newtonsoft.Json;

public class editorScript : MonoBehaviour
{
    //For dropdown selection of AnnotationType
    public TMP_InputField inputText;
    public TMP_Dropdown dropdown;
    public GameObject ForwardLineUIElement;
    public GameObject TwoObjectLineUIElement;

    //For selecting GameObject in select mode
    public Button selectButton;
    public Button selectButton1;
    public Button selectButton2;
    public Camera renderCamera;
    public RawImage rawImageDisplay;
    [HideInInspector] public GameObject selectedObj1 = null;
    [HideInInspector] public GameObject selectedObj2 = null;
    private bool isSelecting;

    public struct GameObjectNames
    {
        public string gameObjectName1;
        public string gameObjectName2;
    }
    GameObjectNames gameObjectNames = new GameObjectNames();

    void Start()
    {
        dropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(dropdown); });
        selectButton.onClick.AddListener(() => { isSelecting = true; });
        selectButton1.onClick.AddListener(() => { isSelecting = true; });
        selectButton2.onClick.AddListener(() => { isSelecting = true; });
        renderCamera = GameObject.Find("RerunManager").GetComponent<Camera>();
        rawImageDisplay = GameObject.Find("Display_Camera_4").GetComponent<RawImage>();

    }

    void DropdownValueChanged(TMP_Dropdown change)
    {
        switch (change.value)
        {
            case 0:
                ForwardLineUIElement.SetActive(false);
                TwoObjectLineUIElement.SetActive(false);
                this.gameObject.GetComponentInParent<bigButton>().setAnnoAsDefault();
                break;
            case 1:
                ForwardLineUIElement.SetActive(true);
                TwoObjectLineUIElement.SetActive(false);
                this.gameObject.GetComponentInParent<bigButton>().setAnnoAsForwardline();
                break;
            case 2:
                ForwardLineUIElement.SetActive(false);
                TwoObjectLineUIElement.SetActive(true);
                this.gameObject.GetComponentInParent<bigButton>().setAnnoAsTwoObjLine();
                break;
        }
    }

    void selectGO()
    {
        if (Input.GetMouseButtonDown(0) && isSelecting)
        {
            Vector2 localCursor;
            var rt = rawImageDisplay.GetComponent<RectTransform>();
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, Input.mousePosition, null, out localCursor))
            {
                float x = ((localCursor.x - rt.rect.x) * rt.rect.width) / rt.rect.width * 2 - 1;
                float y = ((localCursor.y - rt.rect.y) * rt.rect.height) / rt.rect.height * 2 - 1;

                Ray ray = renderCamera.ScreenPointToRay(new Vector3(x, y, 0));
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    if (ForwardLineUIElement.activeSelf)
                    {
                        selectedObj1 = hit.transform.gameObject;
                        GameObject.Find("ForwardTMP").GetComponent<TMP_Text>().text = selectedObj1.name;
                        this.gameObject.GetComponentInParent<bigButton>().setAnnoVisualizationData(selectedObj1.name);
                    }
                    else if (TwoObjectLineUIElement.activeSelf)
                    {
                        if(selectedObj1 == null)
                        {
                            selectedObj1 = hit.transform.gameObject;
                            GameObject.Find("TwoObjTMP1").GetComponent<TMP_Text>().text = selectedObj1.name;
                            gameObjectNames.gameObjectName1 = selectedObj1.name;
                        }
                        else
                        {
                            selectedObj2 = hit.transform.gameObject;
                            GameObject.Find("TwoObjTMP2").GetComponent<TMP_Text>().text = selectedObj2.name;
                            gameObjectNames.gameObjectName2 = selectedObj2.name;
                            this.gameObject.GetComponentInParent<bigButton>().setAnnoVisualizationData(Newtonsoft.Json.JsonConvert.SerializeObject(gameObjectNames));
                        }
                    }
                }
                else
                {
                    Debug.Log("Nothing selected");
                    GameObject.Find("ForwardTMP").GetComponent<TMP_Text>().text = "Invalid selection.";
                }
            }
            isSelecting = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.parent.GetComponentInChildren<TMP_Text>().text = inputText.text;
        this.gameObject.GetComponentInParent<bigButton>().updateTimesBasedOnPosition();

        selectGO();

    }
}
