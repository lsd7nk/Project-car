using UnityEngine;

public class GameController : MonoBehaviour
{
    private InteractorsBase _interactors;
    private RepositoriesBase _repositories;

    private void Awake()
    {
        InitializeBase();
    }

    private void InitializeBase()
    {
        _interactors = new InteractorsBase();
        _repositories = new RepositoriesBase();

        _interactors.Initialize();
        _repositories.AddRepository<BankRepository>(_interactors.GetInteractor<BankInteractor>());
    }
}