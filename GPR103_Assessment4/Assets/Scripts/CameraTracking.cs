using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    Transform   player;
    float       xConstraint;
    float       yConstraintLower;
    float       yConstraintUpper;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>().transform;

        GameManager gameManager = FindObjectOfType<GameManager>();
        xConstraint = gameManager.width / 2 - 16;
        yConstraintLower = this.GetComponent<Camera>().orthographicSize - 0.5f;
        yConstraintUpper = gameManager.rowsTotal - this.GetComponent<Camera>().orthographicSize + 2;

        gameManager.camUpperY = yConstraintUpper;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos;
        pos.x = Mathf.Clamp(player.transform.position.x, -xConstraint, xConstraint);
        pos.y = Mathf.Clamp(player.transform.position.y, yConstraintLower, yConstraintUpper);
        pos.z = transform.position.z;

        transform.position = pos;
    }
}
