using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapTrigger : MonoBehaviour
{

	public GameObject MiniMap;
	bool isMiniMapOpen = true;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
        	if (!isMiniMapOpen)
        	{
                isMiniMapOpen = true;
        		MiniMap.SetActive(true);
        	}
            else
        	{
                isMiniMapOpen = false;
        		MiniMap.SetActive(false);
        	}
            
        }
    }
}
