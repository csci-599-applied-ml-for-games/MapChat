using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    const int scoreMed = 30;
    const int scoreHard = 70;
    public void PlayGame()
    {
    	SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
  
    }

    public void PlayGameWithSelectedLevel(int level)
    {
    	string text = "";
    	if (level == 1)
    	{
        	text = "0";
    	}
    	else if (level == 2)
    	{
    		text = scoreMed.ToString();
    	}
    	else
    	{	
    		text = scoreHard.ToString();
            System.IO.File.WriteAllText(Application.dataPath + "/Scripts/userScore.txt", text);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
            return;
    	}

        System.IO.File.WriteAllText(Application.dataPath + "/Scripts/userScore.txt", text);
    	SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
  
    }
}
