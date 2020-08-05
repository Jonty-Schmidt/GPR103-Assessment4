﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script must be used as the core Player script for managing the player character in the game.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    //=========================  Enums  =====================================//
    public enum DeathType { collision, drowning, offscreen };

    //=========================  Variables  =====================================//
                            public  string      playerName              = "";        //The players name for the purpose of storing the high score
   
                            public  int         playerTotalLives;                   //Players total possible lives.
    [System.NonSerialized]  public  int         playerLivesRemaining;               //PLayers actual lives remaining.

                            private bool        playerIsAlive           = true;     //Is the player currently alive?
                            private bool        playerCanMove           = false;    //Can the player currently move?

    [System.NonSerialized]  public  int         moveTimer               = 0;
                            public  int         moveDelay               = 50;
                            private Rigidbody2D log                     = null;

    [SerializeField]        private Sounds      sounds;
                            private AudioSource audioSrc;
    [SerializeField]        private Text        scoreText;
                            private int         score;

    //=========================  Function - Start()  =====================================//
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        audioSrc.loop = false;
        audioSrc.playOnAwake = false;

        scoreText.text = "Score: " + score;
    }

    //=========================  Function - Update()  =====================================//
    void Update()
    {
        if (playerIsAlive) {
            //-----------------  Movement  -------------------------------//
            Vector2 movement = GetMovementInput();

            if (movement != Vector2.zero)
                StartCoroutine(Move(movement));
            
            //-----------------  Move with log  -------------------------------//
            if (log != null) {
                transform.Translate(log.velocity * Time.deltaTime);
                CheckForOffscreen();
            }
        }
    }

    //=========================  Function - CurrentScore()  =====================================//
    void CurrentScore()
    {
        score++;
        scoreText.text = "Score: " + score;
    }

    //=========================  Function - GetMovementInput()  =====================================//
    Vector2 GetMovementInput()
    {
        Vector2 movement = Vector2.zero;
        moveTimer++;
        bool keyPressed = false;
        if (Input.GetKey(KeyCode.W))
        {
            keyPressed = true;
            if (moveTimer > moveDelay)
            {
                movement = (new Vector2(0, 1));
                moveTimer = 0;
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            keyPressed = true;
            if (moveTimer > moveDelay)
            {
                movement = (new Vector2(0, -1));
                moveTimer = 0;
            }
        }
        if (Input.GetKey(KeyCode.A))
        {
            keyPressed = true;
            if (moveTimer > moveDelay)
            {
                movement = (new Vector2(-1, 0));
                moveTimer = 0;
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            keyPressed = true;

            if (moveTimer > moveDelay)
            {
                movement = (new Vector2(1, 0));
                moveTimer = 0;
            }
        }
        if (!keyPressed)
            moveTimer = moveDelay;

        return movement;
    }

    //=========================  IEnumerator - Move()  =====================================//
    IEnumerator Move(Vector2 movement)
    {
        //-----------------  Spawn additional row?  -------------------------------//
        if (movement.y != 0)
            FindObjectOfType<GameManager>().PlayerJustMoved(Mathf.RoundToInt(transform.position.y + movement.y));
        else
            CheckForOffscreen();

        //-----------------  Audio  -------------------------------//
        if (!audioSrc.isPlaying) {
            audioSrc.clip = sounds.hop;
            audioSrc.Play();
        }

        //-----------------  Movement  -------------------------------//
        for (int i = 0; i < 5; i++) {
            yield return new WaitForSeconds(0.02f);
            if (playerIsAlive)
                transform.Translate(movement / 5);
        }
        
        //-----------------  Check for drowning  -------------------------------//
        if (playerIsAlive)
            CheckForDrowning();

        if (movement.y > 0)
        CurrentScore();
    }

    //=========================  IEnumerator - Respawn()  =====================================//
    IEnumerator Respawn()
    {
        transform.position = Vector3.zero;
        playerIsAlive = false;

        yield return new WaitForSeconds(0.5f);
        playerIsAlive = true;
    }

    //=========================  Function - CheckForDrowning()  =====================================//
    void CheckForDrowning()
    {
        FroggerRow row = GameObject.Find("Row " + Mathf.RoundToInt(transform.position.y)).GetComponent<FroggerRow>();

        if (row.type == FroggerRow.Type.WATER && log == null)
            Die(DeathType.drowning);
    }

    //=========================  Function - CheckForOffscreen()  =====================================//
    void CheckForOffscreen()
    {
        float leftBound = FindObjectOfType<GameManager>().levelConstraintLeft - 10;
        float rightBound = FindObjectOfType<GameManager>().levelConstraintRight + 10;

        if (transform.position.x < leftBound || transform.position.x > rightBound) {
            FroggerRow froggerRow = GameObject.Find("Row " + (int)(transform.position.y)).GetComponent<FroggerRow>();

            switch (froggerRow.type) {
                case FroggerRow.Type.ROAD:
                    Die(DeathType.collision);
                    break;

                case FroggerRow.Type.WATER:
                    Die(DeathType.drowning);
                    break;

                default:
                    Die(DeathType.collision);
                    break;
            }
        }
    }

    //=========================  Function - Die()  =====================================//
    public void Die(DeathType deathType)
    {
        //Debug.Log("We died of " + deathType + ", row: " + GameObject.Find("Row " + (int)(transform.position.y)).name);
        playerIsAlive = false;

        if (deathType == DeathType.collision)
            audioSrc.clip = sounds.deathCollision;
        else if (deathType == DeathType.drowning)
            audioSrc.clip = sounds.deathDrowning;
        else
            Debug.Log("DeathType: " + deathType);

        audioSrc.Play();

        StartCoroutine(Respawn());
    }

    //=========================  Struct - Sounds  =====================================//
    [System.Serializable]
    public struct Sounds {
        public AudioClip deathCollision;
        public AudioClip deathDrowning;
        public AudioClip hop;
    }
    
    //===========================  Trigger - OnTriggerEnter2D()  =====================================//
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Log")
            log = other.gameObject.GetComponent<Rigidbody2D>();
    }

    //===========================  Trigger - OnTriggerExit2D()  =====================================//
    void OnTriggerExit2D(Collider2D other)
    {
        log = null;
    }
}
