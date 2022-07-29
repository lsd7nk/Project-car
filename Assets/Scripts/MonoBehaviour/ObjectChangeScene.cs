using UnityEngine;

public class ObjectChangeScene : MonoBehaviour
{
    [SerializeField] private string _sceneName;
    [SerializeField] private LayerMask _interactLayer;

    private void OnCollisionEnter(Collision collision)
    {
        if (_sceneName == null) { return; }

        if ((_interactLayer.value & (1 << collision.gameObject.layer)) != 0)
        {
            GameSceneManager.Instance.LoadScene(_sceneName);
        }
    }
}
