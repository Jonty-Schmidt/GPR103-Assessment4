using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turtle : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Player player;
    GameManager gameManager;
    Vehicle vehicle;

    float timer;
    static float delay = 3.0f;

    private void Start()
    {
        vehicle = this.GetComponent<Vehicle>();
        gameManager = FindObjectOfType<GameManager>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();

        if (vehicle == null)
            Debug.Log("Vehicle == null");
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0) {
            if (vehicle.type == Vehicle.Type.LOG_ETC) {
                spriteRenderer.sprite = gameManager.spriteOptions.turtle_sub;
                vehicle.type = Vehicle.Type.TURTLE_SUBMERGED;
            }
            else if (vehicle.type == Vehicle.Type.TURTLE_SUBMERGED) {
                spriteRenderer.sprite = gameManager.spriteOptions.turtle;
                vehicle.type = Vehicle.Type.LOG_ETC;
            }

            timer = delay;

            if (player != null && player.log == vehicle)
                player.Die(Player.DeathType.drowning);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
            player = other.GetComponent<Player>();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Player>() != null)
            if (player.log != null && player.log == vehicle)
                player.log = null;
    }
}
