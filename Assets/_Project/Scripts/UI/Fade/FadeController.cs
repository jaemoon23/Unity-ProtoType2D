using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 화면 페이드 인/아웃 전용 컨트롤러
/// 씬 전환, 연출 등 다양한 곳에서 재사용 가능
/// </summary>
public class FadeController : MonoBehaviour
{
    private const string PREFIX = "FadeController";
    #region Singleton
    private static FadeController instance;
    public static FadeController Instance
    {
        get
        {
            if (instance == null)
            {
                GameLog.Error(PREFIX, "인스턴스가 존재하지 않습니다. FadeCanvas가 씬에 있는지 확인해주세요.");
            }
            return instance;
        }
    }
    #endregion

    #region Constants
    private const float DEFAULT_FADE_DURATION = 0.5f;
    #endregion

    #region Serialized Fields
    [Header("페이드 패널")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    [Header("설정")]
    [SerializeField] private float fadeDuration = DEFAULT_FADE_DURATION;
    #endregion

    #region Properties
    /// <summary>
    /// 현재 페이드 중인지 여부
    /// </summary>
    public bool IsFading { get; private set; }

    /// <summary>
    /// 현재 페이드 알파 값
    /// </summary>
    public float CurrentAlpha => fadeCanvasGroup != null ? fadeCanvasGroup.alpha : 0f;
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

        // 초기 상태: 투명 (평소에는 안 보이다가 페이드 호출 시에만 작동)
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.gameObject.SetActive(false);
        }

        GameLog.Log(PREFIX, "초기화 완료");
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// 페이드 인 (화면 밝아짐, 검은 화면 -> 투명)
    /// </summary>
    public async UniTask FadeInAsync(CancellationToken cancellationToken = default)
    {
        await FadeInAsync(fadeDuration, cancellationToken);
    }

    /// <summary>
    /// 페이드 인 (커스텀 지속시간)
    /// </summary>
    public async UniTask FadeInAsync(float duration, CancellationToken cancellationToken = default)
    {
        if (fadeCanvasGroup == null) return;

        IsFading = true;
        fadeCanvasGroup.gameObject.SetActive(true);

        float startAlpha = fadeCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            cancellationToken.ThrowIfCancellationRequested();

            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t);

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.gameObject.SetActive(false);
        IsFading = false;

        GameLog.Log(PREFIX, "페이드 인 완료");
    }

    /// <summary>
    /// 페이드 아웃 (화면 어두워짐, 투명 -> 검은 화면)
    /// </summary>
    public async UniTask FadeOutAsync(CancellationToken cancellationToken = default)
    {
        await FadeOutAsync(fadeDuration, cancellationToken);
    }

    /// <summary>
    /// 페이드 아웃 (커스텀 지속시간)
    /// </summary>
    public async UniTask FadeOutAsync(float duration, CancellationToken cancellationToken = default)
    {
        if (fadeCanvasGroup == null) return;

        IsFading = true;
        fadeCanvasGroup.gameObject.SetActive(true);

        float startAlpha = fadeCanvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            cancellationToken.ThrowIfCancellationRequested();

            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, t);

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        fadeCanvasGroup.alpha = 1f;
        IsFading = false;

        GameLog.Log(PREFIX, "페이드 아웃 완료");
    }

    /// <summary>
    /// 즉시 검은 화면으로 전환 (애니메이션 없음)
    /// </summary>
    public void SetBlack()
    {
        if (fadeCanvasGroup == null) return;

        fadeCanvasGroup.gameObject.SetActive(true);
        fadeCanvasGroup.alpha = 1f;
    }

    /// <summary>
    /// 즉시 투명으로 전환 (애니메이션 없음)
    /// </summary>
    public void SetClear()
    {
        if (fadeCanvasGroup == null) return;

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.gameObject.SetActive(false);
    }
    #endregion
}
