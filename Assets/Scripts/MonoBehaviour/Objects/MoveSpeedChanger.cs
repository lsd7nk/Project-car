using System.Collections;
using UnityEngine;
using ProjectCar.Controllers;
using ProjectCar.Managers;


namespace ProjectCar
{
    namespace Objects
    {
        [RequireComponent(typeof(BoxCollider))]
        public sealed class MoveSpeedChanger : MonoBehaviour
        {
            [SerializeField] private LayerMask _interactLayers;
            [SerializeField][Range(-40f, 40f)] private float _maxForwardSpeedChangeRate;
            [SerializeField][Range(-3f, 3f)] private float _accelerationMultiplierChangeRate;
            [SerializeField] private int _changeTime;
            [SerializeField] private bool _isBoost;
            private CarController _carController;

            public bool IsPaused => PauseManager.Instance.IsPaused;

            private void OnTriggerEnter(Collider other)
            {
                if ((_interactLayers.value & (1 << other.gameObject.layer)) != 0)
                {
                    _carController ??= other.GetComponentInParent<CarController>();

                    StartCoroutine(ChangeSpeedRoutine());
                }
            }

            private IEnumerator ChangeSpeedRoutine()
            {
                if (_carController != null)
                {
                    _carController.ChangeMoveSpeed(_isBoost, _maxForwardSpeedChangeRate, _accelerationMultiplierChangeRate);
                    yield return new WaitForSecondsRealtime(_changeTime);
                    _carController.ResetMoveSpeed();
                    yield break;
                }
            }
        }
    }
}
