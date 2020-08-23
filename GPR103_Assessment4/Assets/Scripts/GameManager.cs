using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //====================================  Variables  ===========================================================================//
    [Header("Playable Area")]
    public int      width;
    public int      margin;
    public int      rowsTotal;
    int             rowsOnScreen;
    public float    camUpperY;

    [Header("Sprite Options")]
    public SpriteOptions    spriteOptions;
    public static int[]     logLengths = { 4, 5, 8 };
    
    [System.NonSerialized] public List<FroggerRow> rows = new List<FroggerRow>();

    [Header("Menus")]
    [SerializeField] GameObject menuStart;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuDeath;
    [SerializeField] GameObject menuFinished;
    [SerializeField] Text       highScoreTextName;
    [SerializeField] Text       highScoreTextScore;
    [SerializeField] InputField playerNameText;
    bool paused = false;

    [SerializeField] HighScore[] highScores;

    //====================================  Function - StartGame()  ========================================================================//
    public void StartGame()
    {
        if (playerNameText.text != "")
            StartCoroutine(CoRoutine_StartGame());
    }

    //====================================  Function - Update()  ========================================================================//
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            StartCoroutine(CoRoutine_Pause(paused == false));
    }

    //====================================  IEnumerator - CoRoutine_StartGame()  ========================================================================//
    IEnumerator CoRoutine_StartGame()
    {
        rowsOnScreen = Mathf.CeilToInt(FindObjectOfType<Camera>().orthographicSize) * 2 + 1;

        SpawnRow(0, FroggerRow.Type.SAFE, 1);
        for (int i = 0; i < rowsOnScreen; i++)
            CreateRow(i + 1);

        if (rowsTotal < rowsOnScreen)
            Debug.Log("Error: rowsTotal < rowsOnScreen");

        Player player = FindObjectOfType<Player>();
        player.Reset();
        
        Image[] images = menuStart.GetComponentsInChildren<Image>();
        Text[] textObjs = menuStart.GetComponentsInChildren<Text>();
        float timeElapsed = 0;
        float duration = 1.0f;
        while (timeElapsed <= duration) {
            yield return 0;
            timeElapsed += Time.deltaTime;

            foreach (Image image in images)
                image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - Time.deltaTime / duration);

            foreach (Text text in textObjs)
                text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - Time.deltaTime / duration);
        }

        player.playerIsActive = true;
            player.playerName = playerNameText.text;

        int minBotScore = rowsTotal * player.pointsPerStep;
        int maxBotScore = (player.playerTotalLives - 1) * player.pointsPerLivesRemaining + minBotScore;
        for (int i = 0; i < highScores.Length; i++)
            highScores[i].score = (int)(Mathf.Lerp(minBotScore, maxBotScore, 1 - (float)i / (float)highScores.Length)) + Random.Range(0, minBotScore / 10);
    }

    //====================================  Function - UnPause()  ========================================================================//
    public void UnPause()
    {
        StartCoroutine(CoRoutine_Pause(false));
    }

    //====================================  IEnumerator - CoRoutine_StartGame()  ========================================================================//
    IEnumerator CoRoutine_Pause(bool pauseNotUnPause)
    {
        Image[] images = menuPause.GetComponentsInChildren<Image>();
        Text[] textObjs = menuPause.GetComponentsInChildren<Text>();
        float timeElapsed = 0;
        float duration = 1.0f;

        Player player = FindObjectOfType<Player>();
        Vehicle[] vehicles = FindObjectsOfType<Vehicle>();
        Turtle[] turtles = FindObjectsOfType<Turtle>();

        if (pauseNotUnPause) {
            menuPause.SetActive(true);
            paused = true;

            foreach (Image image in images)
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

            foreach (Text text in textObjs)
                text.color = new Color(text.color.r, text.color.g, text.color.b, 0);

            player.enabled = false;
            foreach (Vehicle vehicle in vehicles)
                vehicle.enabled = false;
            foreach (FroggerRow froggerRow in rows)
                froggerRow.enabled = false;
            foreach (Turtle turtle in turtles)
                turtle.enabled = false;

            while (timeElapsed <= duration) {
                yield return 0;
                timeElapsed += Time.deltaTime;

                foreach (Image image in images)
                    image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + Time.deltaTime / duration);

                foreach (Text text in textObjs)
                    text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + Time.deltaTime / duration);
            }
        }
        else
        {
            paused = false;

            foreach (Image image in images)
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1);

            foreach (Text text in textObjs)
                text.color = new Color(text.color.r, text.color.g, text.color.b, 1);

            while (timeElapsed <= duration) {
                yield return 0;
                timeElapsed += Time.deltaTime;

                foreach (Image image in images)
                    image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - Time.deltaTime / duration);

                foreach (Text text in textObjs)
                    text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - Time.deltaTime / duration);
            }

            player.enabled = true;
            foreach (Vehicle vehicle in vehicles)
                vehicle.enabled = true;
            foreach (FroggerRow froggerRow in rows)
                froggerRow.enabled = true;
            foreach (Turtle turtle in turtles)
                turtle.enabled = true;

            menuPause.SetActive(false);
        }
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
            SpawnRow(i, (FroggerRow.Type)rand, Random.Range(2.0f, 7.0f) * Mathf.Sign(Random.Range(-1, 1)));
        }
        else
            SpawnRow(i, FroggerRow.Type.END, 0);
    }

    //====================================  Function - CreateRow()  =====================================================================//
    public void OpenDeathMenu()
    {
        StartCoroutine(CoRoutine_DeathMenu(true));
    }

    //====================================  Function - CreateRow()  =====================================================================//
    public void Restart()
    {
        StartCoroutine(CoRoutine_DeathMenu(false));
    }

    //====================================  IEnumerator - CoRoutine_DeathMenu()  =====================================================================//
    IEnumerator CoRoutine_DeathMenu(bool dieNotUnDie)
    {
        Image[] images = menuDeath.GetComponentsInChildren<Image>();
        Text[] textObjs = menuDeath.GetComponentsInChildren<Text>();
        float timeElapsed = 0;
        float duration = 1.0f;

        Player player = FindObjectOfType<Player>();
        Vehicle[] vehicles = FindObjectsOfType<Vehicle>();

        if (dieNotUnDie)
        {
            menuDeath.SetActive(true);

            foreach (Image image in images)
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

            foreach (Text text in textObjs)
                text.color = new Color(text.color.r, text.color.g, text.color.b, 0);

            player.enabled = false;

            foreach (Vehicle vehicle in vehicles)
                Destroy(vehicle.gameObject);
            foreach (FroggerRow froggerRow in rows)
                Destroy(froggerRow.gameObject);

            rows.Clear();
            
            while (timeElapsed <= duration) {
                yield return 0;
                timeElapsed += Time.deltaTime;

                foreach (Image image in images)
                    image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + Time.deltaTime / duration);

                foreach (Text text in textObjs)
                    text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + Time.deltaTime / duration);
            }
        }
        else
        {
            StartCoroutine(CoRoutine_StartGame());

            foreach (Image image in images)
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1);

            foreach (Text text in textObjs)
                text.color = new Color(text.color.r, text.color.g, text.color.b, 1);

            while (timeElapsed <= duration)
            {
                yield return 0;
                timeElapsed += Time.deltaTime;

                foreach (Image image in images)
                    image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - Time.deltaTime / duration);

                foreach (Text text in textObjs)
                    text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - Time.deltaTime / duration);
            }

            menuDeath.SetActive(false);
        }
    }
    
    //====================================  Function - SpawnRow()  =====================================================================//
    void SpawnRow(int row, FroggerRow.Type type, float velocity)
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

    //====================================  Function - SpawnRow()  =====================================================================//
    public void VictoryMenu()
    {
        StartCoroutine(CoRoutine_VictoryMenu(!menuFinished.activeSelf));
    }

    //====================================  IEnumerator - CoRoutine_VictoryMenu()  =====================================================================//
    IEnumerator CoRoutine_VictoryMenu(bool winNotReset)
    {
        Image[] images = menuFinished.GetComponentsInChildren<Image>();
        Text[] textObjs = menuFinished.GetComponentsInChildren<Text>();
        float timeElapsed = 0;
        float duration = 1.0f;

        Player player = FindObjectOfType<Player>();
        Vehicle[] vehicles = FindObjectsOfType<Vehicle>();

        InsertHighScore();

        if (winNotReset)
        {
            menuFinished.SetActive(true);

            foreach (Image image in images)
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

            foreach (Text text in textObjs)
                text.color = new Color(text.color.r, text.color.g, text.color.b, 0);

            player.enabled = false;

            foreach (Vehicle vehicle in vehicles)
                Destroy(vehicle.gameObject);
            foreach (FroggerRow froggerRow in rows)
                Destroy(froggerRow.gameObject);

            rows.Clear();

            while (timeElapsed <= duration) {
                yield return 0;
                timeElapsed += Time.deltaTime;

                foreach (Image image in images)
                    image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + Time.deltaTime / duration);

                foreach (Text text in textObjs)
                    text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + Time.deltaTime / duration);
            }
        }
        else {
            StartCoroutine(CoRoutine_StartGame());

            foreach (Image image in images)
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1);

            foreach (Text text in textObjs)
                text.color = new Color(text.color.r, text.color.g, text.color.b, 1);

            while (timeElapsed <= duration) {
                yield return 0;
                timeElapsed += Time.deltaTime;

                foreach (Image image in images)
                    image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - Time.deltaTime / duration);

                foreach (Text text in textObjs)
                    text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - Time.deltaTime / duration);
            }

            menuFinished.SetActive(false);
        }
    }

    //====================================  Function - InsertHighScore  =====================================================================//
    void InsertHighScore()
    {
        Player player = FindObjectOfType<Player>();

        int scoreRanking = highScores.Length;
        while (scoreRanking >= 0) {
            bool insert = false;

            if (scoreRanking == 0 || highScores[scoreRanking - 1].score > player.score)
                if (highScores[scoreRanking].score < player.score) {
                    highScores[scoreRanking].score = player.score;
                    highScores[scoreRanking].name = player.playerName;
                    for (int j = 0; j < playerNameText.characterLimit - player.playerName.Length; j++)
                        highScores[scoreRanking].name += " ";
                }

            if (!insert)
                scoreRanking--;
        }

        highScoreTextName.text = "";
        foreach (HighScore hS in highScores)
            highScoreTextName.text += "- " + hS.name + "\n";

        highScoreTextScore.text = "";
        foreach (HighScore hS in highScores)
            highScoreTextScore.text += hS.score + "\n";
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

    //====================================  Struct - HighScore  =====================================================================//
    [System.Serializable]
    struct HighScore
    {
        public string name;
        public int score;
    }
}