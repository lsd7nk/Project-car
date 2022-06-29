using System.Collections;
using UnityEngine;

public class FallHandler : Controller
{
    [SerializeField] private LayerMask _interactLayers;
    [SerializeField] private Animator _faderAnimator;
    [SerializeField] private Transform _startTransform;
    private LapsInteractor _lapsInteractor;
    private CarController _carController;
    private Transform _carTransform;
    private Rigidbody _carRigidbody;
    private const string FadeIn = "FadeIn";

    public void Initialize()
    {
        _lapsInteractor = base.Initialize<LapsInteractor>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((_interactLayers.value & (1 << other.gameObject.layer)) != 0)
        {
            StartCoroutine(HandleFall(other.transform.parent.gameObject));
        }
    }

    private IEnumerator HandleFall(GameObject obj)
    {
        FadeInScreen();
        yield return new WaitForSeconds(1.5f);
        GetBackOnTheRoad(obj);
        yield return new WaitForSeconds(1f);
        FadeOutScreen();
        yield break;
    }

    private void GetBackOnTheRoad(GameObject obj)
    {
        if (Initialize(obj))
        {
            _carController?.ApplyBreaking(10000f);

            if (_carTransform != null)
            {
                _carTransform.position = _startTransform.position;
                _carTransform.rotation = _startTransform.rotation;
            }
            if (_carRigidbody != null)
            {
                _carRigidbody.velocity = Vector3.zero;
            }
        }

        _lapsInteractor?.ResetLapsAmount();
    }

    private bool Initialize(GameObject obj)
    {
        if (_carController == null) { _carController = obj.GetComponent<CarController>(); }
        if (_carRigidbody == null) { _carRigidbody = obj.GetComponent<Rigidbody>(); }
        if (_carTransform == null) { _carTransform = obj.GetComponent<Transform>(); }

        return _carController & _carRigidbody & _carRigidbody;
    }

    private void FadeInScreen() => _faderAnimator?.SetBool(FadeIn, true);

    private void FadeOutScreen() => _faderAnimator?.SetBool(FadeIn, false);
}
