using System;
using System.Collections;
using UnityEngine;
using ProjectCar.Controllers;
using ProjectCar.UI;

namespace ProjectCar
{
    namespace Handlers
    {
        public class FallsHandler : MonoBehaviour
        {
            [HideInInspector] public event Action OnFallHandleEvent;
            [SerializeField] private LayerMask _interactLayers;
            [SerializeField] private Transform _startTransform;
            [SerializeField] private Fader _fader;
            private CarController _carController;
            private Transform _carTransform;
            private Rigidbody _carRigidbody;
            private bool _isFirstEnter = false;

            [field: SerializeField] public TrainingObject TrainingÑonfig { get; private set; }

            public void Initialize() => _fader.Initialize();

            private void OnTriggerEnter(Collider other)
            {
                if ((_interactLayers.value & (1 << other.gameObject.layer)) != 0)
                {
                    _isFirstEnter = !_isFirstEnter;

                    if (_isFirstEnter)
                    {
                        StartCoroutine(HandleFall(other.transform.parent.gameObject));
                    }
                }
                else
                {
                    if (other.CompareTag("Chain"))
                    {
                        StartCoroutine(TurnOffObjectRoutine(other.transform.parent.gameObject));
                    }
                    else
                    {
                        StartCoroutine(TurnOffObjectRoutine(other.gameObject));
                    }
                }


                static IEnumerator TurnOffObjectRoutine(GameObject obj)
                {
                    yield return new WaitForSecondsRealtime(2f);
                    obj.SetActive(false);

                    yield break;
                }
            }

            private IEnumerator HandleFall(GameObject obj)
            {
                _fader.gameObject.SetActive(true);
                _fader.FadeInScreen();
                yield return new WaitForSeconds(1.5f);
                GetBackOnTheRoad(obj);
                yield return new WaitForSeconds(1f);
                _fader.FadeOutScreen();
                yield return new WaitForSeconds(0.1f);
                OnFallHandleEvent?.Invoke();
                yield return new WaitForSeconds(1f);
                _fader.gameObject.SetActive(false);

                if (TrainingÑonfig != null)
                {
                    if (TrainingÑonfig.IsCalledFromAnotherScript && !TrainingÑonfig.IsTriggerPassed)
                    {
                        TrainingÑonfig.TrainingDescriptionDisplay();
                    }
                }

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
            }

            private bool Initialize(GameObject obj)
            {
                _carController ??= obj.GetComponent<CarController>();
                _carRigidbody ??= obj.GetComponent<Rigidbody>();
                _carTransform ??= obj.GetComponent<Transform>();

                return _carController & _carRigidbody & _carRigidbody;
            }
        }
    }
}
