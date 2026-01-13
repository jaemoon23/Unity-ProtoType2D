using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.Collections.Generic;

namespace Game.Editor
{
    /// <summary>
    /// 빌드 시 ENABLE_GAME_LOG 심볼 자동 제거/복원
    /// </summary>
    public class BuildPreprocessor : IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        private const string LOG_SYMBOL = "ENABLE_GAME_LOG";
        private static Dictionary<NamedBuildTarget, string> originalSymbols = new();

        public int callbackOrder => 0;

        /// <summary>
        /// 빌드 전: 심볼 제거
        /// </summary>
        public void OnPreprocessBuild(BuildReport report)
        {
            var namedTarget = NamedBuildTarget.FromBuildTargetGroup(report.summary.platformGroup);

            // 현재 심볼 저장
            var symbols = PlayerSettings.GetScriptingDefineSymbols(namedTarget);
            originalSymbols[namedTarget] = symbols;

            // ENABLE_GAME_LOG 제거
            var symbolList = new List<string>(symbols.Split(';'));
            if (symbolList.Remove(LOG_SYMBOL))
            {
                var newSymbols = string.Join(";", symbolList);
                PlayerSettings.SetScriptingDefineSymbols(namedTarget, newSymbols);
                UnityEngine.Debug.Log($"[BuildPreprocessor] 빌드용 로그 심볼 제거됨: {LOG_SYMBOL}");
            }
        }

        /// <summary>
        /// 빌드 후: 심볼 복원
        /// </summary>
        public void OnPostprocessBuild(BuildReport report)
        {
            var namedTarget = NamedBuildTarget.FromBuildTargetGroup(report.summary.platformGroup);

            if (originalSymbols.TryGetValue(namedTarget, out var symbols))
            {
                PlayerSettings.SetScriptingDefineSymbols(namedTarget, symbols);
                originalSymbols.Remove(namedTarget);
                UnityEngine.Debug.Log($"[BuildPreprocessor] 로그 심볼 복원됨: {LOG_SYMBOL}");
            }
        }
    }
}
