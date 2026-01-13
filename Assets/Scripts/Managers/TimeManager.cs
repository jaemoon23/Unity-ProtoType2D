using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.Core.Log;
using Game.Data;

namespace Game.Managers
{
    /// <summary>
    /// 스택 기반 시간 관리자
    /// 여러 시스템이 동시에 TimeScale을 요청할 때 충돌 없이 관리
    /// </summary>
    public class TimeManager : MonoBehaviour
    {
        public static TimeManager Instance { get; private set; }

        [Header("Debug")]
        [SerializeField] private bool showDebugLog = false;

        private readonly List<TimeRequest> timeStack = new();
        private float defaultTimeScale = 1f;

        public float CurrentTimeScale => Time.timeScale;
        public int StackCount => timeStack.Count;
        public IReadOnlyList<TimeRequest> TimeStack => timeStack.AsReadOnly();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            defaultTimeScale = Time.timeScale;
        }

        /// <summary>
        /// 시간 조작 요청을 스택에 추가
        /// </summary>
        /// <param name="id">고유 식별자 (제거 시 사용)</param>
        /// <param name="timeScale">적용할 TimeScale (0 = 정지, 1 = 정상, 0.5 = 슬로우모션)</param>
        /// <param name="priority">우선순위 (높을수록 우선 적용)</param>
        /// <returns>성공 여부</returns>
        public bool Push(string id, float timeScale, int priority = 0)
        {
            if (string.IsNullOrEmpty(id))
            {
                GameLog.Warning("TimeManager", "ID가 비어있습니다.");
                return false;
            }

            if (timeStack.Any(r => r.Id == id))
            {
                GameLog.Warning("TimeManager", $"이미 존재하는 ID: {id}");
                return false;
            }

            var request = new TimeRequest(id, timeScale, priority);
            timeStack.Add(request);

            ApplyTimeScale();

            if (showDebugLog)
            {
                GameLog.Log("TimeManager", $"Push - ID: {id}, TimeScale: {timeScale}, Priority: {priority}");
            }

            return true;
        }

        /// <summary>
        /// 가장 최근 요청을 제거
        /// </summary>
        /// <returns>제거된 요청의 ID (없으면 null)</returns>
        public string Pop()
        {
            if (timeStack.Count == 0)
            {
                GameLog.Warning("TimeManager", "스택이 비어있습니다.");
                return null;
            }

            var last = timeStack[^1];
            timeStack.RemoveAt(timeStack.Count - 1);

            ApplyTimeScale();

            if (showDebugLog)
            {
                GameLog.Log("TimeManager", $"Pop - ID: {last.Id}");
            }

            return last.Id;
        }

        /// <summary>
        /// 특정 ID의 요청을 제거 (순서 무관)
        /// </summary>
        /// <param name="id">제거할 요청의 ID</param>
        /// <returns>성공 여부</returns>
        public bool Remove(string id)
        {
            var request = timeStack.FirstOrDefault(r => r.Id == id);

            if (request == null)
            {
                GameLog.Warning("TimeManager", $"존재하지 않는 ID: {id}");
                return false;
            }

            timeStack.Remove(request);
            ApplyTimeScale();

            if (showDebugLog)
            {
                GameLog.Log("TimeManager", $"Remove - ID: {id}");
            }

            return true;
        }

        /// <summary>
        /// 모든 요청을 제거하고 기본 TimeScale로 복원
        /// </summary>
        public void Clear()
        {
            timeStack.Clear();
            Time.timeScale = defaultTimeScale;

            if (showDebugLog)
            {
                GameLog.Log("TimeManager", "Clear - 모든 요청 제거됨");
            }
        }

        /// <summary>
        /// 특정 ID의 요청이 존재하는지 확인
        /// </summary>
        public bool Contains(string id)
        {
            return timeStack.Any(r => r.Id == id);
        }

        /// <summary>
        /// 우선순위가 가장 높은 요청의 TimeScale 적용
        /// 같은 우선순위면 나중에 추가된 것 적용
        /// </summary>
        private void ApplyTimeScale()
        {
            if (timeStack.Count == 0)
            {
                Time.timeScale = defaultTimeScale;
                return;
            }

            var activeRequest = timeStack
                .OrderByDescending(r => r.Priority)
                .ThenByDescending(r => r.CreatedAt)
                .First();

            Time.timeScale = activeRequest.TimeScale;

            if (showDebugLog)
            {
                GameLog.Log("TimeManager", $"Applied - ID: {activeRequest.Id}, TimeScale: {activeRequest.TimeScale}");
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Time.timeScale = defaultTimeScale;
                Instance = null;
            }
        }
    }
}
