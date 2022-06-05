using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private CarControls _controls;

    private float _horizontalInput;
    private float _verticalInput;
    private float _currentSteerAngle;
    private float _currentBreakForce;
    private bool _isBreaking;

    [SerializeField] private float _motorForce;
    [SerializeField] private float _breakForce;
    [SerializeField] private float _maxSteerAngle;

    [SerializeField] private WheelCollider[] _frontWheelsColliders;
    [SerializeField] private WheelCollider[] _rearWheelsColliders;

    [SerializeField] private Transform[] _frontWheelsTransforms;
    [SerializeField] private Transform[] _rearWheelsTransforms;

    private void Awake() { _controls = new CarControls(); }

    private void OnEnable() { _controls.Car.Enable(); }

    private void OnDisable() { _controls.Car.Disable(); }

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
    }


    private void GetInput()
    {
        _horizontalInput = _controls.Car.Move.ReadValue<Vector2>().x;
        _verticalInput = _controls.Car.Move.ReadValue<Vector2>().y;
        _isBreaking = Convert.ToBoolean(_controls.Car.Break.ReadValue<float>());
    }

    private void HandleMotor()
    {
        float currentBreakForce;

        _frontWheelsColliders[0].motorTorque = _verticalInput * _motorForce;
        _frontWheelsColliders[1].motorTorque = _verticalInput * _motorForce;
        currentBreakForce = _isBreaking ? _breakForce : 0f;

        ApplyBreaking(currentBreakForce);       
    }

    private void ApplyBreaking(float currentBreakForce)
    {
        for (int i = 0; i < 2; i++)
        {
            _frontWheelsColliders[i].brakeTorque = currentBreakForce;
            _rearWheelsColliders[i].brakeTorque = currentBreakForce;
        }
    }

    private void HandleSteering()
    {
        _currentSteerAngle = _maxSteerAngle * _horizontalInput;

        foreach(WheelCollider frontWheelCollider in _frontWheelsColliders)
        {
            frontWheelCollider.steerAngle = _currentSteerAngle;
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
        wheelTransform.rotation = rotation;
        wheelTransform.position = position;
    }
}