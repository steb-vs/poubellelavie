using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    public GameObject MenuPanel;

    // Start is called before the first frame update
    void Start()
    {
        MenuPanel.SetActive(true);
    }

    public void Play()
    {
        print("ON DOIT IMPLEMENTER LE PLAY BUTTON !");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
