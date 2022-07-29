using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IPauseHandler
{
    public void SetPause(bool isPaused);
}

public sealed class PauseManager : MonoBehaviour, IPauseHandler
{
    [SerializeField] private GameObject _pauseCanvas;
    private PauseMenuControls _controls;
    private List<IPauseHandler> _handlers;

    public static PauseManager Instance { get; private set; }
    public bool IsPaused { get; private set; }
    public bool InMenu => GameSceneManager.Instance.InMenu;

    public void RegisterHandler(IPauseHandler handler) => _handlers.Add(handler);

    public void UnRegisterHandler(IPauseHandler handler) => _handlers.Remove(handler);

    public void SetPause(bool isPaused = true)
    {
        foreach (IPauseHandler handler in _handlers)
        {
            handler.SetPause(IsPaused);
        }

        if (_pauseCanvas != null)
        {
            _pauseCanvas.SetActive(IsPaused);
        }
    }

    public void Resume() => ChangePauseState();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            IsPaused = false;
            _handlers = new List<IPauseHandler>();

            ControlsInitialize();
            DontDestroyOnLoad();
        }
        else if (Instance == this)
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable() => _controls?.Enable();

    private void OnDisable() => _controls?.Disable();

    private void ControlsInitialize()
    {
        _controls = new PauseMenuControls();
        _controls.PauseMenu.OpenPauseMenu.canceled += _ => ChangePauseState();
    }

    private void ChangePauseState()
    {
        if (InMenu) { return; }

        IsPaused = !IsPaused;
        SetPause();
    }

    private void DontDestroyOnLoad()
    {
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(_pauseCanvas);
    }
}
