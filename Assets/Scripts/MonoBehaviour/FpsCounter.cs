using System.Collections;
using UnityEngine;

public class FpsCounter : MonoBehaviour
{
    [SerializeField] private DisplayTextUpdater _fpsText;
    public int FPS { get; private set; }

    private void Start()
    {
        _fpsText.Initialize();
        StartCoroutine(FpsDisplayUpdateRoutine());
    }

    private void Update()
    {
        FPS = (int) (1f / Time.unscaledDeltaTime);
    }

    private IEnumerator FpsDisplayUpdateRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        _fpsText?.SetText($"{FPS} fps");
        yield return StartCoroutine(FpsDisplayUpdateRoutine());
        yield break;
    }
}
