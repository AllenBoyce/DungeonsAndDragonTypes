using UnityEngine;
using UnityEngine.UI;
public class QuitButton : MonoBehaviour
{
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        
        Application.Quit();
    }
    private Button _quitButton;
    void Start()
    {
        _quitButton = GetComponent<Button>();
        _quitButton.onClick.AddListener(QuitGame);
    }

    void OnDestroy() {
        _quitButton.onClick.RemoveListener(QuitGame);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
