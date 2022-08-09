using UnityEngine;

public sealed class CameraFollowTo : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField][Range(0.1f, 2f)] private float _lerpSpeed;
    private Vector3 _offset;

    public void Initialize()
    {
        if (_targetTransform == null) { return; }

        _offset = transform.position - _targetTransform.position;
    }

    private void FixedUpdate()
    {
        if (_targetTransform == null) { return; }

        transform.position = Vector3.Lerp(transform.position, (_offset + _targetTransform.position), _lerpSpeed * Time.fixedDeltaTime);
    }
}
