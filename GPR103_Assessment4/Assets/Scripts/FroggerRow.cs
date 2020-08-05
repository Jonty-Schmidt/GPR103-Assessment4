using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FroggerRow : MonoBehaviour
{
    //===========================  Variables  =====================================//
    public enum Type { WATER, ROAD, SAFE}

    public GameObject   PrefabLand;
    public GameObject   PrefabWater;
    public int          iterator = 0;
    public Vector2      SpawnPosition;
    public int          moveDirection;
    public float        speed;
    public float        delay = 4;
    float               timer = float.MaxValue;
    public Type         type;
    public int          rowIndex;
    float               randDelay = 0;
    const float         Width = 40f;
    const float         Height = 15f;

    //===========================  Function - Start()  =====================================//
    void Start()
    {
        for (int i = 0; i < Width; i++) {
            GameObject obj = new GameObject();
            obj.transform.position = new Vector3(this.transform.position.x - Width / 2 + i, this.transform.position.y, 5);
            obj.transform.parent = this.transform;

            Sprite sprite;
            switch (type) {
                case Type.ROAD:
                    sprite = FindObjectOfType<GameManager>().spriteOptions.ground;
                    break;

                case Type.SAFE:
                    sprite = FindObjectOfType<GameManager>().spriteOptions.safe;
                    break;

                case Type.WATER:
                    sprite = FindObjectOfType<GameManager>().spriteOptions.water;
                    break;

                default:
                    sprite = null;
                    break;
            }
            obj.AddComponent<SpriteRenderer>().sprite = sprite;
        }
    }

    //===========================  Function - Update()  =====================================//
    void Update()
    {
        if (type != Type.SAFE) {
            timer += Time.deltaTime;

            if (timer > delay + randDelay)
            {
                timer = 0;
                randDelay = Random.Range(-0.5f, 0.5f);
                Rigidbody2D rb = null;

                SpawnPosition = new Vector2(Width / 2 * -Mathf.Sign(moveDirection), SpawnPosition.y);

                //-------------  Car  -----------------------------------------------------------------------------------//
                if (type == Type.ROAD) {
                    rb = Instantiate(PrefabLand, SpawnPosition, Quaternion.identity).GetComponent<Rigidbody2D>();
                    rb.name = "Vehicle: " + rowIndex + ", #" + iterator;
                    if (Mathf.Sign(moveDirection) == -1)
                        rb.GetComponent<Transform>().Rotate(Vector3.forward, 180);

                    //-------------  Car - Chooses Sprite  -----------------------------------------------------//
                    int random = Random.Range(0, FindObjectOfType<GameManager>().spriteOptions.cars.Length);
                    rb.GetComponent<SpriteRenderer>().sprite = FindObjectOfType<GameManager>().spriteOptions.cars[random];
                }
                //-------------  Log  -----------------------------------------------------------------------------------//
                else if (type == Type.WATER) {
                    rb = Instantiate(PrefabWater, SpawnPosition, transform.rotation).GetComponent<Rigidbody2D>();
                    rb.name = "Log: " + rowIndex + ", #" + iterator;
                    rb.tag = "Log";

                    //-------------  Log - Chooses Sprite  -----------------------------------------------------//
                    int random = Random.Range(0, 3);
                    switch(random) {
                        case 0:
                            rb.GetComponent<SpriteRenderer>().sprite = FindObjectOfType<GameManager>().spriteOptions.log4;
                            rb.GetComponent<BoxCollider2D>().size = new Vector2(4, 1);
                            break;
                        case 1:
                            rb.GetComponent<SpriteRenderer>().sprite = FindObjectOfType<GameManager>().spriteOptions.log5;
                            rb.GetComponent<BoxCollider2D>().size = new Vector2(5, 1);
                            break;
                        case 2:
                            rb.GetComponent<SpriteRenderer>().sprite = FindObjectOfType<GameManager>().spriteOptions.log8;
                            rb.GetComponent<BoxCollider2D>().size = new Vector2(8, 1);
                            break;
                    }
                }

                rb.transform.parent = transform;
                rb.velocity = new Vector2(moveDirection * speed, 0);
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        } 
    }
}
