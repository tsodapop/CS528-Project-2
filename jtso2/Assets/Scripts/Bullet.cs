using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : CAVE2Interactable
{

    [SerializeField]
    public float speed = 10f;
    public float duration = 10f;

    private float lifetime;

    // Start is called before the first frame update
    void Start()
    {
        lifetime = duration;
    }

    // Update is called once per frame
    void Update()
    {
        //update and move bullet
        transform.position += transform.forward * speed * Time.deltaTime;
        
        //update lifetime and destroy if ncessary
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
