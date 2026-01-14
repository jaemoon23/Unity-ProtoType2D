using System;

/// <summary>
/// 시간 조작 요청 데이터
/// </summary>
public class TimeRequest
{
    public string Id { get; }
    public float TimeScale { get; }
    public int Priority { get; }
    public DateTime CreatedAt { get; }

    public TimeRequest(string id, float timeScale, int priority = 0)
    {
        Id = id;
        TimeScale = timeScale;
        Priority = priority;
        CreatedAt = DateTime.Now;
    }
}
