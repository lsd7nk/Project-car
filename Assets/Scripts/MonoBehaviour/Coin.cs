using UnityEngine;
using UnityEngine.Events;

public class Coin : MonoBehaviour
{
    [SerializeField] private LayerMask _interactLayers;
    [HideInInspector] public UnityEvent OnMoneyCollection;

    private void OnTriggerEnter(Collider collider)
    {
        if ((_interactLayers.value & (1 << collider.gameObject.layer)) != 0)
        {
            CollectMoney();
        }
    }

    private void CollectMoney()
    {
        OnMoneyCollection?.Invoke();
        gameObject.SetActive(false);
    }
}
