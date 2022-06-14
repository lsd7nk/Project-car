using UnityEngine;

public class Controller : MonoBehaviour
{
    protected T Initialize<T>() where T : Interactor
    {
        return GetComponent<GameController>().InteractorsBase.GetInteractor<T>();
    }
}
