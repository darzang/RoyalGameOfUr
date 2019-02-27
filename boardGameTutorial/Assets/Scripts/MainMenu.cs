using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
public class MainMenu : MonoBehaviour {

    public OptionsMenu optionMenu;


    public void Play()
    {

        if (EventSystem.current.currentSelectedGameObject.name.ToString() == "SinglePlayerButton")
        {
            PlayerPrefs.SetString("gameMode", "singlePlayer");
        }
        else if (EventSystem.current.currentSelectedGameObject.name.ToString() == "MultiplayerButton")
        {
            PlayerPrefs.SetString("gameMode", "multiPlayer");
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        if (optionMenu.changedAutoRoll == false)
        {
            PlayerPrefs.SetString("AutoRoll", "no");
        }

        if (optionMenu.changedAnimationSpeed == false)
        {
            PlayerPrefs.SetFloat("AnimationSpeed", 1f);
        }

    }


    public void QuitGame()
    {
        Application.Quit();
    }


}
