using UnityEngine;

namespace ProjectCar
{
    namespace Controllers
    {
        [RequireComponent(typeof(CarController))]
        public sealed class VFXCarController : MonoBehaviour
        {
            [Header("VFX")]
            [SerializeField] private TrailRenderer _trail;
            [SerializeField] private Material _speedPenaltyMaterial;
            [Space(10)]
            [SerializeField] private ParticleSystem _rlTireSmoke;
            [SerializeField] private TrailRenderer _rlTireSkid;
            [SerializeField] private ParticleSystem _rrTireSmoke;
            [SerializeField] private TrailRenderer _rrTireSkid;
            private Material _speedBoostMaterial;
            private CarController _carController;
            private bool _isTurnOffTrail;
            private float _defaultTrailTime;

            private void Awake()
            {
                _carController = GetComponent<CarController>();

                if (_trail != null)
                {
                    _defaultTrailTime = _trail.time;
                    _speedBoostMaterial = _trail.material;
                }
            }

            private void OnEnable()
            {
                _carController.OnChangeMoveSpeedEvent += TurnOnTrail;
                _carController.OnResetMoveSpeedEvent += TurnOffTrail;
                _carController.OnDriftingEvent += PlayDriftingSmokeEffects;
                _carController.OnTireSkiddedEvent += EnableEmittingTireSkiddedEffects;
            }

            private void OnDisable()
            {
                _carController.OnChangeMoveSpeedEvent -= TurnOnTrail;
                _carController.OnResetMoveSpeedEvent -= TurnOffTrail;
                _carController.OnDriftingEvent -= PlayDriftingSmokeEffects;
                _carController.OnTireSkiddedEvent -= EnableEmittingTireSkiddedEffects;
            }

            private void FixedUpdate()
            {
                if (_isTurnOffTrail)
                {
                    TurnOffTrailByTime();
                }
            }

            private void TurnOnTrail(bool isBoost)
            {
                if (_trail == null) { return; }

                if (_speedBoostMaterial != null && _speedPenaltyMaterial != null)
                {
                    _trail.material = (isBoost) ? _speedBoostMaterial : _speedPenaltyMaterial;
                }

                _trail.enabled = true;
            }

            private void TurnOffTrail()
            {
                if (_trail == null) { return; }

                _isTurnOffTrail = true;
            }

            private void TurnOffTrailByTime()
            {
                _trail.time -= Time.fixedDeltaTime;

                if (_trail.time <= 0)
                {
                    _isTurnOffTrail = false;
                    _trail.time = _defaultTrailTime;
                    _trail.enabled = false;
                }
            }

            private void PlayDriftingSmokeEffects(bool isDrifting)
            {
                if (_rlTireSmoke != null && _rrTireSmoke != null)
                {
                    if (isDrifting)
                    {
                        _rlTireSmoke.Play();
                        _rrTireSmoke.Play();
                    }
                    else
                    {
                        _rlTireSmoke.Stop();
                        _rrTireSmoke.Stop();
                    }
                }
            }

            private void EnableEmittingTireSkiddedEffects(bool isTireSkidded, float localVelocityX, float carSpeed)
            {
                if (_rlTireSkid != null && _rrTireSkid != null)
                {
                    if (isTireSkidded && Mathf.Abs(localVelocityX) > 5f && Mathf.Abs(carSpeed) > 12f)
                    {
                        _rlTireSkid.emitting = true;
                        _rrTireSkid.emitting = true;
                    }
                    else
                    {
                        _rlTireSkid.emitting = false;
                        _rrTireSkid.emitting = false;
                    }
                }
            }
        }
    }
}
