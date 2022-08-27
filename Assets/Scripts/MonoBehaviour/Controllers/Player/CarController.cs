using System;
using UnityEngine;
using ProjectCar.Managers;
using ProjectCar.Configs;

namespace ProjectCar
{
    namespace Controllers
    {
        [Serializable]
        public sealed class Wheel
        {
            [field: SerializeField] public Transform Transform { get; private set; }
            [field: SerializeField] public WheelCollider Collider { get; private set; }
        }

        [RequireComponent(typeof(Rigidbody))]
        public sealed class CarController : MonoBehaviour, IPauseHandler
        {
            [HideInInspector] public event Action<bool, float, float> OnTireSkiddedEvent;
            [HideInInspector] public event Action<bool> OnDriftingEvent;
            [HideInInspector] public event Action<bool> OnChangeMoveSpeedEvent;
            [HideInInspector] public event Action OnResetMoveSpeedEvent;

            [Header("Characteristics")]
            [SerializeField] private CarConfig _config;
            private float _maxForwardSpeed;
            private float _accelerationMultiplier;

            [Header("Wheels")]
            [SerializeField] private Wheel[] _frontWheels = new Wheel[2];
            [SerializeField] private Wheel[] _rearWheels = new Wheel[2];

            private CarControls _controls;
            private Rigidbody _rigidbody;

            private float _horizontalInput;
            private float _verticalInput;

            private bool _isBreaking;
            private bool _isDecelerating;
            private bool _isDrifting;
            private bool _isTireSkidded;
            private bool _isChangedSpeed;

            private float _carSpeed;
            private float _localVelocityX;
            private float _localVelocityZ;
            // Used to know whether the throttle has reached the maximum value. It goes from -1 to 1.
            private float _throttleAxis;
            private float _steeringAxis;
            private float _driftingAxis;

            // The following variables are used to store information about sideways friction of the wheels(such as
            // extremumSlip, extremumValue, asymptoteSlip, asymptoteValue and stiffness). We change this values to
            // make the car to start drifting.
            private WheelFrictionCurve _flWheelFriction;
            private float _flWextremumSlip;
            private WheelFrictionCurve _frWheelFriction;
            private float _frWextremumSlip;
            private WheelFrictionCurve _rlWheelFriction;
            private float _rlWextremumSlip;
            private WheelFrictionCurve _rrWheelFriction;
            private float _rrWextremumSlip;

            [field: SerializeField] public bool CreateControls { get; private set; }
            public bool IsPaused => PauseManager.Instance.IsPaused;
            public PauseManager PauseManager => PauseManager.Instance;

            public void SetPause(bool isPaused) => Time.timeScale = (isPaused) ? 0f : 1f;

            public void DriveForward()
            {
                if (CreateControls) { return; }

                _verticalInput = 1f;
            }

            public void ChangeMoveSpeed(bool isBoost, float termMaxForwardSpeed, float termAccelerationMultiplier)
            {
                if (!_isChangedSpeed)
                {
                    _isChangedSpeed = true;

                    _maxForwardSpeed += termMaxForwardSpeed;
                    _accelerationMultiplier += termAccelerationMultiplier;
                    OnChangeMoveSpeedEvent?.Invoke(isBoost);
                }
            }

            public void ResetMoveSpeed()
            {
                if (_isChangedSpeed)
                {
                    _maxForwardSpeed = 120f;
                    _accelerationMultiplier = 5f;
                    OnResetMoveSpeedEvent?.Invoke();

                    _isChangedSpeed = false;
                }
            }

            public void ApplyBreaking(float currentBreakForce)
            {
                for (int i = 0; i < 2; i++)
                {
                    _frontWheels[i].Collider.brakeTorque = currentBreakForce;
                    _rearWheels[i].Collider.brakeTorque = currentBreakForce;
                }
            }

