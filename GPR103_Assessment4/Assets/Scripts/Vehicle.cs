using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script must be utlised as the core component on the 'vehicle' obstacle in the frogger game.
/// </summary>
public class Vehicle : MonoBehaviour
{
    public enum Type { CAR_ETC, LOG_ETC, TURTLE_SUBMERGED, CROC_HEAD};
    public Type type;
    public float edgeBounds;
    public float velocity;
    public static int[] logLengths = { 4, 5, 8};

    //===========================  Function - Update()  =====================================//
    void Update()
    {
        transform.position += new Vector3 (velocity * Time.deltaTime, 0, 0);

        if (Mathf.Abs(transform.position.x) > edgeBounds)
        {
            if (type != Type.CROC_HEAD)
                Destroy(this.gameObject);
        }
    }

    //===========================  Collision - OnCollisionEnter2D()  =====================================//
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Player>() != null && type == Type.CAR_ETC)
            other.gameObject.GetComponent<Player>().Die(Player.DeathType.collision);
    }
}