using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GrammarBotClient;
using IBM.Watson.Examples;

public class DialougeTrigger : MonoBehaviour
{

    public GameObject dialogBox;
    public GameObject textBox;
    public GameObject playerBox;
    public GameObject playerText;
    public GameObject NPCText;
    public GameObject NPCBox;
    public GameObject recordIcon;
    public GameObject grammarBox;
    public GameObject WordBox;
    public GameObject wordList;
    public GameObject inputRelevance;
    public GameObject finishBox;
    private TextMeshProUGUI dialogue_content;
    private TextMeshProUGUI playerContent;
    private TextMeshProUGUI NPCContent;
    private TextMeshProUGUI GrammarMessage;
    private TextMeshProUGUI WordListContent;
    private TextMeshProUGUI inputRelevanceText;
    private string[] words;
    public ExampleStreaming exampleStreaming;
    public ExampleNaturalLanguageUnderstandingV1 NLU;
    private GrammarBot grammarBot;
    private Dictionary<string, List<string>> questionList;

    const int WORD_LIST_LENGH = 3;
    int wordsLeft = WORD_LIST_LENGH;


    bool inside = false;
    bool ready = false;
    bool recording = false;
    bool isOpenWordList = true;
    bool happybot = false;
    //bool similar = false;
    bool correctSent = true;
    string topic = "";
    string oldNPCText = "";
    
    void Start() //Setup dialogue boxes
    {
        dialogBox.SetActive(false);
        playerBox.SetActive(false);
        NPCBox.SetActive(false);
        recordIcon.SetActive(false);
        WordBox.SetActive(true);
        finishBox.SetActive(false);
        dialogue_content = textBox.GetComponent<TextMeshProUGUI>();
        playerContent = playerText.GetComponent<TextMeshProUGUI>();
        NPCContent= NPCText.GetComponent<TextMeshProUGUI>();
        GrammarMessage = grammarBox.GetComponent<TextMeshProUGUI>();
        inputRelevanceText = inputRelevance.GetComponent<TextMeshProUGUI>();
        GrammarMessage.text = "";

        WordListContent = wordList.GetComponent<TextMeshProUGUI>();   // Word list of words to be covered in level 1
        LoadQuestions();
        words = new string[WORD_LIST_LENGH];
        words[0] = "recover";
        words[1] = "member";
        words[2] = "grow";
        //words[3] = "worry"; 
        //words[4] = "grow";

        WordListContent.text = "";
        RefreshWordList();

        grammarBot = new GrammarBotClient.GrammarBot(new GrammarBotClient.ApiConfig());

    }

    string GetDialogue(string topic)  //Get dialogues relevant to the location
    {
        string textOut="";
        if (questionList.ContainsKey(topic))
        {
            List<string> questions = questionList[topic];
            if (questions.Count > 0)
            {
                int index = Random.Range(0, questions.Count);
                textOut = questions[index];
                questions.RemoveAt(index);
            }
            else
            {
                textOut = "Out of Questions";
            }
        }
        else
        {
            textOut = "Questions have not set up!";
        }
        
        return textOut;
    }

    void Update()
    {
        // check if ready for dialogue
        if (recording)
        {
            playerContent.text = exampleStreaming.TextOutput();
        }
            
        if (inside == true && !ready) //Enable conversation at "chat" spots
        {
            dialogBox.SetActive(true);
            dialogue_content.text = "Would you like to talk to " + topic + "? \n"
                    + "(Press Space to talk)";
        }
        if (inside == true && (Input.GetKeyDown(KeyCode.Space))&&!ready) //Set up conversation boxes
        {
            // open dialogbox
            NPCContent.text = GetDialogue(topic);
            playerBox.SetActive(true);
            NPCBox.SetActive(true);
            dialogBox.SetActive(false);
            playerContent.text = "";
            inputRelevanceText.text = "";
            GrammarMessage.text = "";
            ready = true;

            // get dialogue data
        }else if (ready)
        {
            if (Input.GetKeyDown(KeyCode.Space)) //Close the conversation boxes once Conversation is done
            {
                ready = false;
                playerContent.text = "";
                inputRelevanceText.text = "";
                playerBox.SetActive(false);
                NPCBox.SetActive(false);

                if(recording){
					exampleStreaming.StopRec();
	                recording = false;
	                recordIcon.SetActive(false);
                }
            }
            TS();
        }

        if (Input.GetKeyDown(KeyCode.Tab)) //Control the word list box
        {
            if(!isOpenWordList)
            {
                WordBox.SetActive(true);
                isOpenWordList = true;
            } 
            else
            {
                WordBox.SetActive(false);
                isOpenWordList = false;
            }
            
        }
    }

    async void CheckGrammar(string content) //Call the grammarbot and display any errors
    {
        var grammar = await grammarBot.CheckAsync(content);
        if (grammar.Success)        // if there is grammar mistakes or not started with uppercase letter 
        {
            int posOffset = 0;
            foreach (var item in grammar.CheckContent.Matches)
            {
                if(item.Message != "This sentence does not start with an uppercase letter") 
                {
                    int pos = (int)item.offset;
                    int len = (int)item.Length;

                    playerContent.text = playerContent.text.Substring(0, pos + posOffset) + 
                        "<color=red>" + playerContent.text.Substring(pos + posOffset, len) + "</color>" +
                        playerContent.text.Substring(pos + len + posOffset);    

                    posOffset += 19;

                    GrammarMessage.text = GrammarMessage.text + item.Message + "\n";
                    Debug.Log(item.Message);
                    correctSent = false;
                }
                else                // if there is no grammar mistakes
                {
                    correctSent = true;
                }
            }
        }
        else                    // if there is no grammar mistakes
        {
            Debug.Log(grammar.Message);
            correctSent = true;
        }
        
    }


