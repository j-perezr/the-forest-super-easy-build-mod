using ModAPI.Attributes;
using UnityEngine;

namespace SuperEasyBuild
{
    public class Main: MonoBehaviour
    {
        public const string ModName = "SuperEasyBuild";
        public const string Version = "1.0.0";
        [ExecuteOnGameStart]
        private static void Init()
        {
            ModAPI.Log.Write($"[{ModName}:{Version}] Initialized");
        }
    }
}