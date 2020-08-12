using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSpot : MonoBehaviour
{
    public enum State { FILLED, EMPTY, FLY, CROC }
    public State state = 0;

    public float timer = 0;
    public float randDelay = 0;
    // Start is called before the first frame update
    void Start()
    {
        randDelay = Random.Range(2, 5);
        state = State.EMPTY;
    }

    // Update is called once per frame
    void Update()
    {

        timer += Time.deltaTime;

        if (timer > randDelay)
        {
            randDelay = Random.Range(2, 5);
            timer = 0;
            switch (state)
            {
                case State.EMPTY:
                    int rand = Random.Range(0, 2);
                    state = (State)((int)(State.FLY) * (rand == 0 ? 0 : 1) + (int)(State.CROC) * (rand == 1 ? 0 : 1));
                    randDelay = Random.Range(1, 5);
                    GetComponent<SpriteRenderer>().sprite = FindObjectOfType<GameManager>().spriteOptions.cars[0];
                    break;
                case State.FLY:
                    state = State.EMPTY;
                    randDelay = Random.Range(1, 5);
                    GetComponent<SpriteRenderer>().sprite = FindObjectOfType<GameManager>().spriteOptions.cars[2];
                    break;
                case State.CROC:
                    state = State.EMPTY;
                    randDelay = Random.Range(1, 5);
                    GetComponent<SpriteRenderer>().sprite = FindObjectOfType<GameManager>().spriteOptions.cars[1];
                    break;
                case State.FILLED:
                    randDelay = 1000;
                    GetComponent<SpriteRenderer>().sprite = FindObjectOfType<GameManager>().spriteOptions.finish;
                    break;
            }
        }
    }
}
