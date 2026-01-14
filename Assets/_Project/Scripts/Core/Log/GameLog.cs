using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
/// 커스텀 로그 시스템
/// - [Conditional] 어트리뷰트로 릴리즈 빌드에서 완전 제거
/// - Settings에서 타입별 토글 가능
/// </summary>
public static class GameLog
{
    private const string LOG_SYMBOL = "ENABLE_GAME_LOG";

    private static GameLogSettings settings;
    private static bool isInitialized;

    /// <summary>
    /// 로그 시스템 초기화 (Awake에서 호출)
    /// </summary>
    public static void Initialize(GameLogSettings logSettings)
    {
        settings = logSettings;
        isInitialized = true;
    }

    /// <summary>
    /// 일반 로그 (Debug.Log 대체)
    /// </summary>
    [HideInCallstack]
    [Conditional(LOG_SYMBOL)]
    public static void Log(object message, Object context = null)
    {
        if (!CanLog()) return;
        if (!settings.EnableLog) return;

        var formattedMessage = FormatMessage("LOG", message);
        Debug.Log(formattedMessage, context);
    }

    /// <summary>
    /// 경고 로그 (Debug.LogWarning 대체)
    /// </summary>
    [HideInCallstack]
    [Conditional(LOG_SYMBOL)]
    public static void Warning(object message, Object context = null)
    {
        if (!CanLog()) return;
        if (!settings.EnableWarning) return;

        var formattedMessage = FormatMessage("WARNING", message);
        Debug.LogWarning(formattedMessage, context);
    }

    /// <summary>
    /// 에러 로그 (Debug.LogError 대체)
    /// </summary>
    [HideInCallstack]
    [Conditional(LOG_SYMBOL)]
    public static void Error(object message, Object context = null)
    {
        if (!CanLog()) return;
        if (!settings.EnableError) return;

        var formattedMessage = FormatMessage("ERROR", message);
        Debug.LogError(formattedMessage, context);
    }

    /// <summary>
    /// 카테고리가 있는 로그
    /// </summary>
    [HideInCallstack]
    [Conditional(LOG_SYMBOL)]
    public static void Log(string category, object message, Object context = null)
    {
        if (!CanLog()) return;
        if (!settings.EnableLog) return;

        var formattedMessage = FormatMessage(category, message);
        Debug.Log(formattedMessage, context);
    }

    /// <summary>
    /// 카테고리가 있는 경고 로그
    /// </summary>
    [HideInCallstack]
    [Conditional(LOG_SYMBOL)]
    public static void Warning(string category, object message, Object context = null)
    {
        if (!CanLog()) return;
        if (!settings.EnableWarning) return;

        var formattedMessage = FormatMessage(category, message);
        Debug.LogWarning(formattedMessage, context);
    }

    /// <summary>
    /// 카테고리가 있는 에러 로그
    /// </summary>
    [HideInCallstack]
    [Conditional(LOG_SYMBOL)]
    public static void Error(string category, object message, Object context = null)
    {
        if (!CanLog()) return;
        if (!settings.EnableError) return;

        var formattedMessage = FormatMessage(category, message);
        Debug.LogError(formattedMessage, context);
    }

    [HideInCallstack]
    private static bool CanLog()
    {
        if (!isInitialized || settings == null)
        {
            Debug.LogWarning("[GameLog] 초기화되지 않았습니다. GameLogInitializer를 씬에 추가하세요.");
            return false;
        }
        return true;
    }

    private static string FormatMessage(string prefix, object message)
    {
        var result = $"[{prefix}] {message}";

        if (settings != null && settings.ShowTimestamp)
        {
            result = $"[{System.DateTime.Now:HH:mm:ss.fff}] {result}";
        }

        return result;
    }
}
