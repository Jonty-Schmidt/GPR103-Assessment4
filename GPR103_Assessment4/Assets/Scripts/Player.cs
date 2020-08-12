using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
    //=========================  Enums  =====================================//
    public enum DeathType { collision, drowning, offscreen, chompChompChomp, time };

    //=========================  Variables  =====================================//
    public  string      playerName              = "";       //The players name for the purpose of storing the high score
    
    [System.NonSerialized]
    public int          playerLivesRemaining    = 5;        //PLayers actual lives remaining.
    public  int         playerTotalLives        = 5;        //Players total possible lives.

    [SerializeField]
    private Text livesText;
    private List<Image> lifeIMGs;

    private bool        playerIsAlive           = true;     //Is the player currently alive?
    private bool        playerCanMove           = false;    //Can the player currently move?

    [System.NonSerialized]
    public Vehicle      log = null;

    [SerializeField]
    private Sounds      sounds;
    private AudioSource audioSrc;

    [SerializeField]
    private Text        scoreText;
    private int         score;
    private float       highestRow = 0;

    [SerializeField]
    private Image       timeImg;
    const float         timeInitial = 20;
    private float       timeLeft = timeInitial;
    private float       timeImgXpos;
    private float       timeImgXsize;

    private GameManager gameManager;

    [SerializeField]
    private bool        invincibleForTheSakeOfDebugging = false;

    //=========================  Function - Start()  =====================================//
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        audioSrc.loop = false;
        audioSrc.playOnAwake = false;
        gameManager = FindObjectOfType<GameManager>();

        scoreText.text = "Score: " + score;

        lifeIMGs = new List<Image>();

        for (int i = 0; i < playerTotalLives; i++)
        {
            GameObject imgObj = new GameObject();
            imgObj.transform.parent = livesText.transform.parent;
            imgObj.AddComponent<RectTransform>().localPosition = livesText.rectTransform.localPosition + new Vector3((i * 25), 5, 0);
            imgObj.AddComponent<Image>().sprite = gameManager.spriteOptions.frog;
            imgObj.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
            imgObj.name = "IMG - Life " + (i + 1);
            lifeIMGs.Add(imgObj.GetComponent<Image>());
        }

        timeImgXpos = timeImg.rectTransform.localPosition.x;
        timeImgXsize = timeImg.rectTransform.sizeDelta.x;
    }

    //=========================  Function - Update()  =====================================//
    void Update()
    {
        if (playerIsAlive) {
            UpdateTime();

            //-----------------  Movement  -------------------------------//
            Vector2 movement = GetMovementInput();

            if (movement != Vector2.zero)
                if (transform.position.y > 0.5f || movement.y >= 0)
                    StartCoroutine(Move(movement));
            
            //-----------------  Move with log  -------------------------------//
            if (log != null) {
                transform.Translate(log.velocity * Time.deltaTime, 0, 0);
                CheckForOffscreen();
            }
        }
    }

    //=========================  Function - UpdateTime()  =====================================//
    void UpdateTime()
    {
        timeLeft -= Time.deltaTime;

        timeImg.rectTransform.sizeDelta = new Vector2(timeImgXsize * timeLeft / timeInitial, timeImg.rectTransform.sizeDelta.y);
        timeImg.rectTransform.localPosition = new Vector3(timeImgXpos - (timeImgXsize - timeImg.rectTransform.sizeDelta.x) / 2, timeImg.rectTransform.localPosition.y, timeImg.rectTransform.localPosition.z);

        if (timeLeft <= 0)
            Die(DeathType.time);
    }

    //=========================  Function - UpdateScore()  =====================================//
    void UpdateScore()
    {
        scoreText.text = "Score: " + score;
    }

    //=========================  Function - GetMovementInput()  =====================================//
    Vector2 GetMovementInput()
    {
        Vector2 movement = Vector2.zero;

        if (Input.GetKeyDown (KeyCode.W))
            movement = (new Vector2(0, 1));
        if (Input.GetKeyDown (KeyCode.S))
            movement = (new Vector2(0, -1));
        if (Input.GetKeyDown (KeyCode.A))
            movement = (new Vector2(-1, 0));
        if (Input.GetKeyDown (KeyCode.D))
            movement = (new Vector2(1, 0));

        return movement;
    }

    //=========================  IEnumerator - Move()  =====================================//
    IEnumerator Move(Vector2 movement)
    {
        //-----------------  Spawn additional row?  -------------------------------//
        if (movement.y != 0)
            FindObjectOfType<GameManager>().PlayerJustMoved(Mathf.RoundToInt(transform.position.y + movement.y), (int)movement.y);
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
        if (playerIsAlive) {
            CheckForDrowning();
            CheckForEnd();

            if (transform.position.y > highestRow) {
                score += Mathf.RoundToInt(transform.position.y - highestRow) * 100 + UnityEngine.Random.Range (0, 25);
                highestRow = transform.position.y;
                UpdateScore();
            }
        }
    }

    //=========================  IEnumerator - CheckForEnd()  =====================================//
    void CheckForEnd()
    {
        EndSpot endSpot = GameObject.Find("Row " + Mathf.RoundToInt(transform.position.y)).GetComponent<EndSpot>();
        if (endSpot != null)
        {
            switch (endSpot.state)
            {
                case (int)EndSpot.State.FILLED:
                    break;
                case EndSpot.State.EMPTY:
                    score += 1000;
                    endSpot.state = EndSpot.State.FILLED;
                    break;
                case EndSpot.State.FLY:
                    score += 1500;
                    endSpot.state = EndSpot.State.FILLED;
                    break;
                case EndSpot.State.CROC:
                    Die(DeathType.chompChompChomp);
                    break;
            }
        }
    }

    //=========================  IEnumerator - Respawn()  =====================================//
    IEnumerator Respawn()
    {
        transform.position = Vector3.zero;
        playerIsAlive = false;

        yield return new WaitForSeconds(0.5f);
        playerIsAlive = true;
        highestRow = 0;

        gameManager.PlayerJustRespawned();
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
        if (Mathf.Abs(transform.position.x) > gameManager.width / 2 - 1) {
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
        if (!invincibleForTheSakeOfDebugging)
        {
            //Debug.Log("We died of " + deathType);
            playerIsAlive = false;
            playerLivesRemaining--;

            switch (deathType) {
                case DeathType.collision:
                    audioSrc.clip = sounds.deathCollision;
                    break;
                case DeathType.drowning:
                    audioSrc.clip = sounds.deathDrowning;
                    break;
                default:
                    audioSrc.clip = sounds.deathCollision;
                    break;
            }

            audioSrc.Play();

            if (playerLivesRemaining >= 0) {
                StartCoroutine(Respawn());
                lifeIMGs[playerLivesRemaining].gameObject.SetActive(false);
            }

        }
    }

    //=========================  Struct - Sounds  =====================================//
    [System.Serializable]
    public struct Sounds {
        public AudioClip deathCollision;
        public AudioClip deathDrowning;
        public AudioClip hop;
    }

    //===========================  Trigger - OnTriggerEnter2D()  =====================================//
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Vehicle>() != null && other.GetComponent<Vehicle>().type == Vehicle.Type.LOG_ETC)
            log = other.gameObject.GetComponent<Vehicle>();

        if (other.GetComponent<Vehicle>() != null && other.GetComponent<Vehicle>().type == Vehicle.Type.CROC_HEAD)
            Die(DeathType.chompChompChomp);
    }

    //===========================  Trigger - OnTriggerExit2D()  =====================================//
    void OnTriggerExit2D(Collider2D other)
    {
        if (log != null && other.gameObject == log.gameObject)
            log = null;
    }
}