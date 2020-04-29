using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetInitialPosition : MonoBehaviour
{

    [SerializeField]
    Vector3 playerPosition;



    // Start is called before the first frame update
    void Start()
    {
        playerPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
