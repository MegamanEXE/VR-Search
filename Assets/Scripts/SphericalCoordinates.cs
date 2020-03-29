using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

/// <summary>
/// In mathematics, a spherical coordinate system is a coordinate system for 
/// three-dimensional space where the position of a point is specified by three numbers: 
/// the radial distance of that point from a fixed origin, its inclination angle measured 
/// from a fixed zenith direction, and the azimuth angle of its orthogonal projection on 
/// a reference plane that passes through the origin and is orthogonal to the zenith, 
/// measured from a fixed reference direction on that plane. 
/// 
/// The zenith direction is the up vector (0,1,0) and the azimuth is the right vector (1,0,0)
/// 
/// (From http://en.wikipedia.org/wiki/Spherical_coordinate_system )
/// </summary>

public class SphericalCoordinates : MonoBehaviour
{
    /// <summary>
    /// the radial distance of that point from a fixed origin.
    /// Radius must be >= 0
    /// </summary>
    public float radius;
    /// <summary>
    /// azimuth angle (in radian) of its orthogonal projection on 
    /// a reference plane that passes through the origin and is orthogonal to the zenith
    /// </summary>
    public float polar;
    /// <summary>
    /// elevation angle (in radian) from the reference plane 
    /// </summary>
    public float elevation;

    /// <summary>
    /// Converts a point from Spherical coordinates to Cartesian (using positive
    /// * Y as up)
    /// </summary>
    /// 
    private string toString()
    {
        return String.Format("({0},{1},{2})", radius, polar, elevation);
    }

    public Vector3 ToCartesian()
    {
        Vector3 res = new Vector3();
        SphericalToCartesian(radius, polar, elevation, out res);
        return res;
    }

    /// <summary>
    /// Converts a point from Cartesian coordinates (using positive Y as up) to
    /// Spherical and stores the results in the store var. (Radius, Azimuth,
    /// Polar)
    /// </summary>
    public SphericalCoordinates CartesianToSpherical(Vector3 cartCoords)
    {
        SphericalCoordinates store = new SphericalCoordinates();
        CartesianToSpherical(cartCoords, out store.radius, out store.polar, out store.elevation);
        return store;
    }

