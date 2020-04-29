using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantMoving : MonoBehaviour
{
    [SerializeField]
    int max_bound = 90;

    Vector3 startPosition;
    Vector3 currentPosition;
    bool forward = true;
    int incr = 0;

    [SerializeField]
    AudioSource moveAudio;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = gameObject.transform.position;
        currentPosition = gameObject.transform.position;
        moveAudio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        incr += 1;
        if (incr % 10 == 0)
        {
            moveWall();
            incr = 0;
        }
    }

    void moveWall()
    {
        //move forward 
        if ((currentPosition.z - startPosition.z <= max_bound) && forward) 
        {
            currentPosition.z = currentPosition.z + 1;
            gameObject.transform.position = (currentPosition);
        }
        else
        {
            forward = false;
        }

        //move backward
        if ((currentPosition.z - startPosition.z <= max_bound +1) && !forward && (startPosition.z - currentPosition.z <= max_bound))
        {
            currentPosition.z = currentPosition.z - 1;
            gameObject.transform.position = (currentPosition);
        }
        else
        {
            forward = true;
        }

        if (!moveAudio.isPlaying)
        {
            moveAudio.Play();
        }
    }
}
