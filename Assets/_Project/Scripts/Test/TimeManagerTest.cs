using UnityEngine;

public class TimeManagerTest : MonoBehaviour
{
    [Header("Pause")]
    [SerializeField] private bool pause;

    [Header("SlowMotion")]
    [SerializeField] private bool slowMotion;
    [SerializeField] private float slowMotionScale = 0.3f;

    [Header("HitStop")]
    [SerializeField] private bool hitStop;

    [Header("Debug Info")]
    [SerializeField] private float currentTimeScale;
    [SerializeField] private int stackCount;

    private bool prevPause;
    private bool prevSlowMotion;
    private bool prevHitStop;

    private void Update()
    {
        // Pause 토글
        if (pause != prevPause)
        {
            if (pause)
            {
                TimeManager.Instance.Push(TimeId.Pause, 0f, 100);
                GameLog.Log("TimeManagerTest", "일시정지 활성화");
            }
            else
            {
                TimeManager.Instance.Remove(TimeId.Pause);
                GameLog.Log("TimeManagerTest", "일시정지 해제");
            }
            prevPause = pause;
        }

        // SlowMotion 토글
        if (slowMotion != prevSlowMotion)
        {
            if (slowMotion)
            {
                TimeManager.Instance.Push(TimeId.SlowMotion, slowMotionScale, 10);
                GameLog.Log("TimeManagerTest", $"슬로우모션 활성화 (배율: {slowMotionScale})");
            }
            else
            {
                TimeManager.Instance.Remove(TimeId.SlowMotion);
                GameLog.Log("TimeManagerTest", "슬로우모션 해제");
            }
            prevSlowMotion = slowMotion;
        }

        // HitStop 토글
        if (hitStop != prevHitStop)
        {
            if (hitStop)
            {
                TimeManager.Instance.Push(TimeId.HitStop, 0f, 30);
                GameLog.Log("TimeManagerTest", "히트스탑 활성화");
            }
            else
            {
                TimeManager.Instance.Remove(TimeId.HitStop);
                GameLog.Log("TimeManagerTest", "히트스탑 해제");
            }
            prevHitStop = hitStop;
        }

        // Debug Info 업데이트
        currentTimeScale = TimeManager.Instance.CurrentTimeScale;
        stackCount = TimeManager.Instance.StackCount;
    }

    private void OnDisable()
    {
        // 비활성화 시 모든 요청 제거
        if (pause) TimeManager.Instance.Remove(TimeId.Pause);
        if (slowMotion) TimeManager.Instance.Remove(TimeId.SlowMotion);
        if (hitStop) TimeManager.Instance.Remove(TimeId.HitStop);
    }
}
