using UnityEngine;
using ProjectCar.Interactors;

namespace ProjectCar
{
    namespace Controllers
    {
        public class Controller : MonoBehaviour
        {
            private protected T Initialize<T>() where T : Interactor
            {
                return GameController.Instance.InteractorsBase.GetInteractor<T>();
            }
        }
    }
}
