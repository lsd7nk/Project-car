using UnityEngine;

public class GameController : MonoBehaviour
{
    private InteractorsBase _interactors;
    private RepositoriesBase _repositories;
    [SerializeField] private CameraFollow _cameraFollow;
    [SerializeField] private FallHandler _fallHandler;
    
    public static GameController Instance { get; private set; }
    public InteractorsBase InteractorsBase => _interactors;

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else if (Instance == this) { Destroy(gameObject); }

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
        _fallHandler?.Initialize();
    }
}