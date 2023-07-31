using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using Rerun;
using UltimateReplay.Storage;
using UltimateReplay;
using UltimateReplay.Statistics;
using UnityEngine.EventSystems;

public struct AnnotationData
{
    [JsonIgnore]  
    public Guid guid;
    public float startTime;
    public float stopTime;
    public string annotationText;
    public List<string> Categories; //TODO
    public AnnotationType type; //type of linerenderer (none, forwardline, twoobjectline)
    public string annotationVisualizationData; //stores name of gameobject(s)
    public RectangleType rectDrawing; // none, rectangle for participant A, rectangle for participant B
    public string rectDimensions; //anchoredposition and width height
}

public struct timelineData
{
    public List<AnnotationData> jsonAnnotations;
    public float scenarioLength;
    public string scenarioName;
    public float currentTime;
    
}

public struct SerializableVector3
{
    public float X;
    public float Y;
    public float Z;
    public SerializableVector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    public SerializableVector3(Vector3 vector3)
    {
        X = vector3.x;
        Y = vector3.y;
        Z = vector3.z;
    }
}

public enum AnnotationType { none, twoobjectline, forwardline, text, box, sphere}
public enum RectangleType { none, ARectangle, BRectangle}
public class timeline : MonoBehaviour
{
    Dictionary<Guid, AnnotationData> m_allAnnotations;

    public GameObject AnnotationPrefab;

    private List<GameObject> AllInstatiatedAnnotations;

    timelineData tData = new timelineData();

    [HideInInspector]
    public float localLength;
    private string localScenarioName;
    [HideInInspector]
    public float localCurrentTime;

    private RectTransform rectT;

    public GameObject rerunManager;
    [HideInInspector]
    public bool DeleteMode;

    private LineRenderer line;
    private GameObject rectangle;
    [HideInInspector] public float sliderValueFromEditor = 50;


    //NEW TEST STUFF
    private Dictionary<AnnotationType, GameObject> annotationPrefabs;
    private Dictionary<Guid, GameObject> m_instantiatedLines = new Dictionary<Guid, GameObject>();
    public Dictionary<Guid, GameObject> m_instantiatedRectangles = new Dictionary<Guid, GameObject>();
    private HashSet<Guid> instantiatedLines = new HashSet<Guid>();
    private HashSet<Guid> instantiatedRectangles = new HashSet<Guid>();
    private GameObject ARectangle;
    private GameObject BRectangle;
    public GameObject rectanglePrefab;

    private RectTransform rectTransform;
    private RectTransform UITransform;
    private float amountMoved;
    private float initialWidth;
    [HideInInspector] public float sideButtonSizeFactor = 200;

