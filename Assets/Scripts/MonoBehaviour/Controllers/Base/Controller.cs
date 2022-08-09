using UnityEngine;

public class Controller : MonoBehaviour
{
    protected T Initialize<T>() where T : Interactor
    {
        return GameController.Instance.InteractorsBase.GetInteractor<T>();
    }
}
