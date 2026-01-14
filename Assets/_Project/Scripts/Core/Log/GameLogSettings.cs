using UnityEngine;

/// <summary>
/// 로그 시스템 설정
/// </summary>
[CreateAssetMenu(fileName = "GameLogSettings", menuName = "Game/Log Settings")]
public class GameLogSettings : ScriptableObject
{
    [Header("로그 타입별 활성화")]
    [SerializeField] private bool enableLog = true;
    [SerializeField] private bool enableWarning = true;
    [SerializeField] private bool enableError = true;

    [Header("추가 옵션")]
    [SerializeField] private bool showTimestamp = false;

    public bool EnableLog => enableLog;
    public bool EnableWarning => enableWarning;
    public bool EnableError => enableError;
    public bool ShowTimestamp => showTimestamp;
}
