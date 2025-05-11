using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
public class MenuNavigationButton : MonoBehaviour
{
    private Button _button;
    [SerializeField] private string _sceneName;

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(NavigateToScene);
    }

    private void NavigateToScene()
    {
        SceneUtility.LoadScene(_sceneName);
    }
}
