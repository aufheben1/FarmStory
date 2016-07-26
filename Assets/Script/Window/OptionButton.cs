using UnityEngine;
using System.Collections;

public class OptionButton : MonoBehaviour {
    bool moving = false;
    bool open = false;
    public GameObject optionPanel;
    public GameObject optionBar;
    public GameObject[] optionButton;

	void OnButtonClick(){

        if (moving) return;
        
        if (!open)
        {
            StartCoroutine(OptionButtonOpen());
        }
        else
        {
            StartCoroutine(OptionButtonClose());
        }

        moving = true;
        open = !open;

	}

    IEnumerator OptionButtonOpen()
    {
        TweenRotation.Begin(gameObject, 0.2f, Quaternion.Euler(0, 0, 90));
        TweenPosition.Begin(optionPanel, 0.2f, new Vector3(-222, 100, 0));
        yield return new WaitForSeconds(0.2f);

        TweenPosition.Begin(gameObject, 0.5f, new Vector3 (-626, 0, -1));
        yield return new WaitForSeconds(0.1f);

        TweenScale.Begin(optionBar, 0.4f, new Vector3(576, 154, 0));
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(0.1f);
            optionButton[3 - i].SetActive(true);
            optionButton[3 - i].transform.localScale = Vector3.zero;
            TweenScale.Begin(optionButton[3 - i], 0.4f, Vector3.one).method = UITweener.Method.BounceIn;
        }

        TweenRotation.Begin(gameObject, 0.2f, Quaternion.Euler(0, 0, -90));
        yield return new WaitForSeconds(0.4f);
        for (int i = 0; i < 4; i++)
        {
            optionButton[i].SendMessage("SetmScale", SendMessageOptions.DontRequireReceiver);
        }
        moving = false;
    }

    IEnumerator OptionButtonClose()
    {
        TweenPosition.Begin(gameObject, 0.5f, Vector3.right * 111);
        TweenScale.Begin(optionBar, 0.4f, new Vector3(0,154, 0));
        
        for (int i = 0; i < 4; i++){
            TweenScale.Begin(optionButton[i], 0.3f, Vector3.zero).method = UITweener.Method.BounceIn;
            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(0.1f);

        TweenRotation.Begin(gameObject, 0.2f, Quaternion.Euler(0, 0, 0));
        TweenPosition.Begin(optionPanel, 0.2f, new Vector3(-222, 50, 0));
        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < 4; i++)
        {
            optionButton[i].SetActive(false);
        }

        moving = false;
    }

    void SetOption(string code)
    {
        if (!PlayerPrefs.HasKey(code))
            PlayerPrefs.SetInt(code, 0);
        else
        {
            PlayerPrefs.SetInt(code, (PlayerPrefs.GetInt(code) + 1) % 2);
        }
        PlayerPrefs.Save();

        if (code == "Sound")
        {
            GameObject.FindWithTag("SoundSource").GetComponent<AudioSource>().mute= !GameObject.FindWithTag("SoundSource").GetComponent<AudioSource>().mute;
        }
        
    }

    void OnSoundButtonClick()
    {
        SetOption("Sound");
        optionButton[0].SendMessage("SetStopsign", SendMessageOptions.DontRequireReceiver);
    }
    
    void OnEffectButtonClick()
    {
        SetOption("Effect");
        optionButton[1].SendMessage("SetStopsign", SendMessageOptions.DontRequireReceiver);
    }

    void OnVibrationButtonClick()
    {
        SetOption("Vibration");
        optionButton[2].SendMessage("SetStopsign", SendMessageOptions.DontRequireReceiver);
    }

    void OnCloseButtonClick()
    {
        if (Application.loadedLevelName == "Map")
            Application.Quit();
        else
        {
            GameObject.FindWithTag("GameController").SendMessage("PauseWindowOpen", SendMessageOptions.DontRequireReceiver);
        }
    }
}