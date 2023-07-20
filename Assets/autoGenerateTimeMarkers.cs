using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class autoGenerateTimeMarkers : MonoBehaviour
{
    // Start is called before the first frame update
    private double videoLength;
    public GameObject timeMarker;

    GameObject a;
    GameObject b;
    GameObject c;
    GameObject d;

    List<GameObject> alreadyInstantiated = new List<GameObject>();

    void Start()
    {

    }

    public List<int> GenerateMarkers(double videoLength)
    {
        var multiplesOfFive = new List<int>();
        var markers = new List<int>();

        for (int i = 5; i <= videoLength; i += 5)
        {
            multiplesOfFive.Add(i);
        }

        for (int i = 1; i < 3; i++)
        {
            var quarter = i * videoLength / 4.0;

        //Find closest multiple of 5
            var closestMarker = 0;
            var smallestDiff = double.MaxValue;
            foreach (int marker in multiplesOfFive)
            {
                var difference = Mathf.Abs((float)(marker - quarter));
                if(difference <= smallestDiff)
                {
                    smallestDiff = difference;
                    closestMarker = marker;
                }
            }

            if(closestMarker < videoLength)
            {
                markers.Add(closestMarker);
            }
        }
        int diffCenterAndFirst = markers[1] - markers[0];
        markers.Add(markers[1] + diffCenterAndFirst);
        markers.Add((int)videoLength);

        return markers;
    }

    public void InstantiateNamePositionMarkers(List<int> markers)
    {
        foreach(GameObject a in alreadyInstantiated)
        {
            Destroy(a);
        }
        alreadyInstantiated.Clear();

        a = Instantiate(timeMarker, this.transform);
        a.GetComponent<TMP_Text>().text = "0";
        a.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, Screen.height / 25);
        a.GetComponent<TMP_Text>().fontSize = Screen.height / 15;
        alreadyInstantiated.Add(a);

        GameObject b = Instantiate(timeMarker, this.transform);
        b.GetComponent<TMP_Text>().text = markers[0].ToString();
        b.GetComponent<RectTransform>().anchoredPosition = new Vector2(this.transform.GetComponent<RectTransform>().rect.width * (markers[0]/GetComponentInParent<timeline>().localLength), Screen.height / 45);
        b.GetComponent<TMP_Text>().fontSize = Screen.height / 20;
        alreadyInstantiated.Add(b);

        GameObject c = Instantiate(timeMarker, this.transform);
        c.GetComponent<TMP_Text>().text = markers[1].ToString();
        c.GetComponent<RectTransform>().anchoredPosition = new Vector2(this.transform.GetComponent<RectTransform>().rect.width * (markers[1] / GetComponentInParent<timeline>().localLength), Screen.height / 45);
        c.GetComponent<TMP_Text>().fontSize = Screen.height / 20;
        alreadyInstantiated.Add(c);

        GameObject d = Instantiate(timeMarker, this.transform);
        d.GetComponent<TMP_Text>().text = markers[2].ToString();
        d.GetComponent<RectTransform>().anchoredPosition = new Vector2(this.transform.GetComponent<RectTransform>().rect.width * (markers[2] / GetComponentInParent<timeline>().localLength), Screen.height / 45);
        d.GetComponent<TMP_Text>().fontSize = Screen.height / 20;
        alreadyInstantiated.Add(d);

        GameObject e = Instantiate(timeMarker, this.transform);
        e.GetComponent<TMP_Text>().text = markers[3].ToString();
        e.GetComponent<RectTransform>().anchoredPosition = new Vector2(this.transform.GetComponent<RectTransform>().rect.width * (markers[3] / GetComponentInParent<timeline>().localLength) , Screen.height / 25);
        e.GetComponent<TMP_Text>().fontSize = Screen.height / 15;
        alreadyInstantiated.Add(e);
    }

    public void ReInstantiateMarkers(double vidLength)
    {
        List<int> listofmarkers = GenerateMarkers(vidLength);
        InstantiateNamePositionMarkers(listofmarkers);
    }
}