    void Start()
    {
        rerunManager = transform.parent.parent.parent.parent.gameObject;
        AllInstatiatedAnnotations = new List<GameObject>();
        m_allAnnotations = new Dictionary<Guid, AnnotationData>();

        tData.jsonAnnotations = new List<AnnotationData>();
        tData = JsonConvert.DeserializeObject<timelineData>(LoadJsonFile("Assets/Resources/test.json"));
        foreach (AnnotationData anno in tData.jsonAnnotations)
        {
            var t = System.Guid.NewGuid();
            m_allAnnotations.Add(t, anno);

        }
        /*localLength = tData.scenarioLength;*/

        localScenarioName = tData.scenarioName;
        localCurrentTime = tData.currentTime;
        updateTimelineSize();

        //NEW TEST STUFF
        annotationPrefabs = new Dictionary<AnnotationType, GameObject>
        {
            {AnnotationType.twoobjectline, Resources.Load<GameObject>("LinePrefab") },
            {AnnotationType.forwardline, Resources.Load<GameObject>("LinePrefab") },
        };

    }
    public LineRenderer InstantiateLineRenderer(Guid guid, AnnotationData annotationData)
    {
        if (annotationData.type == AnnotationType.twoobjectline && annotationData.annotationVisualizationData != null) 
        {
            // Instantiate the line renderer prefab
            GameObject lineObject = Instantiate(annotationPrefabs[AnnotationType.twoobjectline]);
            m_instantiatedLines.Add(guid, lineObject);
            // Get the line renderer component
            LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();
            return lineRenderer;
        }
        else if (annotationData.type == AnnotationType.forwardline && annotationData.annotationVisualizationData != null) 
        {
            // Instantiate the line renderer prefab
            GameObject lineObject = Instantiate(annotationPrefabs[AnnotationType.forwardline]);
            m_instantiatedLines.Add(guid, lineObject);
            // Get the line renderer component
            LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();
            lineObject.transform.SetParent(GameObject.Find(annotationData.annotationVisualizationData).transform);
            lineRenderer.SetPosition(0, GameObject.Find(annotationData.annotationVisualizationData).transform.position);
            return lineRenderer;
        }
        else { return null; }
    }
    public GameObject InstantiateRectangle(Guid guid, AnnotationData anno)
    {
        if (anno.rectDrawing == RectangleType.ARectangle)
        {
            ARectangle = Instantiate(rectanglePrefab, this.transform.parent); // parent is the canvas that holds everything
            if (anno.rectDimensions != null)
            {
                Dictionary<string, float> deserializedData = JsonConvert.DeserializeObject<Dictionary<string, float>>(anno.rectDimensions);
                ARectangle.GetComponent<RectTransform>().anchoredPosition = new Vector2(deserializedData["X"], deserializedData["Y"]);
                ARectangle.GetComponent<RectTransform>().sizeDelta = new Vector2(deserializedData["width"], deserializedData["height"]);
            }
            else
            {
                ARectangle.GetComponent<RectTransform>().anchoredPosition = new Vector2(-Screen.width / 4, Screen.height / 4);
            }
            ARectangle.name = "ARectangle";
            this.gameObject.GetComponentInParent<timeline>().m_instantiatedRectangles.Add(guid, ARectangle);
            return ARectangle;
        }
        else if (anno.rectDrawing == RectangleType.BRectangle)
        {
            BRectangle = Instantiate(rectanglePrefab, this.transform.parent);
            if (anno.rectDimensions != null)
            {
                Dictionary<string, float> deserializedData = JsonConvert.DeserializeObject<Dictionary<string, float>>(anno.rectDimensions);
                BRectangle.GetComponent<RectTransform>().anchoredPosition = new Vector2(deserializedData["X"], deserializedData["Y"]);
                BRectangle.GetComponent<RectTransform>().sizeDelta = new Vector2(deserializedData["width"], deserializedData["height"]);
            }
            else 
            {
                BRectangle.GetComponent<RectTransform>().anchoredPosition = new Vector2(Screen.width / 4, Screen.height / 4);
            }
            BRectangle.name = "BRectangle";
            this.gameObject.GetComponentInParent<timeline>().m_instantiatedRectangles.Add(guid, BRectangle);
            return BRectangle;
        }
        else { return null; }
    }
    // Update is called once per frame
    void Update()
    {
        CheckAllForOverlap();
        localLength = rerunManager.GetComponent<RerunGUI>().totalTimeInFloat;

        //ReInitialize Annotations
        if (Input.GetKeyUp(KeyCode.Space) && !rerunManager.GetComponentInParent<RerunInputManager>().InputOpen)
        {
            ReInitalizeAllAnnotations();    
        }

        //Create New Annotation
        if (Input.GetKeyDown(KeyCode.N) && !rerunManager.GetComponentInParent<RerunInputManager>().InputOpen)
        {
            m_allAnnotations.Add(System.Guid.NewGuid(), new AnnotationData
            {
                startTime = 0,
                stopTime = localLength / 10,
                annotationText = "Double click to edit...",
                Categories = null,
                type = 0,
                annotationVisualizationData = null
            });

            ReInitalizeAllAnnotations();
        }

        //Delete an Annotation
        if (Input.GetKeyDown(KeyCode.Q) && !rerunManager.GetComponentInParent<RerunInputManager>().InputOpen)
        {
            DeleteMode = !DeleteMode;
        }

        //Write to Json
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            tData.jsonAnnotations.Clear();
            foreach (KeyValuePair<Guid, AnnotationData> anno in m_allAnnotations)
            {
                tData.jsonAnnotations.Add(anno.Value);
            }
            tData.scenarioLength = localLength;
            string jsonText = JsonConvert.SerializeObject(tData);
            WriteJsonFile(jsonText, "Assets/Resources/test.json");
        }


