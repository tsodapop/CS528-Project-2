using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionTeleport : MonoBehaviour
{
    [SerializeField]
    GameObject projectileShooter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //if the gameObject collides with other certain objects
    private void OnCollisionEnter(Collision collision)
    {
        //if collide with player, teleport player back to original
       if (collision.gameObject.name == "CAVE2-PlayerController")
        {
            collision.gameObject.transform.position = new Vector3(111, 9, 272);

           

            if (projectileShooter.tag != "grabbedObject")
            {
                projectileShooter.transform.position = new Vector3(111, 10, 274);

                projectileShooter.transform.rotation = new Quaternion(0, 0, 0, 0);

                Rigidbody currentRigidbody = projectileShooter.GetComponent<Rigidbody>();
                currentRigidbody.isKinematic = true;
                currentRigidbody.isKinematic = false;
            }
            Debug.Log(gameObject.name);
        }


    }
}
