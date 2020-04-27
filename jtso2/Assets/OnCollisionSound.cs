using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionSound : MonoBehaviour
{
    new AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        audio = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.tag == "Bullet"))
        {
            //audio = gameObject.GetComponent<AudioSource>();
            audio.Play();
        }

     

    }
}
