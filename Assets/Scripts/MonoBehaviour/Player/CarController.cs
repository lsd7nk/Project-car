using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public sealed class CarController : MonoBehaviour, IPauseHandler
{
    [HideInInspector] public event Action<bool, float, float> OnTireSkiddedEvent;
    [HideInInspector] public event Action<bool> OnDriftingEvent;
    [HideInInspector] public event Action<bool> OnChangeMoveSpeedEvent;
    [HideInInspector] public event Action OnResetMoveSpeedEvent;

    [Header("Characteristics")]
    [SerializeField][Range(90f, 150f)] private float _maxForwardSpeed;
    [SerializeField][Range(35f, 50f)] private float _maxBackwardSpeed;
    [SerializeField][Range(15f, 35f)] private float _maxSteeringAngle;
    [SerializeField][Range(0f, 1f)] private float _steeringSpeed = 0.5f;
    [SerializeField][Range(1f, 10f)] private float _accelerationMultiplier;
    [SerializeField][Range(0f, 3f)] private float _decelerationMultiplier;
    [SerializeField][Range(1f, 10f)] private float _handbrakeMultiplier;
    [SerializeField][Range(300f, 600f)] private float _breakForce;

    [Header("Wheels")]
    [SerializeField] private WheelCollider[] _frontWheelsColliders;
    [SerializeField] private WheelCollider[] _rearWheelsColliders;

    [SerializeField] private Transform[] _frontWheelsTransforms;
    [SerializeField] private Transform[] _rearWheelsTransforms;
    
    private CarControls _controls;
    private Rigidbody _rigidbody;
    private float _horizontalInput;
    private float _verticalInput;
    private bool _isBreaking;
    private bool _isDecelerating;
    private bool _isDrifting;
    private bool _isTireSkidded;
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
        _maxForwardSpeed += termMaxForwardSpeed;
        _accelerationMultiplier += termAccelerationMultiplier;
        OnChangeMoveSpeedEvent?.Invoke(isBoost);
    }

    public void ResetMoveSpeed()
    {
        _maxForwardSpeed = 120f;
        _accelerationMultiplier = 5f;
        OnResetMoveSpeedEvent?.Invoke();
    }

    public void ApplyBreaking(float currentBreakForce)
    {
        for (int i = 0; i < 2; i++)
        {
            _frontWheelsColliders[i].brakeTorque = currentBreakForce;
            _rearWheelsColliders[i].brakeTorque = currentBreakForce;
        }
    }

    private void Awake()
    {
        if (CreateControls)
        {
            _controls = new CarControls();
        }
        _rigidbody = GetComponent<Rigidbody>();

        PauseManager.RegisterHandler(this);
    }

    private void Start() => WheelFrictionCurveInitialize();

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
        if (IsPaused) { return; }

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
                ApplyBreaking(_breakForce);
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
                ApplyBreaking(_breakForce);
            }
            else
            {
                HandleMotor(_maxBackwardSpeed);
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
            _steeringAxis = _steeringAxis + (Time.fixedDeltaTime * 10 * _steeringSpeed);

            if (_steeringAxis > 1f)
            {
                _steeringAxis = 1f;
            }
        }
        else if (_horizontalInput < 0f)
        {
            _steeringAxis = _steeringAxis - (Time.fixedDeltaTime * 10 * _steeringSpeed);

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
                _frontWheelsColliders[i].brakeTorque = 0;
                _frontWheelsColliders[i].motorTorque = (_accelerationMultiplier * 50f) * _throttleAxis;
                _rearWheelsColliders[i].brakeTorque = 0;
                _rearWheelsColliders[i].motorTorque = (_accelerationMultiplier * 50f) * _throttleAxis;
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)  // If the maxSpeed has been reached, then stop applying torque to the wheels.
            {
                _frontWheelsColliders[i].motorTorque = 0;
                _rearWheelsColliders[i].motorTorque = 0;
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
        secureStartingPoint = _driftingAxis * _flWextremumSlip * _handbrakeMultiplier;

        if (secureStartingPoint < _flWextremumSlip)
        {
            _driftingAxis = _flWextremumSlip / (_flWextremumSlip * _handbrakeMultiplier);
        }
        if (_driftingAxis > 1f)
        {
            _driftingAxis = 1f;
        }

        ApplyDrifting(false);

        if (_driftingAxis < 1f)
        {
            _flWheelFriction.extremumSlip = _flWextremumSlip * _handbrakeMultiplier * _driftingAxis;
            _frontWheelsColliders[0].sidewaysFriction = _flWheelFriction;

            _frWheelFriction.extremumSlip = _frWextremumSlip * _handbrakeMultiplier * _driftingAxis;
            _frontWheelsColliders[1].sidewaysFriction = _frWheelFriction;

            _rlWheelFriction.extremumSlip = _rlWextremumSlip * _handbrakeMultiplier * _driftingAxis;
            _rearWheelsColliders[0].sidewaysFriction = _rlWheelFriction;

            _rrWheelFriction.extremumSlip = _rrWextremumSlip * _handbrakeMultiplier * _driftingAxis;
            _rearWheelsColliders[1].sidewaysFriction = _rrWheelFriction;
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

        _rigidbody.velocity = _rigidbody.velocity * (1f / (1f + (0.025f * _decelerationMultiplier)));

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
            _flWheelFriction.extremumSlip = _flWextremumSlip * _handbrakeMultiplier * _driftingAxis;
            _frontWheelsColliders[0].sidewaysFriction = _flWheelFriction;

            _frWheelFriction.extremumSlip = _frWextremumSlip * _handbrakeMultiplier * _driftingAxis;
            _frontWheelsColliders[1].sidewaysFriction = _frWheelFriction;

            _rlWheelFriction.extremumSlip = _rlWextremumSlip * _handbrakeMultiplier * _driftingAxis;
            _rearWheelsColliders[0].sidewaysFriction = _rlWheelFriction;

            _rrWheelFriction.extremumSlip = _rrWextremumSlip * _handbrakeMultiplier * _driftingAxis;
            _rearWheelsColliders[1].sidewaysFriction = _rrWheelFriction;

            Invoke("RecoverTraction", Time.fixedDeltaTime);
        }
        else if (_flWheelFriction.extremumSlip < _flWextremumSlip)
        {
            _flWheelFriction.extremumSlip = _flWextremumSlip;
            _frontWheelsColliders[0].sidewaysFriction = _flWheelFriction;

            _frWheelFriction.extremumSlip = _frWextremumSlip;
            _frontWheelsColliders[1].sidewaysFriction = _frWheelFriction;

            _rlWheelFriction.extremumSlip = _rlWextremumSlip;
            _rearWheelsColliders[0].sidewaysFriction = _rlWheelFriction;

            _rrWheelFriction.extremumSlip = _rrWextremumSlip;
            _rearWheelsColliders[1].sidewaysFriction = _rrWheelFriction;

            _driftingAxis = 0f;
        }
    }

    private void TrottleOff()
    {
        for (int i = 0; i < 2; i++)
        {
            _frontWheelsColliders[i].motorTorque = 0f;
            _rearWheelsColliders[i].motorTorque = 0f;
        }
    }

    private void TurnWheels()
    {
        float steeringAngle = _steeringAxis * _maxSteeringAngle;

        for (int i = 0; i < 2; i++)
        {
            _frontWheelsColliders[i].steerAngle = Mathf.Lerp(_frontWheelsColliders[i].steerAngle, steeringAngle, _steeringSpeed);
        }
    }

    private void ResetSteeringAxis()  // The following method takes the front car wheels to their default position (rotation = 0)
    {
        if (_steeringAxis > 0f)
        {
            _steeringAxis = _steeringAxis - (Time.fixedDeltaTime * 10f * _steeringSpeed);
        }
        else if (_steeringAxis < 0f)
        {
            _steeringAxis = _steeringAxis + (Time.fixedDeltaTime * 10f * _steeringSpeed);
        }

        if (Mathf.Abs(_frontWheelsColliders[0].steerAngle) < 1f)
        {
            _steeringAxis = 0f;
        }
    }

    private void UpdateWheels()
    {
        for (int i = 0; i < 2; i++)
        {
            UpdateSingleWheel(_frontWheelsColliders[i], _frontWheelsTransforms[i]);
            UpdateSingleWheel(_rearWheelsColliders[i], _rearWheelsTransforms[i]);
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
        _flWheelFriction.extremumSlip = _frontWheelsColliders[0].sidewaysFriction.extremumSlip;
        _flWextremumSlip = _frontWheelsColliders[0].sidewaysFriction.extremumSlip;
        _flWheelFriction.extremumValue = _frontWheelsColliders[0].sidewaysFriction.extremumValue;
        _flWheelFriction.asymptoteSlip = _frontWheelsColliders[0].sidewaysFriction.asymptoteSlip;
        _flWheelFriction.asymptoteValue = _frontWheelsColliders[0].sidewaysFriction.asymptoteValue;
        _flWheelFriction.stiffness = _frontWheelsColliders[0].sidewaysFriction.stiffness;

        _frWheelFriction = new WheelFrictionCurve();
        _frWheelFriction.extremumSlip = _frontWheelsColliders[1].sidewaysFriction.extremumSlip;
        _frWextremumSlip = _frontWheelsColliders[1].sidewaysFriction.extremumSlip;
        _frWheelFriction.extremumValue = _frontWheelsColliders[1].sidewaysFriction.extremumValue;
        _frWheelFriction.asymptoteSlip = _frontWheelsColliders[1].sidewaysFriction.asymptoteSlip;
        _frWheelFriction.asymptoteValue = _frontWheelsColliders[1].sidewaysFriction.asymptoteValue;
        _frWheelFriction.stiffness = _frontWheelsColliders[1].sidewaysFriction.stiffness;

        _rlWheelFriction = new WheelFrictionCurve();
        _rlWheelFriction.extremumSlip = _rearWheelsColliders[0].sidewaysFriction.extremumSlip;
        _rlWextremumSlip = _rearWheelsColliders[0].sidewaysFriction.extremumSlip;
        _rlWheelFriction.extremumValue = _rearWheelsColliders[0].sidewaysFriction.extremumValue;
        _rlWheelFriction.asymptoteSlip = _rearWheelsColliders[0].sidewaysFriction.asymptoteSlip;
        _rlWheelFriction.asymptoteValue = _rearWheelsColliders[0].sidewaysFriction.asymptoteValue;
        _rlWheelFriction.stiffness = _rearWheelsColliders[0].sidewaysFriction.stiffness;

        _rrWheelFriction = new WheelFrictionCurve();
        _rrWheelFriction.extremumSlip = _rearWheelsColliders[1].sidewaysFriction.extremumSlip;
        _rrWextremumSlip = _rearWheelsColliders[1].sidewaysFriction.extremumSlip;
        _rrWheelFriction.extremumValue = _rearWheelsColliders[1].sidewaysFriction.extremumValue;
        _rrWheelFriction.asymptoteSlip = _rearWheelsColliders[1].sidewaysFriction.asymptoteSlip;
        _rrWheelFriction.asymptoteValue = _rearWheelsColliders[1].sidewaysFriction.asymptoteValue;
        _rrWheelFriction.stiffness = _rearWheelsColliders[1].sidewaysFriction.stiffness;
    }

    private void CalculateCarStates()
    {
        _carSpeed = (2 * Mathf.PI * _frontWheelsColliders[0].radius * _frontWheelsColliders[0].rpm * 60) / 1000;
        // Save the local velocity of the car in the x axis. Used to know if the car is drifting.
        _localVelocityX = transform.InverseTransformDirection(_rigidbody.velocity).x;
        // Save the local velocity of the car in the z axis. Used to know if the car is going forward or backwards.
        _localVelocityZ = transform.InverseTransformDirection(_rigidbody.velocity).z;
    }
}