            private void Awake()
            {
                if (CreateControls)
                {
                    _controls = new CarControls();
                }
                _rigidbody = GetComponent<Rigidbody>();

                if (PauseManager != null)
                {
                    PauseManager.RegisterHandler(this);
                }
            }

            private void Start()
            {
                _maxForwardSpeed = _config.MaxForwardSpeed;
                _accelerationMultiplier = _config.AccelerationMultiplier;

                WheelFrictionCurveInitialize();
            }

            private void OnEnable()
            {
                if (!CreateControls) { return; }

                _controls.Car.Enable();
                _controls.Car.Break.canceled += _ => RecoverTraction();
            }

            private void OnDisable()
            {
                if (!CreateControls) { return; }

                _controls.Car.Break.canceled -= _ => RecoverTraction();
                _controls.Car.Disable();
            }

            private void FixedUpdate()
            {
                if (PauseManager != null)
                {
                    if (IsPaused) { return; }
                }

                GetInput();
                CalculateCarStates();
                HandleMoveCar();
                UpdateWheels();
            }

            private void GetInput()
            {
                if (!CreateControls) { return; }

                _horizontalInput = _controls.Car.Move.ReadValue<Vector2>().x;
                _verticalInput = _controls.Car.Move.ReadValue<Vector2>().y;
                _isBreaking = Convert.ToBoolean(_controls.Car.Break.ReadValue<float>());
            }

            private void HandleMoveCar()
            {
                DeterminationOfTheDirectionOfTurnSteering();
                DeterminationOfTheDirectionOfMovement();

                if (_isBreaking)
                {
                    CancelInvoke("DecelerateCar");
                    _isDecelerating = false;
                    HandleHandbrake();
                }

                if (_verticalInput == 0f && !_isBreaking && !_isDecelerating)
                {
                    InvokeRepeating("DecelerateCar", 0f, 0.1f);
                    _isDecelerating = true;
                }
            }

            private void DeterminationOfTheDirectionOfMovement()
            {
                if (_verticalInput > 0f)
                {
                    CancelInvoke("DecelerateCar");
                    _isDecelerating = false;

                    ApplyDrifting();

                    _throttleAxis = _throttleAxis + (Time.fixedDeltaTime * 3f);

                    if (_throttleAxis > 1f)
                    {
                        _throttleAxis = 1f;
                    }

                    if (_localVelocityZ < -1f)
                    {
                        ApplyBreaking(_config.BreakForce);
                    }
                    else
                    {
                        HandleMotor(_maxForwardSpeed);
                    }
                }
                else if (_verticalInput < 0f)
                {
                    CancelInvoke("DecelerateCar");
                    _isDecelerating = false;

                    ApplyDrifting();

                    _throttleAxis = _throttleAxis - (Time.fixedDeltaTime * 3f);

                    if (_throttleAxis < -1f)
                    {
                        _throttleAxis = -1f;
                    }

                    if (_localVelocityZ > 1f)
                    {
                        ApplyBreaking(_config.BreakForce);
                    }
                    else
                    {
                        HandleMotor(_config.MaxBackwardSpeed);
                    }
                }
                else
                {
                    TrottleOff();
                }
            }

            private void DeterminationOfTheDirectionOfTurnSteering()
            {
                if (_horizontalInput > 0f)
                {
                    _steeringAxis = _steeringAxis + (Time.fixedDeltaTime * 10 * _config.SteeringSpeed);

                    if (_steeringAxis > 1f)
                    {
                        _steeringAxis = 1f;
                    }
                }
                else if (_horizontalInput < 0f)
                {
                    _steeringAxis = _steeringAxis - (Time.fixedDeltaTime * 10 * _config.SteeringSpeed);

                    if (_steeringAxis < -1f)
                    {
                        _steeringAxis = -1f;
                    }
                }
                else if (_horizontalInput == 0f && _steeringAxis != 0f)
                {
                    ResetSteeringAxis();
                }

                // The following method turns the front car wheels to the left. The speed of this movement will depend on the steeringSpeed variable.
                TurnWheels();
            }

