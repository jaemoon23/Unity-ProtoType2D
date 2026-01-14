using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// GameLog 시스템 초기화
/// </summary>
[DefaultExecutionOrder(-1000)]
public class GameLogInitializer : MonoBehaviour, IInitializable
{
    public static GameLogInitializer Instance { get; private set; }

    [SerializeField] private GameLogSettings settings;

    public bool IsInitialized { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // GameLog는 다른 시스템보다 먼저 초기화되어야 하므로 Awake에서 처리
        if (settings == null)
        {
            Debug.LogError("[GameLogInitializer] Settings가 연결되지 않았습니다.");
            return;
        }

        GameLog.Initialize(settings);
    }

    public async UniTask InitializeAsync()
    {
        // GameLog는 이미 Awake에서 초기화됨
        // 추후 비동기 초기화 작업이 필요하면 여기에 추가
        await UniTask.CompletedTask;

        IsInitialized = true;
        GameLog.Log("GameLogInitializer", "초기화 완료");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
