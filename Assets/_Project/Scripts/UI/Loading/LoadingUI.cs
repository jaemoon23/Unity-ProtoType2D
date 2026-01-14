using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 로딩 UI 컴포넌트
/// 진행률 슬라이더, 텍스트, 로딩 애니메이션 관리
/// 페이드는 FadeController에서 별도 관리
/// </summary>
public class LoadingUI : MonoBehaviour
{
    #region Serialized Fields
    [Header("로딩 진행률")]
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text progressText;

    [Header("로딩 애니메이션")]
    [SerializeField] private GameObject loadingAnimation;
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
    #endregion
}
