using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

/// <summary>
/// 부트씬 초기화 담당
/// 매니저들을 병렬로 초기화하고 완료 후 타이틀씬으로 전환
/// </summary>
public class BootLoader : MonoBehaviour
{
    #region Constants
    private const float PROGRESS_UPDATE_INTERVAL = 0.05f; // 50ms마다 업데이트
    #endregion

    #region Serialized Fields
    [Header("매니저 컨테이너")]
    [SerializeField] private Transform managersContainer;

    [Header("설정")]
    [SerializeField] private float minimumLoadingTime = 1f;
    #endregion

    #region Private Fields
    private int completedCount;
    private int totalCount;
    #endregion

    #region Unity Lifecycle
    private async void Start()
    {
        await InitializeAsync();
    }
    #endregion

    #region Private Methods
    private async UniTask InitializeAsync()
    {
        GameLog.Log("BootLoader", "부트 시퀀스 시작");

        // LoadingManager가 먼저 준비되어 있어야 함 (Awake에서 초기화됨)
        if (LoadingManager.Instance == null || !LoadingManager.Instance.IsInitialized)
        {
            GameLog.Error("BootLoader", "LoadingManager가 초기화되지 않았습니다.");
            return;
        }

        // 로딩 UI 표시
        LoadingManager.Instance.ShowLoading();
        LoadingManager.Instance.UpdateProgress(0f);

        // 매니저 컨테이너를 씬 전환에서 유지
        if (managersContainer != null)
        {
            DontDestroyOnLoad(managersContainer.gameObject);
            GameLog.Log("BootLoader", "매니저 컨테이너 DontDestroyOnLoad 설정 완료");
        }

        // 자식에서 IInitializable 구현체 자동 수집
        var initializables = managersContainer != null
            ? managersContainer.GetComponentsInChildren<IInitializable>()
            : GetComponentsInChildren<IInitializable>();

        var initTasks = new List<UniTask>();
        // 매니저 수 + 최소 로딩 시간 1개 = 총 진행 단계
        totalCount = initializables.Length + 1;
        completedCount = 0;

        foreach (var initializable in initializables)
        {
            var mono = initializable as MonoBehaviour;
            string managerName = mono != null ? mono.GetType().Name : "Unknown";
            initTasks.Add(InitializeWithProgressAsync(initializable, managerName));
        }

        // 최소 로딩 시간도 진행률의 일부로 포함
        initTasks.Add(MinimumLoadingTimeAsync());

        GameLog.Log("BootLoader", $"총 {initializables.Length}개의 매니저 + 최소 로딩 시간 초기화 시작");

        // 모든 태스크 병렬 실행 (매니저 초기화 + 최소 로딩 시간)
        await UniTask.WhenAll(initTasks);

        GameLog.Log("BootLoader", "모든 매니저 초기화 완료");

        // 로딩 UI 숨김
        LoadingManager.Instance.HideLoading();

        // 타이틀씬으로 전환
        await LoadingManager.Instance.LoadSceneAsync(SceneNames.Title, showLoadingUI: false);
    }

    private async UniTask InitializeWithProgressAsync(IInitializable initializable, string managerName)
    {
        try
        {
            GameLog.Log("BootLoader", $"{managerName} 초기화 시작");

            await initializable.InitializeAsync();

            completedCount++;
            float progress = (float)completedCount / totalCount;
            LoadingManager.Instance.UpdateProgress(progress);

            GameLog.Log("BootLoader", $"{managerName} 초기화 완료 ({completedCount}/{totalCount})");
        }
        catch (System.Exception ex)
        {
            GameLog.Error("BootLoader", $"{managerName} 초기화 실패: {ex.Message}");
            throw;
        }
    }

    private async UniTask MinimumLoadingTimeAsync()
    {
        GameLog.Log("BootLoader", $"최소 로딩 시간 {minimumLoadingTime}초 대기 시작");

        // 최소 로딩 시간 동안 점진적으로 진행률 업데이트
        float elapsed = 0f;

        while (elapsed < minimumLoadingTime)
        {
            await UniTask.Delay((int)(PROGRESS_UPDATE_INTERVAL * 1000));
            elapsed += PROGRESS_UPDATE_INTERVAL;

            // 시간 기반 진행률 (0 ~ 1)
            float timeProgress = Mathf.Clamp01(elapsed / minimumLoadingTime);

            // 매니저 진행률 + 시간 진행률 합산
            // 매니저: completedCount / totalCount 중 (totalCount - 1) 부분
            // 시간: 1 / totalCount 부분을 timeProgress만큼
            float managerProgress = (float)completedCount / totalCount;
            float timeContribution = timeProgress / totalCount;
            float totalProgress = managerProgress + timeContribution;

            LoadingManager.Instance.UpdateProgress(totalProgress);
        }

        completedCount++;
        float progress = (float)completedCount / totalCount;
        LoadingManager.Instance.UpdateProgress(progress);

        GameLog.Log("BootLoader", $"최소 로딩 시간 완료 ({completedCount}/{totalCount})");
    }
    #endregion
}
