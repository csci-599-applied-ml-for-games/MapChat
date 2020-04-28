using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GrammarBotClient;
using IBM.Watson.Examples;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.SceneManagement;
using IBM.Cloud.SDK;
using UnityEngine.SocialPlatforms.Impl;

[System.Serializable]
public class QuestionsPair
{
    public string type;
    public List<string> questions;
}
[System.Serializable]
public class QuestionsList
{
    public List<QuestionsPair> list;
}
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
    public GameObject finishMessage;
    public GameObject scoreInList;
    public GameObject scoreInBox;
    private TextMeshProUGUI dialogue_content;
    private TextMeshProUGUI playerContent;
    private TextMeshProUGUI NPCContent;
    private TextMeshProUGUI GrammarMessage;
    private TextMeshProUGUI WordListContent;
    private TextMeshProUGUI inputRelevanceText;
    private TextMeshProUGUI finishMessageText;
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI scoreText2;
    
    private List<string> words;
    public ExampleStreaming exampleStreaming;
    public ExampleNaturalLanguageUnderstandingV1 NLU;
    private GrammarBot grammarBot;
    private Dictionary<string, List<string>> questionList;
    private Dictionary<string, int> questionLeft;

    int wordsLeft;
    bool inside = false;
    bool ready = false;
    bool recording = false;
    bool isOpenWordList = true;
    bool happybot = false;
    bool correctSent = true;
    bool isRoundFinish = false;
    bool isWin = false;
    string topic = "";
    string userLogPath;
    string userScorePath;
    double scoreRound;
    double scoreTotal;
    const int wordNumEasy = 3;
    const int wordNumMed = 4;
    const int wordNumHard = 4;
    const int scoreMed = 30;
    const int scoreHard = 70;
    const int questionNum = 2;
    
    void Start()
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
        finishMessageText = finishMessage.GetComponent<TextMeshProUGUI>();
        scoreText = scoreInList.GetComponent<TextMeshProUGUI>();
        scoreText2 = scoreInBox.GetComponent<TextMeshProUGUI>();
        GrammarMessage.text = "";
        userLogPath = Application.dataPath + "/Scripts/userLog.txt";
        userScorePath = Application.dataPath + "/Scripts/userScore.txt";
        WordListContent = wordList.GetComponent<TextMeshProUGUI>();
        
        File.WriteAllText(userLogPath, "");
        if (!File.Exists(userScorePath))
        {
            File.WriteAllText(userScorePath, "0");
        }
        scoreTotal = double.Parse(File.ReadAllText(userScorePath));
        if (scoreTotal >= scoreHard && SceneManager.GetActiveScene().name != "T1")
        {
            SceneManager.LoadScene("T1");
        }
        scoreRound = 0.0;
        scoreText.text ="Total Score: "+scoreTotal.ToString("F0")+"\n"+"Round Score: "+scoreRound.ToString("F0");
        LoadQuestions(scoreTotal);
        loadWords(scoreTotal);
        WordListContent.text = "";
        RefreshWordList();
        grammarBot = new GrammarBotClient.GrammarBot(new GrammarBotClient.ApiConfig());
    }

    // Get dialogue of given topic from loaded question list
    string GetDialogue(string topic)
    {
        string textOut="";
        bool hasQues = false;
        foreach(var i in questionLeft.Keys)
        {
            if (questionLeft[i] > 0)
            {
                hasQues = true;
                break;
            }
        }
        if (!hasQues)
        {
            finishMessageText.text = "Oh! You Failed This Round!";
            finishBox.SetActive(true);
            isRoundFinish = true;
            scoreRound = 0;
            File.WriteAllText(userScorePath, scoreTotal.ToString());
            scoreText2.text = "Total Score: " + scoreTotal.ToString("F0") + "\n" + "Round Score: " + scoreRound.ToString("F0");
        }
        if (questionList.ContainsKey(topic))
        {
            List<string> questions = questionList[topic];
            if (questionLeft[topic] > 0)
            {
                int index = Random.Range(0, questions.Count);
                textOut = questions[index];
                questions.RemoveAt(index);
                questionLeft[topic]--;
            }
            else
            {
                textOut = "Sorry we are closed!";
            }
        }
        else
        {
            textOut = "Sorry we are closed!";
        }
        
        return textOut;
    }

    void loadWords(double score)
    {
        words = new List<string>();
        string wordString;
        if (score < scoreMed)
        {
            wordString = File.ReadAllText(Application.dataPath + "/Scripts/words_easy_new.txt");
            wordsLeft = wordNumEasy;
        }
        else if (score < scoreHard)
        {
            wordString = File.ReadAllText(Application.dataPath + "/Scripts/words_medium_new.txt");
            wordsLeft = wordNumMed;
        }
        else
        {
            wordString = File.ReadAllText(Application.dataPath + "/Scripts/words_hard_new.txt");
            wordsLeft = wordNumHard;
        }
        List<string> list = new List<string>(wordString.Split('\n'));
        for (int i = 0; i < wordsLeft; i++)
        {
            int idx = Random.Range(0, list.Count);
            words.Add(list[idx].Trim());
            list.RemoveAt(idx);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            isRoundFinish = true;
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            scoreTotal += 10;
        }
        // check if ready for dialogue
        if (recording)
        {
            playerContent.text = exampleStreaming.TextOutput();
        }
            
        if(isRoundFinish && Input.GetKeyDown(KeyCode.Return))
        {
            if(isWin&& SceneManager.GetActiveScene().name == "T1")
            {
                SceneManager.LoadScene("startMenu");
            }
            else 
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
            
        if (inside == true && !ready)
        {
            dialogBox.SetActive(true);
            dialogue_content.text = "Would you like to talk to " + topic + "? \n"
                    + "(Press Space to talk)";

            
        }
        if (inside == true && (Input.GetKeyDown(KeyCode.Space))&&!ready)
        {
            NPCContent.text = GetDialogue(topic);
            playerBox.SetActive(true);
            NPCBox.SetActive(true);
            dialogBox.SetActive(false);
            playerContent.text = "";
            inputRelevanceText.text = "";
            GrammarMessage.text = "";
            ready = true;
        }else if (ready)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ready = false;
                playerContent.text = "";
                inputRelevanceText.text = "Input Relevance: ";
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
        if (Input.GetKeyDown(KeyCode.Tab))
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


    // Call GrammarBot API to check grammar of user's input
    async void CheckGrammar(string content)
    {
        var grammar = await grammarBot.CheckAsync(content);
        double grammarScore = 5.0f;
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
                    File.AppendAllText(userLogPath, GrammarMessage.text + "\n");
                    correctSent = false;
                    grammarScore -= 1f;
                }
                else                // if there is no grammar mistakes
                {
                    //CrossOutWord(content);
                    File.AppendAllText(userLogPath, "No grammar mistake\n");
                    correctSent = true;
                }
            }
        }
        else                    // if there is no grammar mistakes
        {
            File.AppendAllText(userLogPath, "No grammar mistake\n");
            correctSent = true;
        }
        if (grammarScore < 0)
        {
            grammarScore = 0;
        }
        scoreRound += grammarScore;
    }

    // Cross out the word(s) that is/are used correctly by the user
    public void CrossOutWord(string context)
    {
        string[] contextWords = context.Replace("'"," ").Split(' ');
        foreach (var contextWord in contextWords)
        {
            for (int i = 0; i < words.Count; i++)
            {
                //Debug.Log("word: " + words[i] + "contextWord: " + contextWord);
                if(words[i].Equals(contextWord))
                {
                    words[i] = StrikeThrough(words[i]);
                    wordsLeft--;
                    if (wordsLeft == 0)
                    { 
                        if(SceneManager.GetActiveScene().name == "T1")
                        {
                            finishMessageText.text = "Congratulations! You Finished MapChat Demo!";
                        }
                        else
                        {
                            finishMessageText.text = "YAY! You Finished This Round!";
                        }
                        finishBox.SetActive(true);
                        scoreTotal += scoreRound;
                        isRoundFinish = true;
                        isWin = true;
                        File.WriteAllText(userScorePath, scoreTotal.ToString());
                        scoreText2.text = "Total Score: " + scoreTotal.ToString("F0") + "\n" + "Round Score: " + scoreRound.ToString("F0");
                        scoreRound = 0;
                    }
                    
                }
            }
        }
        RefreshWordList();
    }

    // Update the wordList after the word(s) is/are crossed out
    public void RefreshWordList()
    {
        WordListContent.text = "";
        foreach (string word in words)
        {
            WordListContent.text = WordListContent.text + word + "\n";
        }
    }

    // visual of cross out word(s)
    public string StrikeThrough(string s)
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
        if (Input.GetKeyDown(KeyCode.R)) // Speech-to-Text Recording control
        {
            if (!recording)
            {
                exampleStreaming.StartRec();
                GrammarMessage.text = "";
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
        //playerContent.text = "I do prefer artisanal lattes and hope it tastes good.";
        //NPCContent.text = "I highly recommend getting the coconut latte, it is actually made with real coconut cream!";
        if (playerContent.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                File.AppendAllText(userLogPath, NPCContent.text + "\n" + playerContent.text + "\n");
                CheckGrammar(playerContent.text);
                NLU.AnalyzeText(playerContent.text);
                NLU.AnalyzeText(NPCContent.text);
                happybot = true;
            }
        }
        if (happybot)
        {
            if (NLU.sentenceValues.ContainsKey(playerContent.text) && NLU.sentenceValues.ContainsKey(NPCContent.text))
            {
                happybot = false;
                double? similar = CompareText(NLU.sentenceValues[playerContent.text], NLU.sentenceValues[NPCContent.text]);
                File.AppendAllText(userLogPath, similar+"\n\n");
                double temp = similar.Value*25.0;
                if (temp > 10)
                {
                    temp = 10f;
                }
                scoreRound += temp;
                scoreText.text = "Total Score: " + scoreTotal.ToString("F0") + "\n" + "Round Score: " + scoreRound.ToString("F0");
                if (similar > 0.2)
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
        if (Input.GetKeyDown(KeyCode.Q))   // Get next question control
        {
            NPCContent.text = GetDialogue(topic);
            playerContent.text = "";
            GrammarMessage.text = "";
            inputRelevanceText.text = "";
        }
    }

    // check if player at chat spot
    void OnTriggerEnter2D(Collider2D other)
    {
        inside = true;
        // save current colliding object name
        topic = other.gameObject.name;
    }

    // check if player leave chat spot
    private void OnTriggerExit2D(Collider2D collision)
    {
        inside = false;
        dialogBox.SetActive(false);
    }
    void OnTriggerEnter(Collider other)
    {
        inside = true;
        // save current colliding object name
        topic = other.gameObject.name;
    }

    // check if player leave chat spot
    private void OnTriggerExit(Collider collision)
    {
        inside = false;
        dialogBox.SetActive(false);
    }

    // Load dialogue content from file to a list
    void LoadQuestions(double score)
    {
        string jsonString;
        if (score < scoreMed)
        {
            jsonString = File.ReadAllText(Application.dataPath + "/Scripts/questions_easy.json");
        }
        else if (score < scoreHard)
        {
            jsonString = File.ReadAllText(Application.dataPath + "/Scripts/questions_medium.json");
        }
        else
        {
            jsonString = File.ReadAllText(Application.dataPath + "/Scripts/questions_hard.json");
        }
        
        QuestionsList temp = JsonUtility.FromJson<QuestionsList>(jsonString);
        questionList = new Dictionary<string, List<string>>();
        foreach (var questionPair in temp.list)
        {
            questionList.Add(questionPair.type, questionPair.questions);
        }
        questionLeft = new Dictionary<string, int>();
        foreach(string topic in questionList.Keys)
        {
            questionLeft.Add(topic, questionNum);
        }

    }

    // compare user input and question using NLU to give relevence score
    double? CompareText(ans a, ans q)
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
        return score;
    }
  
}
