using UnityEngine;

[CreateAssetMenu(fileName = "Car", menuName = "Config/Car", order = 52)]
public class CarConfig : ScriptableObject
{
    [SerializeField][Range(90f, 150f)] private float _maxForwardSpeed;
    [SerializeField][Range(35f, 50f)] private float _maxBackwardSpeed;
    [SerializeField][Range(15f, 35f)] private float _maxSteeringAngle;
    [SerializeField][Range(0f, 1f)] private float _steeringSpeed = 0.5f;
    [SerializeField][Range(1f, 10f)] private float _accelerationMultiplier;
    [SerializeField][Range(0f, 3f)] private float _decelerationMultiplier;
    [SerializeField][Range(1f, 10f)] private float _handbrakeMultiplier;
    [SerializeField][Range(300f, 600f)] private float _breakForce;

    public float MaxForwardSpeed => _maxForwardSpeed;
    public float MaxBackwardSpeed => _maxBackwardSpeed;
    public float MaxSteeringAngle => _maxSteeringAngle;
    public float SteeringSpeed => _steeringSpeed;
    public float AccelerationMultiplier => _accelerationMultiplier;
    public float DecelerationMultiplier => _decelerationMultiplier;
    public float HandbrakeMultiplier => _handbrakeMultiplier;
    public float BreakForce => _breakForce;


}
