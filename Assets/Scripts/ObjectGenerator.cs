using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ObjectGenerator : MonoBehaviour
{
    public Vector3 center;
    public Vector3 size;

    public int numberOfCubes = 1;
    public int numberOfGreenSpheres = 0;
    public int numberOfRedSpheres = 0;

    public GameObject cubePrefab;
    public GameObject greenSpherePrefab;
    public GameObject redSpherePrefab;
    public GameObject targetPrefab;

    private List<string> coordinates;

    public InputField scenarioInputField;
    public Toggle targetField;
    public Toggle greenSphereField;
    public Toggle redSphereField;
    

    void Start()
    {
           
    }

    void Update()
    {

    }

    public void generate()
    {
        destroyAll();
        coordinates = new List<string>();

        for (int i = 0; i < numberOfCubes; i++)
            spawnCube(false);

        if (greenSphereField.isOn){
            for (int i = 0; i < numberOfGreenSpheres; i++)
                spawnSphere(false);
        }

        if (redSphereField.isOn)
        {
            for (int i = 0; i < numberOfRedSpheres; i++)
                spawnSphere(true);
        }

        if (targetField.isOn) spawnCube(true); 
    }

    void spawnCube(bool isTarget)
    {

        Vector3 pos = center + new Vector3(Random.RandomRange(-size.x / 2, size.x / 2), 
            Random.RandomRange(-size.y / 2, size.y / 2), 
            Random.RandomRange(-size.z / 2, size.z / 2)
            );

        if (!isTarget){
            Instantiate(cubePrefab, pos, Quaternion.identity);
            string stringToAppend = pos[0].ToString() + "," + pos[1].ToString() + "," + pos[2].ToString()+",green,cube";
            //Debug.Log(stringToAppend);

            coordinates.Add(stringToAppend);
        }
        if (isTarget){
            Instantiate(targetPrefab, pos, Quaternion.identity);
            string stringToAppend = pos[0].ToString() + "," + pos[1].ToString() + "," + pos[2].ToString()+",red,cube";
            Debug.Log("Target: " +stringToAppend);

            coordinates.Add(stringToAppend);
        }

    }

    void spawnSphere(bool isRed)
    {
        
        Vector3 pos = center + new Vector3(Random.RandomRange(-size.x / 2, size.x / 2),
            Random.RandomRange(-size.y / 2, size.y / 2),
            Random.RandomRange(-size.z / 2, size.z / 2)
            );

        if (!isRed)
        {
            Instantiate(greenSpherePrefab, pos, Quaternion.identity);
            string stringToAppend = pos[0].ToString() + "," + pos[1].ToString() + "," + pos[2].ToString() + ",green,sphere";
            //Debug.Log(stringToAppend);

            coordinates.Add(stringToAppend);
        }
        if (isRed)
        {
            Instantiate(redSpherePrefab, pos, Quaternion.identity);
            string stringToAppend = pos[0].ToString() + "," + pos[1].ToString() + "," + pos[2].ToString() + ",red,sphere";
            Debug.Log("Target: " + stringToAppend);

            coordinates.Add(stringToAppend);
        }

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(center, size);
    }

    public void writeScenarioFile()
    {
        string name = scenarioInputField.text.ToString();
        string path = "Assets/Resources/" +name +".txt";

        string targetPresent, featureConj;

        targetPresent = (targetField.isOn) ? "present" : "absent";
        featureConj = (redSphereField.isOn) ? "conjunction" : "feature";

        StreamWriter sw = new StreamWriter(path);
        sw.WriteLine(targetPresent +"," +featureConj);
        sw.Close();

        File.AppendAllLines(path,coordinates.ToArray());
        Debug.Log("File written to: " +path);
    }

    public void loadScenarioFile()
    {
        destroyAll();
        string name = scenarioInputField.text.ToString();
        string path = "Assets/Resources/" + name + ".txt";

        Debug.Log("Reading: " + path);
        StreamReader sr = new StreamReader(path);
        string buffer = sr.ReadLine();  //ignore first row

        while (sr.Peek() > -1)
        {
            buffer = sr.ReadLine();
            float x, y, z;
            x = float.Parse(buffer.Split(',')[0]);
            y = float.Parse(buffer.Split(',')[1]);
            z = float.Parse(buffer.Split(',')[2]);
            string color = buffer.Split(',')[3];
            string shape = buffer.Split(',')[4];

            Vector3 pos = center + new Vector3(x, y, z);
            if (shape == "cube" && color == "green") Instantiate(cubePrefab, pos, Quaternion.identity);
            else if (shape == "cube" && color == "red") Instantiate(targetPrefab, pos, Quaternion.identity);
            else if (shape == "sphere" && color == "green") Instantiate(greenSpherePrefab, pos, Quaternion.identity);
            else if (shape == "sphere" && color == "red") Instantiate(redSpherePrefab, pos, Quaternion.identity);
        }

        sr.Close();
    }

    void destroyAll()
    {
        GameObject[] gos = GameObject.FindGameObjectsWithTag("shape");

        foreach (GameObject go in gos)
            Destroy(go);
    }
}
