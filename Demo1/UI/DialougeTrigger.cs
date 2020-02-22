using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using IBM.Watsson.Examples;

public class DialougeTrigger : MonoBehaviour
{

    public GameObject dialogBox;
    public GameObject textBox;
    public GameObject playerBox;
    public GameObject playerText;
    public GameObject NPCText;
    public GameObject NPCBox;
    public GameObject recordIcon;
    private TextMeshProUGUI dialogue_content;
    private TextMeshProUGUI playerContent;
    private TextMeshProUGUI NPCContent;
    public ExampleStreaming exampleStreaming;

    bool inside = false;
    bool ready = false;
    bool recording = false;
    string topic = "";
    
    void Start()
    {
        dialogBox.SetActive(false);
        playerBox.SetActive(false);
        NPCBox.SetActive(false);
        recordIcon.SetActive(false);
        dialogue_content = textBox.GetComponent<TextMeshProUGUI>();
        playerContent = playerText.GetComponent<TextMeshProUGUI>();
        NPCContent= NPCText.GetComponent<TextMeshProUGUI>();
        //playerContent.text = "";


    }

    string GetDialogue(string topic)
    {
        string textOut="";
        if (topic == "Coffee")
        {
            textOut = "How many cups of coffee do you drink in a day?";
        }
        else if (topic == "Library")
        {
            textOut="What type of book genres do you read?";
        }else if(topic== "Grocery")
        {
            textOut="What is your favorite breakfast meal to eat?";
        }
        return textOut;
    }
    void Update()
    {
        // check if ready for dialogue
        //playerContent.text = exampleStreaming.TextOutput();
        if (recording)
        {
            //playerContent.text = playerContent.text + "\n" + exampleStreaming.TextOutput();
            playerContent.text = exampleStreaming.TextOutput();
        }
            
        
            
        if (inside == true && !ready)
        {
            dialogBox.SetActive(true);
            dialogue_content.text = "Would you like to talk to " + topic + "? \n"
                    + "(Press Space or Enter to talk)";

            NPCContent.text = GetDialogue(topic);
        }
        if (inside == true && (Input.GetKeyDown(KeyCode.Space))&&!ready)
        {
            // open dialogbox
            //dialogBox.SetActive(true);
            playerBox.SetActive(true);
            NPCBox.SetActive(true);
            dialogBox.SetActive(false);
            playerContent.text = "";
            ready = true;

            // get dialogue data
        }else if (ready)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ready = false;
                playerContent.text = "";
                playerBox.SetActive(false);
                NPCBox.SetActive(false);

            }
            TS();
        }
            

            //dialogue_content.text = "getting dialogue from AI for topic: " + topic;
            //dialogue_content.text = exampleStreaming.TextOutput();
        

        //dialogue_content.text = exampleStreaming.TextOutput();
    }

    void TS()
    {
        //dB2.SetActive(true);
        //Debug.Log("TS called");
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (!recording)
            {
                exampleStreaming.StartRec();
                recording = true;
                recordIcon.SetActive(true);
            }
            else
            {
                exampleStreaming.StopRec();
                recording = false;
                recordIcon.SetActive(false);
            }
            
            //Debug.Log("recording");
        }
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        inside = true;
        Debug.Log("Object that entered the trigger : " + other);

        // save current colliding object name
        topic = other.gameObject.name;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        inside = false;
        dialogBox.SetActive(false);
        

    }
  
}