    public void CrossOutWord(string context) //Cross out the word in word list if present in the given sentence 
    {
        string[] contextWords = context.Replace("'"," ").Split(' ');
        foreach (var contextWord in contextWords)
        {
            for (int i = 0; i < words.Length; i++)
            {
                if(contextWord.Equals(words[i]))
                {
                    words[i] = StrikeThrough(words[i]);
                    wordsLeft--;
                    if (wordsLeft == 0)
                    {
                        finishBox.SetActive(true);
                    }
                }
            }
        }
        RefreshWordList();
    }

    public void RefreshWordList()
    {
        WordListContent.text = "";
        foreach (string word in words)
        {
            WordListContent.text = WordListContent.text + word + "\n";
        }
    }

    public string StrikeThrough(string s) //Strike through word once covered
    {
        string strikethrough = "";
        foreach (char c in s)
        {
            strikethrough = strikethrough + c + '\u0336';
        }
        return strikethrough;
    }

    void TS()
    {
        if (Input.GetKeyDown(KeyCode.R)) //Control recording on R
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
        }
        if (playerContent.text != "") //Once we get the text from Speech to Text, Check its grammar and call Natural Language Understanding
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                CheckGrammar(playerContent.text);
                NLU.AnalyzeText(playerContent.text);
                NLU.AnalyzeText(NPCContent.text);
                happybot = true;
            }
        }
        if (happybot) //If the Natural language understanding analysis is complete
        {
            if (NLU.sentenceValues.ContainsKey(playerContent.text) && NLU.sentenceValues.ContainsKey(NPCContent.text))
            {
                happybot = false;
                double? similar = CompareText(NLU.sentenceValues[playerContent.text], NLU.sentenceValues[NPCContent.text]);

                if (similar > 0.2) //Set similarity threshold to 0.2 and check if the sentences cross the threshold
                {
                    inputRelevanceText.text = "Content similarity Score: " + similar.Value.ToString("P2")+"\n"+"Is similar: "+ "<color=green>"+"Yes"+ "</color>";
                    if (correctSent)
                    {
                        CrossOutWord(NLU.sentenceValues[playerContent.text].lemma);
                    }
                }
                else
                {
                    inputRelevanceText.text = "Content similarity Score: " + similar.Value.ToString("P2") + "\n" + "Is similar: " + "<color=red>" + "No" + "</color>";
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Q)) //Get next question on Q
        {
            NPCContent.text = GetDialogue(topic);
            playerContent.text = "";
            GrammarMessage.text = "";
            inputRelevanceText.text = "";
        }
    }

    void OnTriggerEnter2D(Collider2D other) //Use a trigger to check if we have entered a "chat" location
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

    void LoadQuestions() //Setup questions
    {
        questionList = new Dictionary<string, List<string>>();
        List<string> temp = new List<string>();
        temp.Add("Would you like some whipped cream with that hot chocolate? We have some new breakfast sandwiches today!");
        temp.Add("How do you like your coffee? And would you like some bread to eat with that?");
        temp.Add("What blend of beans would you like in your coffee? And would you like me to heat up that muffin?");
        questionList.Add("Coffee", temp);
        List<string> temp2 = new List<string>();
        temp2.Add("Welcome! We have some fresh berries in our produce aisle today. There’s some organic jam to go with them too.");
        temp2.Add("Hello! We have a special offer on sanitizers and hand wash soaps. Couple that with some healthy fruit snacks and you’ll be good to fight any virus in the world. ");
        temp2.Add("Welcome to the only grocery store in town that offers fresh organic, non-gmo and ethical fruits and vegetables. We offer wholesome food, home and beauty products to anyone who wants to save the day. ");
        questionList.Add("Grocery", temp2);
        List<string> temp3 = new List<string>();
        temp3.Add("Hi! Are you here to check out our Aquatic center? We recently renovated our swimming pool.");
        temp3.Add("Hello! Would you like to enroll in our gym's membership program? We have several trained professionals offering classes ranging from kickboxing to cycling and yoga.");
        temp3.Add("Hey! Check out our proshop rentals for some select sports equipment. You can purchase a locker to keep your personal exercise equipment.");
        questionList.Add("Fitness", temp3);
    }

    double? CompareText(ans a, ans q) //Compare the categories from the question answer-pair in the chat
    {
        double? inter = 0.00;
        double? union = 0.00;
        List<string> temp = new List<string>();
        foreach (var c1 in a.cat)
        {
            foreach (var c2 in q.cat)
            {
                if (c1.mainlabel == c2.mainlabel)
                {
                    inter = inter + ((c1.score + c2.score) / 2);
                    temp.Add(c1.mainlabel);
                }
            }
        }
        union = inter;
        foreach (var c1 in a.cat)
        {
            if (!temp.Contains(c1.mainlabel))
            {
                union = union + c1.score;
            }
        }
        foreach (var c2 in q.cat)
        {
            if (!temp.Contains(c2.mainlabel))
            {
                union = union + c2.score;
            }

        }
        double? score = inter / union;

        Debug.Log("score: " + score.Value.ToString("P2"));
        return score;
    }
  
}
