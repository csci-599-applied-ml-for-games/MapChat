using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerController : MonoBehaviour
{

    public GameObject NPCBox;
    private Rigidbody2D body;

    // Start is called before the first frame update
    public float units = 4.0f;
    void Start()
    {
        //QualitySettings.vSyncCount = 0;
        //Application.targetFrameRate = 10;
        body = GetComponent<Rigidbody2D>();
    }

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
        //Vector2 position = transform.position;

 

        // if player is not currently in chat
        if(!NPCBox.active){
            // units/sec
            // move x position
            //position.x = position.x + (units * horizontal * Time.deltaTime);
            // move y position
            //position.y = position.y + (units * vertical * Time.deltaTime);
            body.MovePosition(new Vector2((transform.position.x + units * horizontal * Time.deltaTime), (transform.position.y + units * vertical * Time.deltaTime)));
        }

        // update position on board
        //transform.position = position;
    }
}
