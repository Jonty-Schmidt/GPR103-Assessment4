using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FroggerRow : MonoBehaviour
{
    //===========================  Variables  =====================================//
    public enum Type { WATER, ROAD, SAFE, END}

    public float    velocity;
    float           delay = 1;
    float           timer = float.MaxValue;
    public Type     type;
    float           randDelay = 0;
    GameManager     gameManager;
    Transform       tileParent;
    float           spawnX;

    //===========================  Function - Start()  =====================================//
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        tileParent = new GameObject().transform;
        tileParent.name = "TileParent";
        tileParent.transform.parent = transform;
        spawnX = gameManager.width / 2 + gameManager.margin;

        for (int i = 0; i < gameManager.width; i++) {
            GameObject obj = new GameObject();
            obj.transform.position = new Vector3(this.transform.position.x - gameManager.width / 2 + i, this.transform.position.y, 5);
            obj.transform.parent = tileParent;
            obj.name = "Tile";

            Sprite sprite;
            switch (type) {
                case Type.ROAD:
                    sprite = gameManager.spriteOptions.ground;
                    break;

                case Type.SAFE:
                    sprite = gameManager.spriteOptions.safe;
                    break;

                case Type.WATER:
                    sprite = gameManager.spriteOptions.water;
                    break;

                case Type.END:
                    sprite = gameManager.spriteOptions.end;
                    break;

                default:
                    sprite = null;
                    break;
            }
            obj.AddComponent<SpriteRenderer>().sprite = sprite;
        }

        if (type == Type.END)
            for (int i = 0;i<5;i++)
                NewVehicle(new Vector2((i + 1) * spawnX / 3 - spawnX, transform.position.y)).name = "FinishSpot: " + i;
    }

    //===========================  Function - Update()  =====================================//
    void Update()
    {
        if (type != Type.SAFE && type != Type.END) {
            timer += Time.deltaTime;

            if (timer > delay + randDelay)
            {
                timer = 0;
                randDelay = Random.Range(-0.5f, 0.5f);
                Vector2 spawnPos = new Vector2(spawnX * -((velocity < 0)?-1:1), transform.position.y);
                Vehicle v = NewVehicle(spawnPos);

                if (type == Type.ROAD)
                {
                    v.name = "Car";
                    v.type = Vehicle.Type.CAR_ETC;

                    //-------------  Car - Chooses Sprite  -----------------------------------------------------//
                    int random = Random.Range(0, FindObjectOfType<GameManager>().spriteOptions.cars.Length);
                    v.GetComponent<SpriteRenderer>().sprite = FindObjectOfType<GameManager>().spriteOptions.cars[random];
                    v.GetComponent<BoxCollider2D>().size = new Vector2(1, 1);
                }
                else
                {
                    float randType = Random.Range(0, 3);
                    switch (randType)
                    {
                        case 0:
                            v.name = "Turtle";
                            v.type = Vehicle.Type.LOG_ETC;
                            v.GetComponent<SpriteRenderer>().sprite = gameManager.spriteOptions.turtle;
                            v.GetComponent<BoxCollider2D>().size = Vector3.one;
                            v.gameObject.AddComponent<Turtle>();
                            break;
                        case 1:
                            v.name = "Croc";
                            v.type = Vehicle.Type.LOG_ETC;
                            v.GetComponent<SpriteRenderer>().sprite = gameManager.spriteOptions.croc;
                            v.GetComponent<BoxCollider2D>().size = new Vector2(2, 1);
                            v.GetComponent<BoxCollider2D>().offset = new Vector2(-0.5f, 0);

                            Vehicle crocHead = new GameObject().AddComponent<Vehicle>();
                            crocHead.type = Vehicle.Type.CROC_HEAD;
                            crocHead.gameObject.AddComponent<BoxCollider2D>().size = Vector3.one;
                            crocHead.gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
                            crocHead.transform.position = v.transform.position + new Vector3(Mathf.Sign(velocity), 0, 0);
                            crocHead.transform.parent = v.transform;
                            crocHead.velocity = 0;
                            crocHead.name = "CrocHead";
                            break;
                        case 2:
                            v.name = "Log";
                            v.type = Vehicle.Type.LOG_ETC;
                            int rand = Random.Range(0, gameManager.spriteOptions.logs.Length);
                            v.GetComponent<SpriteRenderer>().sprite = gameManager.spriteOptions.logs[rand];
                            v.GetComponent<BoxCollider2D>().size = new Vector2(Vehicle.logLengths[rand], 1);
                            break;
                    }
                }
            }
        } 
    }

    //===========================  Function - NewVehicle()  =====================================//
    Vehicle NewVehicle(Vector2 spawnPos)
    {
        Vehicle vehicle = new GameObject().AddComponent<Vehicle>();
        vehicle.gameObject.AddComponent<BoxCollider2D>().isTrigger = true;
        vehicle.gameObject.AddComponent<SpriteRenderer>();
        vehicle.transform.position = spawnPos;
        vehicle.transform.parent = transform;
        vehicle.edgeBounds = spawnX;
        vehicle.velocity = velocity;

        if (velocity < 0)
            vehicle.transform.localScale = new Vector3(-1, 1, 1);

        return vehicle;
    }

    //===========================  Function - DisableRow()  =====================================//
    public void DisableRow()
    {
        this.gameObject.SetActive(false);
        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i) != tileParent)
                Destroy(transform.GetChild(i).gameObject);
    }

    //===========================  Function - EnableRow()  =====================================//
    public void EnableRow()
    {
        this.gameObject.SetActive(true);
    }
}
