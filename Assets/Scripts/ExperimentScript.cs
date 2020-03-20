using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExperimentScript : MonoBehaviour
{
    //Gizmo
    public Vector3 center;  
    public Vector3 size;
    public const int MAX_TRIALS = 3;
    public bool targetPresent;
    public string searchType;

    public GameObject cubePrefab;
    public GameObject greenSpherePrefab;
    public GameObject redSpherePrefab;
    public GameObject targetPrefab;

    public GameObject panelRef;
    public Text timeResultDisplay;

    float timer = 0.0f;
    string currentTrial;
    string decisionTime;
    string answerCorrect;
    string response;
    
    bool alreadyInput = false;
    static int trialNum = 0;    //counts trials so far
    static bool initializedTrialPool = false;
    static HashSet<int> trialCompleted = new HashSet<int>();
    static Stack<int> trialIndex = new Stack<int>();

    static StreamWriter resultWriter = new StreamWriter("Assets/Results/results.txt");

    void Start()
    {
        alreadyInput = false;
        timer = 0.0f;
        answerCorrect = "";
        response = "";

        generateTrialNames();

        if (trialNum >= MAX_TRIALS) { resultWriter.Close(); SceneManager.LoadScene("End"); }
        runRandomTrial();
        trialNum++;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Y) && !alreadyInput) {
            alreadyInput = true; 
            answerCorrect = (targetPresent) ? "correct" : "incorrect";
            response = "True";
            decisionTime = timer.ToString("0.00");
            timeResultDisplay.text = "Time: " +decisionTime +"s";

            writeResult();

            panelRef.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.N) && !alreadyInput) {
            alreadyInput = true;
            answerCorrect = (!targetPresent) ? "correct" : "incorrect";
            response = "False";
            decisionTime = timer.ToString("0.00");
            timeResultDisplay.text = "Time: " + decisionTime + "s";

            writeResult();

            panelRef.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Space) && alreadyInput){
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        }
    }

    void readFile(string filename)
    {
        string buffer;
        string path = "Assets/Resources/" +filename +".txt";

        StreamReader sr = new StreamReader(path);
        buffer = sr.ReadLine();
        
        targetPresent = (buffer.Split(',')[0] == "present") ? true : false;
        searchType = buffer.Split(',')[1];


        while(sr.Peek() > -1){
            buffer = sr.ReadLine();
            float x, y, z;
            x = float.Parse(buffer.Split(',')[0]);
            y = float.Parse(buffer.Split(',')[1]);
            z = float.Parse(buffer.Split(',')[2]);
            string color = buffer.Split(',')[3];
            string shape = buffer.Split(',')[4];

            //Debug.Log(string.Format("{0} {1} {2} {3} {4}", x,y,z,color,shape));

            Vector3 pos = center + new Vector3(x,y,z);            
            if (shape == "cube" && color == "green") Instantiate(cubePrefab, pos, Quaternion.identity);
            else if (shape == "cube" && color == "red") Instantiate(targetPrefab, pos, Quaternion.identity);
            else if (shape == "sphere" && color == "green") Instantiate(greenSpherePrefab, pos, Quaternion.identity);
            else if (shape == "sphere" && color == "red") Instantiate(redSpherePrefab, pos, Quaternion.identity);
        }

        sr.Close();
    }

    void writeResult()
    {
        //trialNumber, correctness, actual, response, time
        string buffer = currentTrial + "," + answerCorrect + "," + targetPresent.ToString() + "," + response + "," + decisionTime;
        resultWriter.WriteLine(buffer);
        
    }

    void generateTrialNames()
    {
        if (!initializedTrialPool)
        {
            initializedTrialPool = true;

            //HACK: wrote result column names here because this code is executed only once at the start
            resultWriter.WriteLine("TrialNumber,Correctness,Actual,Response,Time");

            System.Random rnd = new System.Random();
            while (trialCompleted.Count < MAX_TRIALS)
            {
                trialCompleted.Add(rnd.Next(MAX_TRIALS) + 1);
            }

            foreach (int i in trialCompleted){
                trialIndex.Push(i);
            }
        }

    }

    void runRandomTrial()
    {
        string index = trialIndex.Pop().ToString(); //don't need .txt because readFile() does it
        readFile("s" +index); 
        Debug.Log("Running trial: s" +index);
        currentTrial = "s" +index;

    }

    void OnApplicationQuit()
    {
        if(resultWriter.BaseStream != null) resultWriter.Close();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(center, size);
    }
}
