using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class OnCollisionChangeScene : MonoBehaviour
{
    AudioSource levelAudio;
    Scene currentScene;
    string currentSceneName;

    // Start is called before the first frame update
    void Start()
    {
        levelAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        currentScene = SceneManager.GetActiveScene();
        currentSceneName = currentScene.name;

        //levelAudio.Play();
        NetworkServer.Reset();

        if (currentSceneName == "Level_0")
        {
            Debug.Log("Level 0 to Level 1");
            SceneManager.LoadScene("Level_1");
        }

        if (currentSceneName == "Level_1")
        {
            Debug.Log("Level 1 to Level 2");
            SceneManager.LoadScene("Level_2");
        }

        if (currentSceneName == "Level_2")
        {
            Debug.Log("Level 2 to Level 3");
            SceneManager.LoadScene("Level_3");
        }

        if (currentSceneName == "Level_3")
        {
            Debug.Log("Level 3 to Level 4");
            SceneManager.LoadScene("Level_4");
        }

        if (currentSceneName == "Level_4")
        {
            //NetworkManager.networkSceneName.
            Debug.Log("Level 4 to Level 5");
            SceneManager.LoadScene("Level_5");
        }

        if (currentSceneName == "Level_5")
        {
            Debug.Log("Level 5 to Level 6");
            SceneManager.LoadScene("Level_6");

        }
        if (currentSceneName == "Level_6")
        {
            if (!levelAudio.isPlaying)
            {
                levelAudio.Play();
            }
            Debug.Log("Level 6 - you made it!!");

        }

    }
}
