using UnityEngine;

namespace Game.Core.Log
{
    /// <summary>
    /// GameLog 시스템 초기화
    /// </summary>
    [DefaultExecutionOrder(-1000)]
    public class GameLogInitializer : MonoBehaviour
    {
        public static GameLogInitializer Instance { get; private set; }

        [SerializeField] private GameLogSettings settings;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (settings == null)
            {
                Debug.LogError("[GameLogInitializer] Settings가 연결되지 않았습니다.");
                return;
            }

            GameLog.Initialize(settings);
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }
    }
}
