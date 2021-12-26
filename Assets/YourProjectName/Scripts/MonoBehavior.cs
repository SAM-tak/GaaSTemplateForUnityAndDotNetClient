using System.Diagnostics;
using CustomUnity;

namespace YourProjectName
{
    public class MonoBehaviour : CustomUnity.MonoBehaviour
    {
        /// <summary>
        /// イベント関数用
        /// </summary>
        /// <param name="message"></param>
        public void DebugLog(string message)
        {
            LogInfo(message);
        }

        /// <summary>
        /// イベント関数用
        /// </summary>
        public void DebugBreak()
        {
#if(UNITY_EDITOR || DEVELOPMENT_BUILD)
            UnityEngine.Debug.Log("DebugBreak", this);
            UnityEngine.Debug.Break();
#endif
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        protected void Profiling(string memberName)
        {
            ProfileSampler.Begin(this, memberName, 0);
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        protected void Profiling(string memberName, int id)
        {
            ProfileSampler.EndAndBegin(this, memberName, id);
        }

        protected ProfileSampler NewProfiling(string memberName, int id = 0)
        {
            return ProfileSampler.Create(this, memberName, id);
        }
    }
}
