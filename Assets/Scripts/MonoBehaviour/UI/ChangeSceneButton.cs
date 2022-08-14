using System.Collections;
using UnityEngine;
using ProjectCar.Managers;

namespace ProjectCar
{
    namespace UI
    {
        public sealed class ChangeSceneButton : MonoBehaviour
        {
            [SerializeField] private string _sceneName;
            private const float _delay = 4f;

            public GameSceneManager GameSceneManager => GameSceneManager.Instance;

            public void ChangeSceneWithDelay()
            {
                if (_sceneName == null) { return; }

                if (!GameSceneManager.IsLoadingScene)
                {
                    StartCoroutine(LoadSceneRoutine());
                }
            }

            public void ChangeScene()
            {
                if (_sceneName == null) { return; }

                if (!GameSceneManager.IsLoadingScene)
                {
                    LoadScene();
                }
            }

            private IEnumerator LoadSceneRoutine()
            {
                if (_delay > 0)
                {
                    yield return new WaitForSecondsRealtime(_delay);
                }

                GameSceneManager.LoadScene(_sceneName);
                yield break;
            }

            private void LoadScene() => GameSceneManager.LoadScene(_sceneName);
        }
    }
}
