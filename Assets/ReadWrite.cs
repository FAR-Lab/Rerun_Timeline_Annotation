/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEditor;
using System.IO;

public class ReadWrite : MonoBehaviour
{
    string path = "Assets/Resources/test.json";
    void Start()
    {

        string c = LoadJsonFile("Assets/Resources/test.json");
        AnnotationData bAnnotation = JsonConvert.DeserializeObject<AnnotationData>(c);
        Debug.Log(bAnnotation.startTime);

        Product b = JsonConvert.DeserializeObject<Product>(c);
        Debug.Log(b.Name);
    }
    private void OnApplicationQuit()
    {
        WriteJsonFile(jsonfile, path);
    }
    public string LoadJsonFile(string m_path) //basically implement textasset but with streamreader instead since textasset is read only
    {

        StreamReader sr = new StreamReader(m_path);
        var read = sr.ReadToEnd();

        return read;
    }

    private void WriteJsonFile(string jsonfile, string m_path)
    {
        Debug.Log("Trying to write to: " + m_path);

        StreamWriter writer = new StreamWriter(m_path, false);
        writer.Write(jsonfile);

        writer.Close();
    }




}*/