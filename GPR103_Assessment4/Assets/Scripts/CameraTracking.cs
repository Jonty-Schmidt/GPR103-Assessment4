using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracking : MonoBehaviour
{
    Transform   player;
    float       leftConstraint;
    float       rightConstraint;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>().transform;
        leftConstraint = FindObjectOfType<GameManager>().levelConstraintLeft + 4;
        rightConstraint = FindObjectOfType<GameManager>().levelConstraintRight - 4;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos;
        pos.x = Mathf.Clamp(player.transform.position.x, leftConstraint, rightConstraint);
        pos.y = player.transform.position.y;
        pos.z = transform.position.z;

        transform.position = pos;
    }
}
