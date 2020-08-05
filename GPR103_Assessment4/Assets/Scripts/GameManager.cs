using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //====================================  Variables  ===========================================================================//
    [Header("Scoring")]
    public int currentScore = 0;                            //The current score in this round.
    public int highScore = 0;                               //The highest score achieved either in this session or over the lifetime of the game.

    [Header("Playable Area")]
    public float levelConstraintTop;                        //The maximum positive Y value of the playable space.
    public float levelConstraintBottom;                     //The maximum negative Y value of the playable space.
    public float levelConstraintLeft;                       //The maximum negative X value of the playable space.
    public float levelConstraintRight;                      //The maximum positive X value of the playablle space.

    //[Header("Gameplay Loop")]
    [System.NonSerialized]  public bool isGameRunning;      //Is the gameplay part of the game current active?
    [System.NonSerialized]  public float totalGameTime;     //The maximum amount of time or the total time avilable to the player.
    [System.NonSerialized]  public float gameTimeRemaining; //The current elapsed time
    public GameObject       Spawner;
    const int               rowsOnScreen = 10;

    [Header("Sprite Options")]
    public SpriteOptions    spriteOptions;
    
    [System.NonSerialized] public List<GameObject> rows = new List<GameObject>();

    //====================================  Function - Start()  ========================================================================//
    void Start()
    {
        FroggerRow safespawner;
        safespawner = Instantiate(Spawner).GetComponent<FroggerRow>();
        safespawner.transform.parent = transform;
        safespawner.name = "Row " + 0;
        safespawner.transform.position = new Vector2(0, 0);
        safespawner.rowIndex = 0;
        safespawner.type = FroggerRow.Type.SAFE;
        rows.Add(safespawner.gameObject);

        for (int i = 0; i < rowsOnScreen; i++)
            CreateRow(i + 1);
    }

    //====================================  Function - CreateRow()  =====================================================================//
    public void PlayerJustMoved(int yPos)
    {
        int upperBound = yPos + 10;
        int lowerBound = yPos - 10;

        if (rows.Count <= upperBound)
            CreateRow(upperBound);
    }

    //====================================  Function - CreateRow()  =====================================================================//
    void CreateRow(int i)
    {
        FroggerRow spawner;
        spawner = Instantiate(Spawner).GetComponent<FroggerRow>();
        spawner.transform.parent = transform;
        spawner.name = "Row " + i;
        spawner.moveDirection = (int)(((int)(Random.Range(0, 2)) - 0.5) * 2);
        spawner.speed = Random.Range(2, 7);
        spawner.transform.position = new Vector2(0, i);
        spawner.SpawnPosition.y = i;
        spawner.rowIndex = i;

        if (Random.Range(0, 2) == 0)
            spawner.GetComponent<FroggerRow>().type = FroggerRow.Type.ROAD;
        else
            spawner.GetComponent<FroggerRow>().type = FroggerRow.Type.WATER;

        rows.Add(spawner.gameObject);
    }

    //====================================  Struct - SpriteOptions  =====================================================================//
    [System.Serializable]
    public struct SpriteOptions
    {
        public Sprite[] cars;
        public Sprite[] trucks;
        public Sprite   log4;
        public Sprite   log5;
        public Sprite   log8;

        public Sprite   ground;
        public Sprite   water;
        public Sprite   safe;
    }
}
