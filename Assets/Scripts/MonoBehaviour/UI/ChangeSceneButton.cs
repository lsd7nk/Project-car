using System.Collections;
using UnityEngine;
using ProjectCar.Managers;
using ProjectCar.Controllers;

namespace ProjectCar
{
    namespace UI
    {
        public sealed class ChangeSceneButton : MonoBehaviour
        {
            [SerializeField] private string _sceneName;
            private const float _delay = 3f;

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
                    GameSceneManager.LoadScene(_sceneName);
                }
            }

            public void LoadPreviousScene() => GameSceneManager.LoadScene(GameSceneManager.PreviousScene);

            public void ReloadScene() => GameSceneManager.LoadScene(GameSceneManager.CurrentScene);

            private IEnumerator LoadSceneRoutine()
            {
                if (_delay > 0)
                {
                    yield return new WaitForSecondsRealtime(_delay);
                }

                GameSceneManager.LoadScene(_sceneName);
                yield break;
            }
        }
    }
}
