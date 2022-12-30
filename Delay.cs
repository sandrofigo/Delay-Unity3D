using System;
using System.Collections;
using UnityEngine;

namespace Timing
{
    public sealed class Delay
    {
        private IEnumerator coroutine;

        /// <summary>
        /// Indicates if the delay has completed and the provided action was invoked.
        /// </summary>
        public bool IsComplete { get; private set; }

        private Action action;

        private Delay()
        {
        }

        private static IEnumerator DelayCoroutine(Delay delayReference, float delay, bool ignoreTimeScale, int framesToSkip)
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

            delayReference.InvokeAction();
        }

        private static IEnumerator WaitCoroutine(Delay delayReference, Func<bool> condition, float timeout = -1, bool ignoreTimeScale = false, bool skipEvaluationForFirstFrame = false)
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

            delayReference.InvokeAction();
        }

        private void InvokeAction()
        {
            if (IsComplete)
                return;

            action.Invoke();

            IsComplete = true;
        }

        private static Delay CreateInternal(float delay, Action action, bool ignoreTimeScale, int framesToSkip)
        {
            var d = new Delay
            {
                action = action
            };

            if (delay <= 0 && framesToSkip <= 0)
            {
                d.InvokeAction();
                return d;
            }

            d.coroutine = DelayCoroutine(d, delay, ignoreTimeScale, framesToSkip);

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
            var d = new Delay
            {
                action = action
            };

            if (condition()) //TODO: unit test e.g. null
            {
                d.InvokeAction();
                return d;
            }

            d.coroutine = WaitCoroutine(d, condition, timeout, ignoreTimeScale, true);

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
        /// Stops the ongoing delay, but does not execute the provided action.
        /// </summary>
        /// <remarks><see cref="IsComplete"/> is not set to TRUE by this method.</remarks>
        public void Stop()
        {
            DelayMonoBehaviour.StopDelay(coroutine);
        }

        /// <summary>
        /// Stops the delay and executes the provided action.
        /// </summary>
        public void Complete()
        {
            Stop();
            InvokeAction();
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