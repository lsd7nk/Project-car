using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private LayerMask _interactLayers;
    private BoxCollider _collider;

    private void Start()
    {
        _collider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if ((_interactLayers.value & (1 << collider.gameObject.layer)) != 0)
        {
            
        }
    }
}
