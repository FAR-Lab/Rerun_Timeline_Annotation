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

public struct AnnotationData
{
    [JsonIgnore]  
    public Guid guid;
    public float startTime;
    public float stopTime;
    public string annotationText;
    public List<string> Categories;
    public AnnotationType type;
    public string annotationVisualizationData; //stores vector3s
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

public enum AnnotationType { twopointline, forwardline, text, box, sphere, ray}
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


    //NEW TEST STUFF
    private Dictionary<AnnotationType, GameObject> annotationPrefabs;
    public GameObject TESTSUBJECT;
    private Dictionary<Guid, GameObject> m_instantiatedObjects = new Dictionary<Guid, GameObject>();
    private HashSet<Guid> instantiatedAnnotations = new HashSet<Guid>();
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
            {AnnotationType.twopointline, Resources.Load<GameObject>("LinePrefab") },
            {AnnotationType.forwardline, Resources.Load<GameObject>("LinePrefab") }
        };

    }

    //NEW TEST STUFF
/*    public void InitializeTestLineRenderer(AnnotationData annotationData)
    {
        Vector3 startPoint = TESTSUBJECT.transform.position;
        Vector3 endPoint = startPoint + TESTSUBJECT.transform.forward * 500;

        SerializableVector3 serializableStartPoint = new SerializableVector3(startPoint);
        SerializableVector3 serializableEndPoint = new SerializableVector3(endPoint);

        string visualizationData = JsonConvert.SerializeObject(new[] { serializableStartPoint, serializableEndPoint });

*//*        AnnotationData lineAnnotation = new AnnotationData
        {
            startTime = annotationData.startTime,
            stopTime = annotationData.stopTime,
            annotationText = annotationData.annotationText,
            Categories = annotationData.Categories,
            type = AnnotationType.twopointline,
            annotationVisualizationData = visualizationData
        };

        m_allAnnotations.Add(System.Guid.NewGuid(), lineAnnotation);*/
/*        InstantiateLineRenderer(annotationData, "0");
*//*    }

    public void InitializeTestForwardLineRenderer(AnnotationData annotationData)
    {
*//*        AnnotationData forwardLine = new AnnotationData
        {
            startTime = annotationData.startTime,
            stopTime = annotationData.stopTime,
            annotationText = annotationData.annotationText,
            Categories = annotationData.Categories,
            type = annotationData.type,
        };

        m_allAnnotations.Add(System.Guid.NewGuid(), forwardLine);*/
 /*       InstantiateLineRenderer(annotationData, "1");*//*
    }*/
    public void InstantiateLineRenderer(Guid guid, AnnotationData annotationData)
    {
        if (annotationData.type == AnnotationType.twopointline && annotationData.annotationVisualizationData != null) // Type 0 is twopoint
        {
            Debug.Log("Type 0 instantiate code run");
            // Instantiate the line renderer prefab
            GameObject lineObject = Instantiate(annotationPrefabs[AnnotationType.twopointline]);
            m_instantiatedObjects.Add(guid, lineObject);
            // Get the line renderer component
            LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();
            // Parse the annotation visualization data into Vector3
            Vector3[] points = JsonConvert.DeserializeObject<Vector3[]>(annotationData.annotationVisualizationData);
            Debug.Log(points[0]);
            Debug.Log(points[1]);
            // Set the line renderer positions
            lineRenderer.SetPosition(0, points[0]);
            lineRenderer.SetPosition(1, points[1]);
        }
        else if (annotationData.type == AnnotationType.forwardline) 
        {
            Debug.Log("Type 1 instantiate code run");
            // Instantiate the line renderer prefab
            GameObject lineObject = Instantiate(annotationPrefabs[AnnotationType.forwardline]);
            m_instantiatedObjects.Add(guid, lineObject);
            // Get the line renderer component
            LineRenderer lineRenderer = lineObject.GetComponent<LineRenderer>();
            lineObject.transform.SetParent(TESTSUBJECT.transform);
            lineRenderer.SetPosition(0, TESTSUBJECT.transform.position);
            lineRenderer.SetPosition(1, TESTSUBJECT.transform.forward * 50);
        }
        else
        {
            Debug.Log("Nothing");
            return;
        }
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
                Categories = null
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
                if (!instantiatedAnnotations.Contains(anno.Key))
                {
                    InstantiateLineRenderer(anno.Key, anno.Value);
                    instantiatedAnnotations.Add(anno.Key);
                }
            }

            if ((m_instantiatedObjects.ContainsKey(anno.Key) && localCurrentTime < anno.Value.startTime) || (m_instantiatedObjects.ContainsKey(anno.Key) && localCurrentTime > anno.Value.stopTime))
            {
                if (m_instantiatedObjects.ContainsKey(anno.Key))
                {
                    GameObject GO = m_instantiatedObjects[anno.Key];
                    Destroy(GO);
                    m_instantiatedObjects.Remove(anno.Key);
                    instantiatedAnnotations.Remove(anno.Key);
                }
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
                        transform.GetChild(i).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(transform.GetChild(i).gameObject.GetComponent<RectTransform>().sizeDelta.x, 14);
                        transform.GetChild(i).gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(transform.GetChild(i).gameObject.GetComponent<RectTransform>().anchoredPosition.x, 10);
                        transform.GetChild(j).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(transform.GetChild(j).gameObject.GetComponent<RectTransform>().sizeDelta.x, 14);
                        transform.GetChild(j).gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(transform.GetChild(j).gameObject.GetComponent<RectTransform>().anchoredPosition.x, -10);

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
