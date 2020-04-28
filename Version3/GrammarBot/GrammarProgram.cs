using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GrammarBotClient;
using TMPro;

public class GrammarProgram : MonoBehaviour
{

    private GrammarBot grammarBot;
    
    // Start is called before the first frame update
    void Start()
    {
        grammarBot = new GrammarBotClient.GrammarBot(new GrammarBotClient.ApiConfig());
        Check();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    async void Check()
    {
        var grammar = await grammarBot.CheckAsync("I loves music.");
        if (grammar.Success)
        {
            foreach (var item in grammar.CheckContent.Matches)
            {
                Debug.Log(item.offset);
                Debug.Log(item.Message);
            }
        }
        else 
        {
            Debug.Log(grammar.Message);
        }
        
    }
}
