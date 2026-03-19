using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuScene : MonoBehaviour
{
    // Start is called before the first frame update

    public Button PlayButton,
        m_YourFirstButton,
        OptionsButton,
        QuitButton,
        m_YourSecondButton,
        m_YourThirdButton;

    void Start()
    {
        Debug.Log("You have clicked the button!");

        //Calls the TaskOnClick/TaskWithParameters/ButtonClicked method when you click the Button
        PlayButton.onClick.AddListener(ReturnToPlay);
        OptionsButton.onClick.AddListener(TaskOnClick);
        QuitButton.onClick.AddListener(TaskOnClickV2);
        // m_YourSecondButton.onClick.AddListener(
        //     delegate
        //     {
        //         TaskWithParameters("Hello");
        //     }
        // );
        //m_YourThirdButton.onClick.AddListener(() => ButtonClicked(42));
        m_YourThirdButton.onClick.AddListener(TaskOnClick);
    }

    void ChangeScene()
    {
        const string sceneToLoad = "Assets/Scenes/SampleScene.unity";
        var op = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);
        op.completed += (AsyncOperation obj) =>
        {
            Scene loadedScene = SceneManager.GetSceneByPath("Assets/Scenes/SampleScene.unity");
            //Debug.Log($"{sceneToLoad} finished loading (build index: {loadedScene.buildIndex}).");
            Debug.Log($"It has {loadedScene.rootCount} root(s).");
            //Debug.Log($"There are now {SceneManager.loadedSceneCount} Scenes open.");
        };
    }

    void ReturnToPlay()
    {
        //const string sceneToLoad = "Assets/Scenes/Menu.unity";

        //Output this to console when Button1 or Button3 is clicked
        Debug.Log("You have clicked the button!");

        ChangeScene();
    }

    void TaskOnClick()
    {
        //Output this to console when Button1 or Button3 is clicked
        Debug.Log("You have clicked the button!");
    }

    void TaskOnClickV2()
    {
        //Output this to console when Button1 or Button3 is clicked
        Debug.Log("this is a test for quit button");
    }
}
