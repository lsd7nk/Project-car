using UnityEngine;
using UnityEngine.Events;

namespace ProjectCar
{
    public class CheckPoint : MonoBehaviour
    {
        [SerializeField] private LayerMask _interactLayers;
        [SerializeField] private int _index;
        [HideInInspector] public UnityEvent<CheckPoint> OnCheckPointComplete;

        public int Index => _index;

        private void OnTriggerEnter(Collider other)
        {
            if ((_interactLayers.value & (1 << other.gameObject.layer)) != 0)
            {
                OnCheckPointComplete?.Invoke(this);
            }
        }
    }
}
