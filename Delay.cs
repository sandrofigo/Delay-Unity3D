using System;
using System.Collections;
using UnityEngine;

namespace Timing
{
    public class Delay : MonoBehaviour
    {
        private static Delay instance;

        private static Delay Instance
        {
            get
            {
                if (instance == null)
                {
                    new GameObject("Delay").AddComponent<Delay>();
                }

                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private static IEnumerator DelayCoroutine(float delay, Action action, bool ignoreTimeScale, int framesToSkip)
        {
            while (framesToSkip > 1)
            {
                framesToSkip--;
                yield return null;
            }

            if (ignoreTimeScale)
                yield return new WaitForSecondsRealtime(delay);
            else
                yield return new WaitForSeconds(delay);

            action.Invoke();
        }

        private static IEnumerator WaitCoroutine(Func<bool> condition, Action action, float timeout = -1, bool ignoreTimeScale = false)
        {
            float elapsedTime = 0;
            while (!condition())
            {
                if (timeout >= 0)
                {
                    if (ignoreTimeScale)
                        elapsedTime += UnityEngine.Time.unscaledDeltaTime;
                    else
                        elapsedTime += UnityEngine.Time.deltaTime;

                    if (elapsedTime >= timeout)
                        break;
                }

                yield return null;
            }

            action.Invoke();
        }

        /// <summary>
        /// Executes an action after a given delay.
        /// </summary>
        public static void Create(float delay, Action action, bool ignoreTimeScale = false)
        {
            CreateInternal(delay, action, ignoreTimeScale, 0);
        }

        private static void CreateInternal(float delay, Action action, bool ignoreTimeScale, int framesToSkip)
        {
            if (delay == 0 && framesToSkip <= 0)
            {
                action.Invoke();
                return;
            }

            Instance.StartCoroutine(DelayCoroutine(delay, action, ignoreTimeScale, framesToSkip));
        }

        /// <summary>
        /// Skips a frame and then executes the given action.
        /// </summary>
        public static void SkipFrame(Action onNextFrame)
        {
            CreateInternal(0, onNextFrame, true, 1);
        }

        /// <summary>
        /// Skips a number of frame and then executes the given action.
        /// </summary>
        public static void SkipFrames(int framesToSkip, Action onNextFrame)
        {
            CreateInternal(0, onNextFrame, true, framesToSkip);
        }

        public static void WaitUntil(Func<bool> condition, Action action, float timeout = -1, bool ignoreTimeScale = false)
        {
            Instance.StartCoroutine(WaitCoroutine(condition, action, timeout, ignoreTimeScale));
        }

        /// <summary>
        /// Cancels all ongoing delays.
        /// </summary>
        public static void CancelAll()
        {
            Instance.StopAllCoroutines();
        }
    }
}