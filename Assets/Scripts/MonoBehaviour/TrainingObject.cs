using System.Collections;
using UnityEngine;

public sealed class TrainingObject : MonoBehaviour
{
    [SerializeField] private GameObject _trainingCanvas;
    [SerializeField] private TrainingObjConfig _config;
    [SerializeField][Range(3f, 10f)] private float _trainingDisplayTime;
    private DisplayTextUpdater _descriptionText;

    public bool IsTriggerPassed { get; private set; }
    [field: SerializeField] public bool IsCalledFromAnotherScript { get; private set; }

    public void TrainingDescriptionDisplay()
    {
        if (!IsTriggerPassed && !_trainingCanvas.activeInHierarchy)
        {
            StartCoroutine(TrainingDescriptionDisplayRoutine());

            IsTriggerPassed = true;
        }

    }

    private void Start() => Initialize();

    private void OnTriggerEnter(Collider other)
    {
        if (_trainingCanvas == null) { return; }

        if (!IsCalledFromAnotherScript)
        {
            if (_config != null)
            {
                TrainingDescriptionDisplay();
            }
        }
    }

    private IEnumerator TrainingDescriptionDisplayRoutine()
    {
        _trainingCanvas.SetActive(true);
        _descriptionText?.SetText($"{_config.Name}\n\n{_config.Description}");

        yield return new WaitForSecondsRealtime(_trainingDisplayTime);

        _trainingCanvas.SetActive(false);

        yield break;
    }

    private void Initialize()
    {
        if (_trainingCanvas == null) { return; }

        IsTriggerPassed = false;
        _descriptionText = _trainingCanvas.GetComponentInChildren<DisplayTextUpdater>();

        _descriptionText?.Initialize();
    }
}
