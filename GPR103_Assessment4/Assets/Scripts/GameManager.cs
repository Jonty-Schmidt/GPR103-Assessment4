using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //====================================  Variables  ===========================================================================//
    [Header("Playable Area")]
    public int  width;
    public int  margin;
    public int  rowsTotal;
    int  rowsOnScreen;
    public float camUpperY;

    [Header("Gameplay Loop")]
    [System.NonSerialized]  public bool isGameRunning;      //Is the gameplay part of the game current active?
    [System.NonSerialized]  public float totalGameTime;     //The maximum amount of time or the total time avilable to the player.
    [System.NonSerialized]  public float gameTimeRemaining; //The current elapsed time

    [Header("Sprite Options")]
    public SpriteOptions    spriteOptions;
    public static int[] logLengths = { 4, 5, 8 };
    
    [System.NonSerialized] public List<FroggerRow> rows = new List<FroggerRow>();

    //====================================  Function - Start()  ========================================================================//
    void Start()
    {
        rowsOnScreen = Mathf.CeilToInt(FindObjectOfType<Camera>().orthographicSize) * 2 + 1;

        spawnRow(0, FroggerRow.Type.SAFE, 1);
        for (int i = 0; i < rowsOnScreen; i++)
             CreateRow(i + 1);

        if (rowsTotal < rowsOnScreen)
            Debug.Log("Error: rowsTotal < rowsOnScreen");
    }

    //====================================  Function - CreateRow()  =====================================================================//
    public void PlayerJustMoved(int yPos, int yDir)
    {
        int upperBound = yPos + rowsOnScreen / 2;
        int lowerBound = yPos - rowsOnScreen / 2 - 1;

        if (yDir == 1) {
            if (rows.Count <= rowsTotal) {
                if (rows.Count <= upperBound)
                    CreateRow(upperBound);
                else
                    rows[upperBound].EnableRow();
            }

            if (lowerBound > 0 && yPos - lowerBound > 0 && yPos <= camUpperY)
                for (int i = 0; i < lowerBound; i++)
                    rows[i].DisableRow();
        }
        else if (yDir == -1) {
            if (upperBound > rowsOnScreen)
                if (upperBound < rows.Count)
                    rows[upperBound + 1].DisableRow();

            if (lowerBound >= 0)
                rows[lowerBound].EnableRow();
        }
    }

    //====================================  Function - PlayerJustRespawnned()  =====================================================================//
    public void PlayerJustRespawned()
    {
        for (int i = 0; i < rows.Count; i++)
            if (i < rowsOnScreen)
                rows[i].gameObject.SetActive(true);
            else
                rows[i].gameObject.SetActive(false);
    }

    //====================================  Function - CreateRow()  =====================================================================//
    void CreateRow(int i)
    {
        if (i != rowsTotal) {
            int rand = Random.Range(0, 2);
            spawnRow(i, (FroggerRow.Type)rand, Random.Range(2.0f, 7.0f) * Mathf.Sign(Random.Range(-1, 1)));
        }
        else
            spawnRow(i, FroggerRow.Type.END, 0);
    }
    
    //====================================  Function - SpawnRow()  =====================================================================//
    void spawnRow(int row, FroggerRow.Type type, float velocity)
    {
        FroggerRow spawner;
        spawner = new GameObject().AddComponent<FroggerRow>();
        spawner.transform.parent = transform;
        spawner.name = "Row " + row;
        spawner.GetComponent<FroggerRow>().velocity = velocity;
        spawner.transform.position = new Vector2(0, row);
        spawner.GetComponent<FroggerRow>().type = type;
        rows.Add(spawner);
    }

    //====================================  Struct - SpriteOptions  =====================================================================//
    [System.Serializable]
    public struct SpriteOptions
    {
        public Sprite[] cars;
        public Sprite[] trucks;
        public Sprite[] logs;
        public Sprite   finish;

        public Sprite   croc;
        public Sprite   turtle;
        public Sprite   turtle_sub;
        public Sprite   frog;

        public Sprite   ground;
        public Sprite   water;
        public Sprite   safe;
        public Sprite   end;
    }
}