    /// <summary>
    /// Converts a point from Spherical coordinates to Cartesian (using positive
    /// * Y as up). All angles are in radians.
    /// </summary>
    public void SphericalToCartesian(float radius, float polar, float elevation, out Vector3 outCart)
    {
        float a = radius * Mathf.Cos(elevation);
        outCart.x = a * Mathf.Cos(polar);
        outCart.y = radius * Mathf.Sin(elevation);
        outCart.z = a * Mathf.Sin(polar);
    }
    //Overloaded
    public Vector3 SphericalToCartesian(float radius, float polar, float elevation)
    {
        float x, y, z;
        float a = radius * Mathf.Cos(elevation);
        x = a * Mathf.Cos(polar);
        y = radius * Mathf.Sin(elevation);
        z = a * Mathf.Sin(polar);

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Converts a point from Cartesian coordinates (using positive Y as up) to
    /// Spherical and stores the results in the store var. (Radius, Azimuth,
    /// Polar)
    /// </summary>
    public void CartesianToSpherical(Vector3 cartCoords, out float outRadius, out float outPolar, out float outElevation)
    {
        if (cartCoords.x == 0)
            cartCoords.x = Mathf.Epsilon;
        outRadius = Mathf.Sqrt((cartCoords.x * cartCoords.x)
                        + (cartCoords.y * cartCoords.y)
                        + (cartCoords.z * cartCoords.z));
        outPolar = Mathf.Atan(cartCoords.z / cartCoords.x);
        if (cartCoords.x < 0)
            outPolar += Mathf.PI;
        outElevation = Mathf.Asin(cartCoords.y / outRadius);
    }

    ///////////////////////// Spherical Coordinates end here /////////////////////////

    public Vector3 center;
    public float size;

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

    private System.Random r;

    private const float offset = 5.5f;
    void Start()
    {
        r = new System.Random();
        

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawSphere(center, size);
    }

    void spawn() ////////////////////////////////////////////// Generic
    {
        float randRadius, randPolar, randElevation;
        randRadius = (float)r.NextDouble() * (size-offset)+offset;
        randPolar = (float)r.NextDouble() * (2 * Mathf.PI);
        randElevation = (float)r.NextDouble() * (2 * Mathf.PI);

        Debug.Log(String.Format("{0}, {1}, {2}",randRadius,randPolar,randElevation));

        Vector3 pos = SphericalToCartesian(randRadius, randPolar, randElevation);

        string forWriting = String.Format("s,{0},{1},{2},c,{3},{4},{5}",randRadius,randPolar,randElevation,
                                                                        pos.x,pos.y,pos.z);
        Instantiate(cubePrefab, pos, Quaternion.identity);        
    }
    //////////////////////////////////////////////////////////////////////////////////////////////////////

    public void generate()
    {
        destroyAll();
        coordinates = new List<string>();

        for (int i = 0; i < numberOfCubes; i++)
            spawnCube(false);

        if (greenSphereField.isOn)
        {
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
        float randRadius, randPolar, randElevation;
        randRadius = (float)r.NextDouble() * (size - offset) + offset;
        randPolar = (float)r.NextDouble() * (2 * Mathf.PI);
        randElevation = (float)r.NextDouble() * (2 * Mathf.PI);

        Vector3 pos = SphericalToCartesian(randRadius, randPolar, randElevation);

        string coordString = String.Format("s,{0},{1},{2},c,{3},{4},{5}", randRadius, randPolar, randElevation,
                                                                        pos.x, pos.y, pos.z);

        if (!isTarget)
        {
            Instantiate(cubePrefab, pos, Quaternion.identity);
            string stringToAppend = coordString + ",green,cube";
            
            coordinates.Add(stringToAppend);
        }
        if (isTarget)
        {
            Instantiate(targetPrefab, pos, Quaternion.identity);
            string stringToAppend = coordString + ",red,cube";

            coordinates.Add(stringToAppend);
        }

    }

    void spawnSphere(bool isRed)
    {

        float randRadius, randPolar, randElevation;
        randRadius = (float)r.NextDouble() * (size - offset) + offset;
        randPolar = (float)r.NextDouble() * (2 * Mathf.PI);
        randElevation = (float)r.NextDouble() * (2 * Mathf.PI);

        Vector3 pos = SphericalToCartesian(randRadius, randPolar, randElevation);

        string coordString = String.Format("s,{0},{1},{2},c,{3},{4},{5}", randRadius, randPolar, randElevation,
                                                                        pos.x, pos.y, pos.z);

        if (!isRed)
        {
            Instantiate(greenSpherePrefab, pos, Quaternion.identity);
            string stringToAppend = coordString + ",green,sphere";

            coordinates.Add(stringToAppend);
        }
        if (isRed)
        {
            Instantiate(redSpherePrefab, pos, Quaternion.identity);
            string stringToAppend = coordString + ",red,sphere";
            
            coordinates.Add(stringToAppend);
        }

    }

    public void writeScenarioFile()
    {
        string name = scenarioInputField.text.ToString();
        string path = "Assets/Resources/" + name + ".txt";

        string targetPresent, featureConj;

        targetPresent = (targetField.isOn) ? "present" : "absent";
        featureConj = (redSphereField.isOn) ? "conjunction" : "feature";

        StreamWriter sw = new StreamWriter(path);
        sw.WriteLine(targetPresent + "," + featureConj);
        sw.Close();

        File.AppendAllLines(path, coordinates.ToArray());
        Debug.Log("File written to: " + path);
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
            x = float.Parse(buffer.Split(',')[5]);  //Updated for spherical coordinates, xyz begin at 5
            y = float.Parse(buffer.Split(',')[6]);
            z = float.Parse(buffer.Split(',')[7]);
            string color = buffer.Split(',')[8];
            string shape = buffer.Split(',')[9];

            float rRadius, rPolar, rElevation; //Spherical. Irrelevant for rendering but reading for any future functionality
            rRadius = float.Parse(buffer.Split(',')[1]);
            rPolar = float.Parse(buffer.Split(',')[2]);
            rElevation = float.Parse(buffer.Split(',')[3]);

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