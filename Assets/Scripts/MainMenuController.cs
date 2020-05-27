using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{

    GameObject startSetsPanel;
    GameObject mainPanel;

    public int widthMax;
    public int heightMax;

    // Start is called before the first frame update
    void Start()
    {
        startSetsPanel = GameObject.Find("StartSetsPanel");
        startSetsPanel.SetActive(false);
        mainPanel = GameObject.Find("MainPanel");
        mainPanel.SetActive(true);
        widthMax = Screen.width / 64 - 8;//??? maybe something more adaptive? fix camera maybe? try later
        heightMax = Screen.height / 64 - 4;//same
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnGameExit()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void OnGameStart()
    {
        startSetsPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void OnSetsBack()
    {
        startSetsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public void OnConfirm()
    {
		int width = 16;
		int height = 9;
		if(startSetsPanel.transform.Find("WidthInputField").gameObject.GetComponent<InputField>().text != ""){
			width = (int)System.Convert.ToUInt32(startSetsPanel.transform.Find("WidthInputField").gameObject.GetComponent<InputField>().text);//resize field to min/max if outbounds
		}
        if(startSetsPanel.transform.Find("HeightInputField").gameObject.GetComponent<InputField>().text != ""){
			height = (int)System.Convert.ToUInt32(startSetsPanel.transform.Find("HeightInputField").gameObject.GetComponent<InputField>().text);
		}
        if (width < 4)
        {
            width = 4;
        }
        if (width > widthMax)
        {
            width = widthMax;
        }
        if (height < 4)
        {
            height = 4;
        }
        if (height > heightMax)
        {
            height = heightMax;
        }
        PlayerPrefs.SetInt("width", width);
        PlayerPrefs.SetInt("height", height);
        SceneManager.LoadScene("MainScene");
    }
}
