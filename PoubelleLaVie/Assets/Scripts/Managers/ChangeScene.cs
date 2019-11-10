using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void LoadLevel()
    {
        SceneManager.LoadScene("IA_Bonjour");
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
