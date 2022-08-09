using UnityEngine;

public class GameController : MonoBehaviour
{
    private InteractorsBase _interactors;
    private RepositoriesBase _repositories;
    private CameraFollowTo _cameraFollow;

    public static GameController Instance { get; private set; }
    public InteractorsBase InteractorsBase => _interactors;
    [field: SerializeField] public GameObject TrainingCanvas { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else if (Instance == this) { Destroy(gameObject); }

        Initialize();
    }

    private void Initialize()
    {
        _interactors = new InteractorsBase();
        _repositories = new RepositoriesBase();
        _cameraFollow = Camera.main.GetComponent<CameraFollowTo>();

        _interactors.Initialize();
        AddRepositories();

        ComponentsInitialize();
    }

    private void AddRepositories()
    {
        _repositories.AddRepository<BankRepository>(_interactors.GetInteractor<BankInteractor>());
        _repositories.AddRepository<FallsRepository>(_interactors.GetInteractor<FallsInteractor>());
    }

    private void ComponentsInitialize()
    {
        CoinsController coinsController = GetComponent<CoinsController>();
        LapsController lapsController = GetComponent<LapsController>();
        FallsController fallsController = GetComponent<FallsController>();

        coinsController?.Initialize();
        lapsController?.Initialize();
        fallsController?.Initialize();
        _cameraFollow?.Initialize();
    }
}