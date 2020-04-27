using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : CAVE2Interactable
{
    [SerializeField]
    public Transform playerTransform;

    Vector3 direction ;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        direction.z = playerTransform.eulerAngles.y - 180;
        direction.x = playerTransform.eulerAngles.x + 50;
        transform.localEulerAngles = direction;
    }
}
