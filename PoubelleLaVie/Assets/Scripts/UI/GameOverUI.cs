using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public GameObject container;

    private List<TextMeshProUGUI> _choices;
    private List<string> _choicesSave;

    [HideInInspector]
    public int index = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        _choices = new List<TextMeshProUGUI>();
        _choicesSave = new List<string>();
        for (int i = 0; i < container.transform.childCount; ++i)
        {
            _choices.Add(container.transform.GetChild(i).GetComponent<TextMeshProUGUI>());
            _choicesSave.Add(_choices[i].text);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float axis = Input.GetAxis(InputHelper.HORIZONTAL);
        
        if (axis > 0.1f && _tricked == false)
        {
            _tricked = true;
            _choices[index].text = $"{_choicesSave[index]}";
            ++index;
            if (index >= _choices.Count)
                index = 0;
        } 
        else if (axis < -0.1f && _tricked == false)
        {
            _tricked = true;
            _choices[index].text = $"{_choicesSave[index]}";
            --index;
            if (index < 0)
                index = _choices.Count - 1;
        }
        else if (_tricked == true && (axis < 0.1f && axis > -0.1f))
        {
            _tricked = false;
        }

        _choices[index].text = $"> {_choicesSave[index]}";
        
        if (Input.GetButtonUp(InputHelper.TAKE_N_DROP))
        {
            switch (index)
            {
                case 0:
                    SceneManager.LoadScene("IA_Bonjour");
                    break;
                case 1:
                    SceneManager.LoadScene("MainMenu");
                    break;
            }
        }
    }

    private bool _tricked = false;
}
