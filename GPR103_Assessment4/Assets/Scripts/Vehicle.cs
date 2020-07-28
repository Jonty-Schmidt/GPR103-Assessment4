using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script must be utlised as the core component on the 'vehicle' obstacle in the frogger game.
/// </summary>
public class Vehicle : MonoBehaviour
{
    /*
    //===========================  Variables  =====================================//
    public int      moveDirection;      //This variabe is to be used to indicate the direction the vehicle is moving in.
    public float    speed;              //This variable is to be used to control the speed of the vehicle.
    public Vector2  startingPosition;   //This variable is to be used to indicate where on the map the vehicle starts (or spawns)
    public Vector2  endPosition;        //This variablle is to be used to indicate the final destination of the vehicle.

    //===========================  Function - Start()  =====================================//
    void Start()
    {

    }
    */
    
    //===========================  Function - Update()  =====================================//
    void Update()
    {
        if (Mathf.Abs(transform.position.x) > 20)
            Destroy(this.gameObject);

    }

    //===========================  Collision - OnCollisionEnter2D()  =====================================//
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
            //if (!GameObject.Find("Spawner " + (int)(collision.transform.position.y)).GetComponent<FroggerRow>().water)
                collision.gameObject.GetComponent<Player>().Die(Player.DeathType.collision);
    }
}