        foreach (KeyValuePair<Guid, AnnotationData> anno in m_allAnnotations)
        {
            localCurrentTime = rerunManager.GetComponent<RerunGUI>().currentTimeInFloat;
            if (((localCurrentTime < anno.Value.stopTime) && (localCurrentTime > anno.Value.startTime)))
            {
                if (!m_instantiatedLines.ContainsKey(anno.Key))
                {
                    if (!instantiatedLines.Contains(anno.Key))
                    {
                        line = InstantiateLineRenderer(anno.Key, anno.Value);
                        instantiatedLines.Add(anno.Key);
                    }
                }
                if (!m_instantiatedRectangles.ContainsKey(anno.Key))
                {
                    if (!instantiatedRectangles.Contains(anno.Key))
                    {
                        rectangle = InstantiateRectangle(anno.Key, anno.Value);
                        instantiatedRectangles.Add(anno.Key);
                    }
                }


                //This is here so that the lineRenderer positions are in void Update()
                if (anno.Value.type == AnnotationType.twoobjectline)
                {
                    Dictionary<string, string> deserializedData = JsonConvert.DeserializeObject<Dictionary<string, string>>(anno.Value.annotationVisualizationData);
                    line.SetPosition(0, GameObject.Find(deserializedData["gameObjectName1"]).transform.position);
                    line.SetPosition(1, GameObject.Find(deserializedData["gameObjectName2"]).transform.position);
                }
                if(anno.Value.type == AnnotationType.forwardline)
                {
                    if (line != null)
                    {
                        line.SetPosition(1, GameObject.Find(anno.Value.annotationVisualizationData).transform.forward * sliderValueFromEditor);
                    }
                }
            }

            if (localCurrentTime < anno.Value.startTime || localCurrentTime > anno.Value.stopTime)
            {
                if (m_instantiatedLines.ContainsKey(anno.Key))
                {
                    GameObject GO = m_instantiatedLines[anno.Key];
                    Destroy(GO);
                    m_instantiatedLines.Remove(anno.Key);
                }
                if (m_instantiatedRectangles.ContainsKey(anno.Key))
                {
                    GameObject GO = m_instantiatedRectangles[anno.Key];
                    Destroy(GO);
                    m_instantiatedRectangles.Remove(anno.Key);
                }
                instantiatedLines.Remove(anno.Key);
                instantiatedRectangles.Remove(anno.Key);

            }
        }
    }


    public void ReInitalizeAllAnnotations()
    {
        foreach (var t in AllInstatiatedAnnotations)
        {
            Destroy(t.gameObject);
        }
        foreach (KeyValuePair<Guid, AnnotationData> anno in m_allAnnotations)
        {

            var t = Instantiate(AnnotationPrefab, transform);
            AllInstatiatedAnnotations.Add(t);
            t.GetComponent<bigButton>().InitilizeData(anno.Value, anno.Key);
            if (anno.Value.type == AnnotationType.none) { t.GetComponentInChildren<editorScript>().lineRendererdropdown.value = 0; }
            if (anno.Value.type == AnnotationType.forwardline) { t.GetComponentInChildren<editorScript>().lineRendererdropdown.value = 1; }
            if (anno.Value.type == AnnotationType.twoobjectline) { t.GetComponentInChildren<editorScript>().lineRendererdropdown.value = 2; }
            t.GetComponentInChildren<editorScript>().inputText.text = anno.Value.annotationText;

        }

    }
    //Delete An Annotation
    public void DeleteAnnotation(Guid guid)
    {
        m_allAnnotations.Remove(guid);
        DeleteMode = false;
    }
    public void UpdateAnnotation(AnnotationData anno, Guid guid)
    {
        if (m_allAnnotations.ContainsKey(guid))
        {
            m_allAnnotations[guid] = anno;
        }
    }
    private string LoadJsonFile(string m_path)
    {

        StreamReader sr = new StreamReader(m_path);
        var read = sr.ReadToEnd();

        return read;
    }

    private void WriteJsonFile(string jsonfile, string m_path)
    {
        Debug.Log("Writing to: " + m_path);

        StreamWriter writer = new StreamWriter(m_path, false);
        writer.Write(jsonfile);

        writer.Close();
    }

    public void updateTimelineSize()
    {
        rectT = this.gameObject.GetComponent<RectTransform>();
        //left and PosY
        rectT.offsetMin = new Vector2(Screen.width / 64, Screen.height / 36);
        //right and dunno
        rectT.offsetMax = new Vector2(Screen.width / -64, 0);
        //height of timeline RectTransform
        rectT.sizeDelta = new Vector2(rectT.sizeDelta.x, Screen.height / 9);
    }
    void CheckAllForOverlap()
    {
        for(int i = 0; i<transform.childCount; i++)
        {
            for(int j = i+1; j < transform.childCount; j++)
            {
                if (transform.GetChild(i).gameObject.CompareTag("Annotation") && transform.GetChild(j).gameObject.CompareTag("Annotation"))
                {
                    if (CheckOverlap(transform.GetChild(i).gameObject, transform.GetChild(j).gameObject))
                    {
                        transform.GetChild(i).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(transform.GetChild(i).gameObject.GetComponent<RectTransform>().sizeDelta.x, 28);
                        transform.GetChild(i).gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(transform.GetChild(i).gameObject.GetComponent<RectTransform>().anchoredPosition.x, 20);
                        transform.GetChild(j).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(transform.GetChild(j).gameObject.GetComponent<RectTransform>().sizeDelta.x, 28);
                        transform.GetChild(j).gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(transform.GetChild(j).gameObject.GetComponent<RectTransform>().anchoredPosition.x, -20);
                    }
                    //BROKEN, currently just press space to fix
                        /*                    if(!CheckOverlap(transform.GetChild(i).gameObject, transform.GetChild(j).gameObject))
                                            {
                                                transform.GetChild(i).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(transform.GetChild(i).gameObject.GetComponent<RectTransform>().sizeDelta.x, 28);
                                                transform.GetChild(i).gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(transform.GetChild(i).gameObject.GetComponent<RectTransform>().anchoredPosition.x, 0);
                                                transform.GetChild(j).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(transform.GetChild(j).gameObject.GetComponent<RectTransform>().sizeDelta.x, 28);
                                                transform.GetChild(j).gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(transform.GetChild(j).gameObject.GetComponent<RectTransform>().anchoredPosition.x, 0);
                                            }*/
                }
            }
        }
    }

    bool CheckOverlap(GameObject obj1, GameObject obj2)
    {
        float obj1MinX = obj1.GetComponent<RectTransform>().anchoredPosition.x;
        float obj1MaxX = obj1.GetComponent<RectTransform>().anchoredPosition.x + obj1.GetComponent<RectTransform>().rect.width;

        float obj2MinX = obj2.GetComponent<RectTransform>().anchoredPosition.x;
        float obj2MaxX = obj2.GetComponent<RectTransform>().anchoredPosition.x + obj2.GetComponent<RectTransform>().rect.width;

        return obj1MaxX >= obj2MinX && obj2MaxX >= obj1MinX;

    }
}
