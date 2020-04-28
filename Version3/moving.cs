using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moving : MonoBehaviour
{
    public GameObject Player;
    public GameObject NPCBox;

    // Start is called before the first frame update
    public float units = 4.0f;

    // Update is called once per frame
    void Update()
    {
        // get axis input (keyboard left, right, up, down arrows)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // print to console when arrow pressed
        //if(horizontal != 0) Debug.Log(horizontal);
        //if(vertical != 0) Debug.Log(vertical);

        // grab position of player on grid (x,y)
        Vector3 position = transform.position;

        if (!NPCBox.active)
        {
            position.x = position.x + (units * horizontal * Time.deltaTime);
            // move y position
            position.z = position.z + (units * vertical * Time.deltaTime);
        }
        
        

        // update position on board
        transform.position = position;
    }
}
