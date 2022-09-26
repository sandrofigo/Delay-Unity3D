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

        private static IEnumerator WaitCoroutine(Func<bool> condition, Action action, float timeout = -1, bool ignoreTimeScale = false, bool skipEvaluationForFirstFrame = false)
        {
            if(skipEvaluationForFirstFrame)
                yield return null;

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
        public static Coroutine Create(float delay, Action action, bool ignoreTimeScale = false)
        {
            return CreateInternal(delay, action, ignoreTimeScale, 0);
        }

        private static Coroutine CreateInternal(float delay, Action action, bool ignoreTimeScale, int framesToSkip)
        {
            if (delay <= 0 && framesToSkip <= 0)
            {
                action.Invoke();
                return null;
            }

            return Instance.StartCoroutine(DelayCoroutine(delay, action, ignoreTimeScale, framesToSkip));
        }

        /// <summary>
        /// Skips a frame and then executes the given action.
        /// </summary>
        public static Coroutine SkipFrame(Action onNextFrame)
        {
            return CreateInternal(0, onNextFrame, true, 1);
        }

        /// <summary>
        /// Skips a number of frame and then executes the given action.
        /// </summary>
        public static Coroutine SkipFrames(int framesToSkip, Action onNextFrame)
        {
            return CreateInternal(0, onNextFrame, true, framesToSkip);
        }

        public static Coroutine WaitUntil(Func<bool> condition, Action action, float timeout = -1, bool ignoreTimeScale = false)
        {
            if(condition()) //TODO: unit test e.g. null
            {
                action.Invoke();
                return null;
            }

            return Instance.StartCoroutine(WaitCoroutine(condition, action, timeout, ignoreTimeScale, true));
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