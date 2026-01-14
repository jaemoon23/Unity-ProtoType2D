using UnityEngine;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 씬 로딩 및 로딩 UI 관리
/// 부트씬에서 가장 먼저 초기화되어야 함
/// </summary>
public class LoadingManager : MonoBehaviour
{
    #region Singleton
    private static LoadingManager instance;
    public static LoadingManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameLog.Error("LoadingManager", "인스턴스가 존재하지 않습니다. BootScene에서 먼저 초기화해주세요.");
            }
            return instance;
        }
    }
    #endregion

    #region Serialized Fields
    [Header("로딩 UI")]
    [SerializeField] private LoadingUI loadingUI;
    #endregion

    #region Properties
    public bool IsInitialized { get; private set; }
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        IsInitialized = true;

        GameLog.Log("LoadingManager", "초기화 완료");
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
    #endregion

    #region Public Methods - Progress
    /// <summary>
    /// 로딩 UI 표시 및 진행률 초기화
    /// </summary>
    public void ShowLoading()
    {
        if (loadingUI != null)
        {
            loadingUI.Show();
            loadingUI.SetLoadingAnimation(true);
        }
    }

    /// <summary>
    /// 로딩 UI 숨김
    /// </summary>
    public void HideLoading()
    {
        if (loadingUI != null)
        {
            loadingUI.SetLoadingAnimation(false);
            loadingUI.Hide();
        }
    }

    /// <summary>
    /// 진행률 업데이트 (0 ~ 1)
    /// </summary>
    public void UpdateProgress(float progress)
    {
        if (loadingUI != null)
        {
            loadingUI.SetProgress(progress);
        }
    }
    #endregion

    #region Public Methods - Scene Loading
    /// <summary>
    /// 씬 로드 (페이드 인/아웃 포함)
    /// </summary>
    /// <param name="sceneName">로드할 씬 이름</param>
    /// <param name="showLoadingUI">로딩 UI 표시 여부</param>
    /// <param name="cancellationToken">취소 토큰</param>
    public async UniTask LoadSceneAsync(string sceneName, bool showLoadingUI = true, CancellationToken cancellationToken = default)
    {
        // 토큰이 제공되지 않으면 오브젝트 파괴 시 자동 취소되는 토큰 사용
        var token = cancellationToken == default
            ? this.GetCancellationTokenOnDestroy()
            : cancellationToken;

        GameLog.Log("LoadingManager", $"씬 로드 시작: {sceneName}");

        // 페이드 아웃
        if (loadingUI != null)
        {
            await loadingUI.FadeOutAsync(token);

            if (showLoadingUI)
            {
                ShowLoading();
                UpdateProgress(0f);
            }
            else
            {
                // 로딩 UI 안 보여줘도 페이드 패널은 비활성화해야 가리지 않음
                loadingUI.HideFadePanel();
            }
        }

        // 씬 비동기 로드
        var operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        // 로딩 진행률 업데이트 (0.9까지만 진행됨)
        while (operation.progress < 0.9f)
        {
            if (showLoadingUI)
            {
                UpdateProgress(operation.progress / 0.9f);
            }
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        // 로딩 완료
        if (showLoadingUI)
        {
            UpdateProgress(1f);
        }

        // 씬 활성화
        operation.allowSceneActivation = true;

        // 씬 활성화 대기
        await UniTask.WaitUntil(() => operation.isDone, cancellationToken: token);

        // 로딩 UI 숨김 및 페이드 인
        if (loadingUI != null)
        {
            if (showLoadingUI)
            {
                HideLoading();
            }
            await loadingUI.FadeInAsync(token);
        }

        GameLog.Log("LoadingManager", $"씬 로드 완료: {sceneName}");
    }

    /// <summary>
    /// 씬 로드 (SceneName enum 사용)
    /// </summary>
    public async UniTask LoadSceneAsync(SceneName sceneName, bool showLoadingUI = true, CancellationToken cancellationToken = default)
    {
        string sceneNameStr = GetSceneNameString(sceneName);
        await LoadSceneAsync(sceneNameStr, showLoadingUI, cancellationToken);
    }
    #endregion

    #region Private Methods
    private string GetSceneNameString(SceneName sceneName)
    {
        return sceneName switch
        {
            SceneName.Boot => SceneNames.Boot,
            SceneName.Title => SceneNames.Title,
            SceneName.Game => SceneNames.Game,
            _ => sceneName.ToString()
        };
    }
    #endregion
}
