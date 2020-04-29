using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionParticles : MonoBehaviour
{
    [SerializeField]
    ParticleSystem particle1;

    [SerializeField]
    ParticleSystem particle2;

    [SerializeField]
    ParticleSystem particle3;

    [SerializeField]
    ParticleSystem particle4;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!particle1.isPlaying)
        {
            particle1.Play();
        }

        if (!particle2.isPlaying)
        {
            particle2.Play();
        }

        if (!particle3.isPlaying)
        {
            particle3.Play();
        }

        if (!particle4.isPlaying)
        {
            particle4.Play();
        }

    }
}
