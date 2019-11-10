using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeFromBlack : MonoBehaviour
{
    public Image image;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(action());
    }

    IEnumerator action()
    {
        while (image.color.a > 0)
        {
            var color = image.color;
            color.a -= Time.deltaTime;
            image.color = color;
            yield return new WaitForEndOfFrame();
        }
    }
}
