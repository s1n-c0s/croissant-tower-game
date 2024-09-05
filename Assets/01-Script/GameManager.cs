using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetScene();
        }
    }

    public void ResetScene()
    {
        // Get the active scene and reload it
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
