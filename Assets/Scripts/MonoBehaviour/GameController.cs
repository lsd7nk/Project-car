using UnityEngine;

public class GameController : MonoBehaviour
{
    private InteractorsBase _interactors;
    private RepositoriesBase _repositories;
    [SerializeField] private CameraFollow _cameraFollow;

    public InteractorsBase InteractorsBase => _interactors;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()  // initialize repositories & interactors
    {
        _interactors = new InteractorsBase();
        _repositories = new RepositoriesBase();

        _interactors.Initialize();
        _repositories.AddRepository<BankRepository>(_interactors.GetInteractor<BankInteractor>());

        ComponentsInitialize();
    }

    private void ComponentsInitialize()
    {
        CoinsController coinsController = GetComponent<CoinsController>();
        LapsController lapsController = GetComponent<LapsController>();

        coinsController?.Initialize();
        lapsController?.Initialize();
        _cameraFollow?.Initialize();
    }
}