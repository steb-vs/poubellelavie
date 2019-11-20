using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject container;

    private List<TextMeshProUGUI> _choices;
    private List<string> _choicesSave;

    public int index = 0;

    public Image fadeToBlackPanel;

    private IController<UIAction> _controller;

    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<IController<UIAction>>();

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
        float axis = -_controller.GetActionValue(UIAction.Vertical);

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

        if (_controller.GetActionUp(UIAction.Select) && fading == false)
        {
            switch (index)
            {
                case 0:
                    //SceneManager.LoadScene("Game");
                    break;
                case 1:
                    fading = true;
                    GameHelper.Settings.playerCount = 1;
                    StartCoroutine(fadeToBlack());
                    break;
                case 2:
                    fading = true;
                    GameHelper.Settings.playerCount = 2;
                    StartCoroutine(fadeToBlack());
                    break;
                case 3:
                    Application.Quit();
                    break;
            }
        }
    }

    private bool fading = false;

    IEnumerator fadeToBlack()
    {
        while (fadeToBlackPanel.color.a < 1)
        {
            var color = fadeToBlackPanel.color;
            color.a += Time.deltaTime;
            fadeToBlackPanel.color = color;
            yield return new WaitForEndOfFrame();
        }
        SceneManager.LoadScene("Game");
        yield return null;
    }

    private bool _tricked = false;
}