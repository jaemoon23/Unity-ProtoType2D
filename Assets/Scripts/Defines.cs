namespace Game
{
    public static class Defines
    {
        public static class Tags
        {
            public const string Player = "Player";
        }

        /// <summary>
        /// TimeManager에서 사용하는 ID 상수
        /// </summary>
        public static class TimeId
        {
            // 시스템 (Priority 100)
            public const string Pause = "Pause";
            public const string Loading = "Loading";

            // UI/메뉴 (Priority 80)
            public const string Menu = "Menu";
            public const string Inventory = "Inventory";
            public const string Settings = "Settings";

            // 연출 (Priority 50)
            public const string Cutscene = "Cutscene";
            public const string Dialogue = "Dialogue";
            public const string Tutorial = "Tutorial";

            // 전투/액션 (Priority 30)
            public const string HitStop = "HitStop";
            public const string Parry = "Parry";
            public const string Counter = "Counter";
            public const string Finisher = "Finisher";

            // 효과 (Priority 10)
            public const string SlowMotion = "SlowMotion";
            public const string BulletTime = "BulletTime";
            public const string TimeWarp = "TimeWarp";

            // 스킬/능력 (Priority 20)
            public const string PlayerSkill = "PlayerSkill";
            public const string BossPhase = "BossPhase";
            public const string Ultimate = "Ultimate";
        }
    }
}
