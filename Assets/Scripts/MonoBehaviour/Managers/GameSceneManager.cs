using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;


namespace ProjectCar
{
    namespace Managers
    {
        public sealed class GameSceneManager : MonoBehaviour
        {
            [SerializeField] private GameObject _loadCanvas;
            [SerializeField] private Image _progressBar;
            private float _targetFillAmount;

            public static GameSceneManager Instance { get; private set; }
            public PauseManager PauseManager => PauseManager.Instance;
            public bool IsPaused => PauseManager.Instance.IsPaused;
            public bool InMenu { get; private set; }
            public bool IsLoadingScene { get; private set; }
            public string CurrentScene { get; private set; }

            public async void LoadScene(string sceneName)
            {
                if (!IsLoadingScene)
                {
                    IsLoadingScene = true;

                    if (IsPaused)
                    {
                        PauseManager.Resume();
                    }

                    var scene = SceneManager.LoadSceneAsync(sceneName);

                    PrepareToLoad();

                    scene.allowSceneActivation = false;
                    _loadCanvas.SetActive(true);

                    do
                    {
                        await Task.Delay(300);  // clear this
                        _targetFillAmount = scene.progress;

                    } while (scene.progress < 0.9f);

                    await Task.Delay(1000);  // clear this

                    scene.allowSceneActivation = true;
                    InMenu = (sceneName == "Menu") ? true : false;

                    await Task.Delay(100);
                    _loadCanvas.SetActive(false);

                    IsLoadingScene = false;
                    CurrentScene = sceneName;
                }
            }

            public void Quit() => Application.Quit();

            private void Awake()
            {
                if (Instance == null)
                {
                    Instance = this;
                    IsLoadingScene = false;
                    DontDestroyOnLoad();
                }
                else if (Instance == this)
                {
                    Destroy(gameObject);
                }

                InMenu = (SceneManager.GetActiveScene().name == "Menu") ? true : false;
            }

            private void Update()
            {
                if (_progressBar == null) { return; }

                _progressBar.fillAmount = _targetFillAmount;
            }

            private void DontDestroyOnLoad()
            {
                DontDestroyOnLoad(gameObject);
                DontDestroyOnLoad(_loadCanvas);
            }

            private void PrepareToLoad()
            {
                ResetProgressBar();
                Camera.main.nearClipPlane = 20f;
            }

            private void ResetProgressBar()
            {
                if (_progressBar == null) { return; }

                _targetFillAmount = 0f;
                _progressBar.fillAmount = 0f;
            }
        }
    }
}
