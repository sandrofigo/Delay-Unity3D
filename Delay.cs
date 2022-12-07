using System;
using System.Collections;
using UnityEngine;

namespace Timing
{
    public sealed class Delay
    {
        private IEnumerator coroutine;

        private Delay()
        {
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
            if (skipEvaluationForFirstFrame)
                yield return null;

            float elapsedTime = 0;
            while (!condition())
            {
                if (timeout >= 0)
                {
                    if (ignoreTimeScale)
                        elapsedTime += Time.unscaledDeltaTime;
                    else
                        elapsedTime += Time.deltaTime;

                    if (elapsedTime >= timeout)
                        break;
                }

                yield return null;
            }

            action.Invoke();
        }

        private static Delay CreateInternal(float delay, Action action, bool ignoreTimeScale, int framesToSkip)
        {
            if (delay <= 0 && framesToSkip <= 0)
            {
                action.Invoke();
                return null;
            }

            var d = new Delay
            {
                coroutine = DelayCoroutine(delay, action, ignoreTimeScale, framesToSkip)
            };

            DelayMonoBehaviour.StartDelay(d.coroutine);

            return d;
        }

        /// <summary>
        /// Executes an action after a given delay.
        /// </summary>
        public static Delay Create(float delay, Action action, bool ignoreTimeScale = false)
        {
            return CreateInternal(delay, action, ignoreTimeScale, 0);
        }

        /// <summary>
        /// Skips a frame and then executes the given action.
        /// </summary>
        public static Delay SkipFrame(Action onNextFrame)
        {
            return CreateInternal(0, onNextFrame, true, 1);
        }

        /// <summary>
        /// Skips a number of frame and then executes the given action.
        /// </summary>
        public static Delay SkipFrames(int framesToSkip, Action onNextFrame)
        {
            return CreateInternal(0, onNextFrame, true, framesToSkip);
        }

        public static Delay WaitUntil(Func<bool> condition, Action action, float timeout = -1, bool ignoreTimeScale = false)
        {
            if (condition()) //TODO: unit test e.g. null
            {
                action.Invoke();
                return null;
            }

            var d = new Delay
            {
                coroutine = WaitCoroutine(condition, action, timeout, ignoreTimeScale, true)
            };

            if (!DelayMonoBehaviour.StartDelay(d.coroutine))
                return null;

            return d;
        }

        /// <summary>
        /// Cancels all ongoing delays.
        /// </summary>
        public static void StopAll()
        {
            DelayMonoBehaviour.Instance.StopAllCoroutines();
        }

        /// <summary>
        /// Stops the ongoing delay.
        /// </summary>
        public void Stop()
        {
            DelayMonoBehaviour.StopDelay(coroutine);
        }
    }

    internal sealed class DelayMonoBehaviour : MonoBehaviour
    {
        private static DelayMonoBehaviour instance;

        private static bool applicationIsQuitting;

        internal static DelayMonoBehaviour Instance
        {
            get
            {
                if (applicationIsQuitting)
                {
                    return null;
                }

                if (instance == null)
                {
                    var obj = new GameObject("Delay").AddComponent<DelayMonoBehaviour>();
                    DontDestroyOnLoad(obj);
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

        public static bool StartDelay(IEnumerator coroutine)
        {
            if (Instance == null)
                return false;

            Instance.StartDelayInternal(coroutine);

            return true;
        }

        private void StartDelayInternal(IEnumerator coroutine)
        {
            if (applicationIsQuitting)
                return;

            StartCoroutine(coroutine);
        }

        public static bool StopDelay(IEnumerator coroutine)
        {
            if (Instance == null)
                return false;

            Instance.StopDelayInternal(coroutine);

            return true;
        }

        private void StopDelayInternal(IEnumerator coroutine)
        {
            if (applicationIsQuitting)
                return;

            StopCoroutine(coroutine);
        }

        private void OnDestroy()
        {
            applicationIsQuitting = true;
            StopAllCoroutines();
        }
    }
}