            private void HandleMotor(float _maxSpeed)
            {
                if (Mathf.RoundToInt(_carSpeed) < _maxSpeed)
                {
                    for (int i = 0; i < 2; i++)  // Apply positive torque in all wheels to go forward if maxSpeed has not been reached.
                    {
                        _frontWheels[i].Collider.brakeTorque = 0;
                        _frontWheels[i].Collider.motorTorque = (_accelerationMultiplier * 50f) * _throttleAxis;
                        _rearWheels[i].Collider.brakeTorque = 0;
                        _rearWheels[i].Collider.motorTorque = (_accelerationMultiplier * 50f) * _throttleAxis;
                    }
                }
                else
                {
                    for (int i = 0; i < 2; i++)  // If the maxSpeed has been reached, then stop applying torque to the wheels.
                    {
                        _frontWheels[i].Collider.motorTorque = 0;
                        _rearWheels[i].Collider.motorTorque = 0;
                    }
                }
            }

            private void ApplyDrifting(bool playVFX = true)
            {
                _isDrifting = (Mathf.Abs(_localVelocityX) > 2.5f) ? true : false;

                if (playVFX)
                {
                    PlayDriftingVFX();
                }
            }

            private void HandleHandbrake()
            {
                CancelInvoke("RecoverTraction");

                float secureStartingPoint;

                _driftingAxis = _driftingAxis + Time.fixedDeltaTime;
                secureStartingPoint = _driftingAxis * _flWextremumSlip * _config.HandbrakeMultiplier;

                if (secureStartingPoint < _flWextremumSlip)
                {
                    _driftingAxis = _flWextremumSlip / (_flWextremumSlip * _config.HandbrakeMultiplier);
                }
                if (_driftingAxis > 1f)
                {
                    _driftingAxis = 1f;
                }

                ApplyDrifting(false);

                if (_driftingAxis < 1f)
                {
                    _flWheelFriction.extremumSlip = _flWextremumSlip * _config.HandbrakeMultiplier * _driftingAxis;
                    _frontWheels[0].Collider.sidewaysFriction = _flWheelFriction;

                    _frWheelFriction.extremumSlip = _frWextremumSlip * _config.HandbrakeMultiplier * _driftingAxis;
                    _frontWheels[1].Collider.sidewaysFriction = _frWheelFriction;

                    _rlWheelFriction.extremumSlip = _rlWextremumSlip * _config.HandbrakeMultiplier * _driftingAxis;
                    _rearWheels[0].Collider.sidewaysFriction = _rlWheelFriction;

                    _rrWheelFriction.extremumSlip = _rrWextremumSlip * _config.HandbrakeMultiplier * _driftingAxis;
                    _rearWheels[1].Collider.sidewaysFriction = _rrWheelFriction;
                }

                _isTireSkidded = true;
                ApplyDrifting();
            }

            private void DecelerateCar()
            {
                ApplyDrifting();

                if (_throttleAxis != 0f)
                {
                    if (_throttleAxis > 0f)
                    {
                        _throttleAxis = _throttleAxis - (Time.fixedDeltaTime * 10f);
                    }
                    else if (_throttleAxis < 0f)
                    {
                        _throttleAxis = _throttleAxis + (Time.fixedDeltaTime * 10f);
                    }

                    if (Mathf.Abs(_throttleAxis) < 0.15f)
                    {
                        _throttleAxis = 0f;
                    }
                }

                _rigidbody.velocity = _rigidbody.velocity * (1f / (1f + (0.025f * _config.DecelerationMultiplier)));

                TrottleOff();

                if (_rigidbody.velocity.magnitude < 0.25f)
                {
                    _rigidbody.velocity = Vector3.zero;
                    CancelInvoke("DecelerateCar");
                }
            }

            private void PlayDriftingVFX()
            {
                OnDriftingEvent?.Invoke(_isDrifting);
                OnTireSkiddedEvent?.Invoke(_isTireSkidded, _localVelocityX, _carSpeed);
            }

