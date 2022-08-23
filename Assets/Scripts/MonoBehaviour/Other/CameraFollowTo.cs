using UnityEngine;

namespace ProjectCar
{
    public sealed class CameraFollowTo : MonoBehaviour
    {
        [SerializeField] private Transform _targetTransform;
        [SerializeField][Range(0.1f, 2f)] private float _lerpSpeed;
        private Vector3 _offset;
        private const string _targetTag = "Player";

        private void Awake() => Initialize();

        private void FixedUpdate()
        {
            if (_targetTransform == null) { return; }

            transform.position = Vector3.Lerp(transform.position, (_offset + _targetTransform.position), _lerpSpeed * Time.fixedDeltaTime);
        }

        private void Initialize()
        {
            _targetTransform = GameObject.FindGameObjectWithTag(_targetTag).transform;

            _offset = transform.position - _targetTransform.position;
        }
    }
}
