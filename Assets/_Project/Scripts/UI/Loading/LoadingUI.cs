using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;

/// <summary>
/// 로딩 UI 컴포넌트
/// 진행률 슬라이더, 텍스트, 로딩 애니메이션, 페이드 패널 관리
/// </summary>
public class LoadingUI : MonoBehaviour
{
    #region Serialized Fields
    [Header("페이드")]
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    [Header("로딩 진행률")]
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text progressText;

    [Header("로딩 애니메이션")]
    [SerializeField] private GameObject loadingAnimation;

    [Header("설정")]
    [SerializeField] private float fadeDuration = 0.5f;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Hide();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// 로딩 UI 표시
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);

        // 페이드 패널 비활성화 (로딩 중에는 안 보이게)
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.gameObject.SetActive(false);
        }

        SetProgress(0f);
    }

    /// <summary>
    /// 로딩 UI 숨김
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 진행률 업데이트 (0 ~ 1)
    /// </summary>
    public void SetProgress(float progress)
    {
        progress = Mathf.Clamp01(progress);

        if (progressSlider != null)
        {
            progressSlider.value = progress;
        }

        if (progressText != null)
        {
            int percentage = Mathf.RoundToInt(progress * 100f);
            progressText.text = $"{percentage}%";
        }
    }

    /// <summary>
    /// 로딩 애니메이션 활성화/비활성화
    /// </summary>
    public void SetLoadingAnimation(bool active)
    {
        if (loadingAnimation != null)
        {
            loadingAnimation.SetActive(active);
        }
    }

    /// <summary>
    /// 페이드 패널만 비활성화
    /// </summary>
    public void HideFadePanel()
    {
        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 페이드 인 (화면 밝아짐)
    /// </summary>
    public async UniTask FadeInAsync(CancellationToken cancellationToken = default)
    {
        if (fadeCanvasGroup == null) return;

        fadeCanvasGroup.gameObject.SetActive(true);
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = 1f - (elapsed / fadeDuration);
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.gameObject.SetActive(false);
    }

    /// <summary>
    /// 페이드 아웃 (화면 어두워짐)
    /// </summary>
    public async UniTask FadeOutAsync(CancellationToken cancellationToken = default)
    {
        if (fadeCanvasGroup == null) return;

        fadeCanvasGroup.gameObject.SetActive(true);
        fadeCanvasGroup.alpha = 0f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = elapsed / fadeDuration;
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        fadeCanvasGroup.alpha = 1f;
    }
    #endregion
}
