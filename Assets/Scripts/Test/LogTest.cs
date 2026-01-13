using Game.Core.Log;
using UnityEngine;

namespace Game
{
    public class LogTest : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            GameLog.Log("로그 테스트", "LogTest", gameObject);
            GameLog.Warning("경고 테스트", "WarningTest", gameObject);
            GameLog.Error("에러 테스트", "ErrorTest", gameObject);
        }
    }
}
