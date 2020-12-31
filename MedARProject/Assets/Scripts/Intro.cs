//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Responsible for intro behaviour
/// </summary>
public class Intro : MonoBehaviour
{
    [SerializeField]
    private TMP_Text counter;

    private void Start()
    {
        StartCoroutine(coroIntro());
    }

    //Dummy
    private IEnumerator coroIntro()
    {
        for (int i = 5; i > 0; i--)
        {
            counter.text = i + "...";
            yield return new WaitForSeconds(1);
        }

        endIntro();
    }

    private void endIntro()
    {
        StateManager.Instance.enterNextState();
    }
}
