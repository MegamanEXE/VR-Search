using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class IntroScript : MonoBehaviour
{
    public InputField participantID;
    public InputField experimenter;
    public InputField startTime;
    public Text warning;

    public string particID;
    public string expName;
    public string expTime;

    EventSystem system;
    // Start is called before the first frame update
    void Start()
    {
        system = EventSystem.current;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if (next != null)
            {

                InputField inputfield = next.GetComponent<InputField>();
                if (inputfield != null) inputfield.OnPointerClick(new PointerEventData(system));  //if it's an input field, also set the text caret

                system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
                
            }
            //else Debug.Log("next nagivation element not found");
        }
    }

    public void startExperiment()
    {
        if ((participantID.text == "") || (experimenter.text == "") || (startTime.text == ""))
        {
            warning.gameObject.SetActive(true);
        }
        else
        {
            SharedData._participantID = participantID.text;
            SharedData._experimenterName = experimenter.text;
            SharedData._expTime = startTime.text;

            SceneManager.LoadScene("Experiment");
        }
    }
}
