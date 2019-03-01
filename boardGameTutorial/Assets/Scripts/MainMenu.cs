using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour {

    public OptionsMenu optionMenu;

    public void Play () {

        PlayerPrefs.SetString ("difficulty", "easy");
        if (EventSystem.current.currentSelectedGameObject.name.ToString () == "SinglePlayerButton") {
            PlayerPrefs.SetString ("gameMode", "singlePlayer");
            Debug.Log ("Game mode is: " + PlayerPrefs.GetString ("gameMode"));

        } else if (EventSystem.current.currentSelectedGameObject.name.ToString () == "MultiplayerButton") {
            PlayerPrefs.SetString ("gameMode", "multiPlayer");
            Debug.Log ("Game mode is: " + PlayerPrefs.GetString ("gameMode"));

        } else {
            Debug.Log ("Nothing clicked ?!");
        }

        if (optionMenu.changedAutoRoll == false) {
            PlayerPrefs.SetString ("AutoRoll", "no");
        }

        if (optionMenu.changedAnimationSpeed == false) {
            PlayerPrefs.SetFloat ("AnimationSpeed", 1f);
        }
        PlayerPrefs.Save ();
        SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex + 1);

    }

    public void QuitGame () {
        Application.Quit ();
    }

}