            private void RecoverTraction()
            {
                _isTireSkidded = false;
                _driftingAxis = _driftingAxis - (Time.fixedDeltaTime / 1.5f);

                if (_driftingAxis < 0f)
                {
                    _driftingAxis = 0f;
                }

                // If the 'driftingAxis' value is not 0f, it means that the wheels have not recovered their traction.
                // I are going to continue decreasing the sideways friction of the wheels until we reach the initial
                // car's grip.
                if (_flWheelFriction.extremumSlip > _flWextremumSlip)
                {
                    _flWheelFriction.extremumSlip = _flWextremumSlip * _config.HandbrakeMultiplier * _driftingAxis;
                    _frontWheels[0].Collider.sidewaysFriction = _flWheelFriction;

                    _frWheelFriction.extremumSlip = _frWextremumSlip * _config.HandbrakeMultiplier * _driftingAxis;
                    _frontWheels[1].Collider.sidewaysFriction = _frWheelFriction;

                    _rlWheelFriction.extremumSlip = _rlWextremumSlip * _config.HandbrakeMultiplier * _driftingAxis;
                    _rearWheels[0].Collider.sidewaysFriction = _rlWheelFriction;

                    _rrWheelFriction.extremumSlip = _rrWextremumSlip * _config.HandbrakeMultiplier * _driftingAxis;
                    _rearWheels[1].Collider.sidewaysFriction = _rrWheelFriction;

                    Invoke("RecoverTraction", Time.fixedDeltaTime);
                }
                else if (_flWheelFriction.extremumSlip < _flWextremumSlip)
                {
                    _flWheelFriction.extremumSlip = _flWextremumSlip;
                    _frontWheels[0].Collider.sidewaysFriction = _flWheelFriction;

                    _frWheelFriction.extremumSlip = _frWextremumSlip;
                    _frontWheels[1].Collider.sidewaysFriction = _frWheelFriction;

                    _rlWheelFriction.extremumSlip = _rlWextremumSlip;
                    _rearWheels[0].Collider.sidewaysFriction = _rlWheelFriction;

                    _rrWheelFriction.extremumSlip = _rrWextremumSlip;
                    _rearWheels[1].Collider.sidewaysFriction = _rrWheelFriction;

                    _driftingAxis = 0f;
                }
            }

            private void TrottleOff()
            {
                for (int i = 0; i < 2; ++i)
                {
                    _frontWheels[i].Collider.motorTorque = 0f;
                    _rearWheels[i].Collider.motorTorque = 0f;
                }
            }

            private void TurnWheels()
            {
                float steeringAngle = _steeringAxis * _config.MaxSteeringAngle;

                for (int i = 0; i < 2; ++i)
                {
                    _frontWheels[i].Collider.steerAngle = Mathf.Lerp(_frontWheels[i].Collider.steerAngle, steeringAngle, _config.SteeringSpeed);
                }
            }

            private void ResetSteeringAxis()  // The following method takes the front car wheels to their default position (rotation = 0)
            {
                if (_steeringAxis > 0f)
                {
                    _steeringAxis = _steeringAxis - (Time.fixedDeltaTime * 10f * _config.SteeringSpeed);
                }
                else if (_steeringAxis < 0f)
                {
                    _steeringAxis = _steeringAxis + (Time.fixedDeltaTime * 10f * _config.SteeringSpeed);
                }

                if (Mathf.Abs(_frontWheels[0].Collider.steerAngle) < 1f)
                {
                    _steeringAxis = 0f;
                }
            }

            private void UpdateWheels()
            {
                for (int i = 0; i < 2; ++i)
                {
                    UpdateSingleWheel(_frontWheels[i].Collider, _frontWheels[i].Transform);
                    UpdateSingleWheel(_rearWheels[i].Collider, _rearWheels[i].Transform);
                }
            }

