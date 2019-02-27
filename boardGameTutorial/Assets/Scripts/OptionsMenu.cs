using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour {

    public bool changedAutoRoll = false;
    public bool changedAnimationSpeed = false;
    public void setAutoRoll()
    {
        if (EventSystem.current.currentSelectedGameObject.name.ToString() == "YesButton")
        {
            PlayerPrefs.SetString("AutoRoll", "yes");
        }
        else if (EventSystem.current.currentSelectedGameObject.name.ToString() == "NoButton")
        {
            PlayerPrefs.SetString("AutoRoll", "no");
        }
        changedAutoRoll = true;
    }

    public void setAnimationSpeed()
    {
        switch (EventSystem.current.currentSelectedGameObject.name.ToString())
        {
            case "Speed1Button":
                PlayerPrefs.SetFloat("AnimationSpeed", 1f);
                break;

            case "Speed2Button":
                PlayerPrefs.SetFloat("AnimationSpeed", 2f);
                break;

            case "Speed4Button":
                PlayerPrefs.SetFloat("AnimationSpeed", 4f);
                break;
        }
        changedAnimationSpeed = true;
    }

    public void setLevel()
    {
        switch (EventSystem.current.currentSelectedGameObject.name.ToString())
        {
            case "EasyButton":
                PlayerPrefs.SetString("difficulty", "easy");
                break;
            case "MediumButton":
                PlayerPrefs.SetString("difficulty", "medium");
                break;
            case "HardButton":
                PlayerPrefs.SetString("difficulty", "hard");
                break;
        }
         
    }
}
