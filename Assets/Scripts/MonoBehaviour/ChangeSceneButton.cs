using UnityEngine;

public sealed class ChangeSceneButton : MonoBehaviour
{
    [SerializeField] private string _sceneName;

    public void ChangeScene()
    {
        if (_sceneName == null) { return; }

        GameSceneManager.Instance.LoadScene(_sceneName);
    }
}