            private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
            {
                Vector3 position;
                Quaternion rotation;

                wheelCollider.GetWorldPose(out position, out rotation);

                wheelTransform.rotation = Quaternion.Lerp(wheelTransform.rotation, rotation, 15f * Time.fixedDeltaTime);
                wheelTransform.position = position;
            }

            private void WheelFrictionCurveInitialize()
            {
                _flWheelFriction = new WheelFrictionCurve();
                _flWheelFriction.extremumSlip = _frontWheels[0].Collider.sidewaysFriction.extremumSlip;
                _flWextremumSlip = _frontWheels[0].Collider.sidewaysFriction.extremumSlip;
                _flWheelFriction.extremumValue = _frontWheels[0].Collider.sidewaysFriction.extremumValue;
                _flWheelFriction.asymptoteSlip = _frontWheels[0].Collider.sidewaysFriction.asymptoteSlip;
                _flWheelFriction.asymptoteValue = _frontWheels[0].Collider.sidewaysFriction.asymptoteValue;
                _flWheelFriction.stiffness = _frontWheels[0].Collider.sidewaysFriction.stiffness;

                _frWheelFriction = new WheelFrictionCurve();
                _frWheelFriction.extremumSlip = _frontWheels[1].Collider.sidewaysFriction.extremumSlip;
                _frWextremumSlip = _frontWheels[1].Collider.sidewaysFriction.extremumSlip;
                _frWheelFriction.extremumValue = _frontWheels[1].Collider.sidewaysFriction.extremumValue;
                _frWheelFriction.asymptoteSlip = _frontWheels[1].Collider.sidewaysFriction.asymptoteSlip;
                _frWheelFriction.asymptoteValue = _frontWheels[1].Collider.sidewaysFriction.asymptoteValue;
                _frWheelFriction.stiffness = _frontWheels[1].Collider.sidewaysFriction.stiffness;

                _rlWheelFriction = new WheelFrictionCurve();
                _rlWheelFriction.extremumSlip = _rearWheels[0].Collider.sidewaysFriction.extremumSlip;
                _rlWextremumSlip = _rearWheels[0].Collider.sidewaysFriction.extremumSlip;
                _rlWheelFriction.extremumValue = _rearWheels[0].Collider.sidewaysFriction.extremumValue;
                _rlWheelFriction.asymptoteSlip = _rearWheels[0].Collider.sidewaysFriction.asymptoteSlip;
                _rlWheelFriction.asymptoteValue = _rearWheels[0].Collider.sidewaysFriction.asymptoteValue;
                _rlWheelFriction.stiffness = _rearWheels[0].Collider.sidewaysFriction.stiffness;

                _rrWheelFriction = new WheelFrictionCurve();
                _rrWheelFriction.extremumSlip = _rearWheels[1].Collider.sidewaysFriction.extremumSlip;
                _rrWextremumSlip = _rearWheels[1].Collider.sidewaysFriction.extremumSlip;
                _rrWheelFriction.extremumValue = _rearWheels[1].Collider.sidewaysFriction.extremumValue;
                _rrWheelFriction.asymptoteSlip = _rearWheels[1].Collider.sidewaysFriction.asymptoteSlip;
                _rrWheelFriction.asymptoteValue = _rearWheels[1].Collider.sidewaysFriction.asymptoteValue;
                _rrWheelFriction.stiffness = _rearWheels[1].Collider.sidewaysFriction.stiffness;
            }

            private void CalculateCarStates()
            {
                _carSpeed = (2 * Mathf.PI * _frontWheels[0].Collider.radius * _frontWheels[0].Collider.rpm * 60) / 1000;
                // Save the local velocity of the car in the x axis. Used to know if the car is drifting.
                _localVelocityX = transform.InverseTransformDirection(_rigidbody.velocity).x;
                // Save the local velocity of the car in the z axis. Used to know if the car is going forward or backwards.
                _localVelocityZ = transform.InverseTransformDirection(_rigidbody.velocity).z;
            }
        }
    }
}