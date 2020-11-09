using System;
using UnityEditor;
using UnityEngine;

namespace Compopulate
{
    public static class OnPlay
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoadRuntimeMethod()
        {
            var fields = Compopulator.FindFields(GameObject.FindObjectsOfType(typeof(Component)) as Component[]);
            foreach (var cfield in fields)
            {
                if (Compopulator.CanCompop(cfield, out var a, out var b))
                {
                    InterruptPlay(() => Compopulator.FindAndProcessAll());
                    break;
                }
            }

        }

        static Action onInterrupted;

        static void InterruptPlay(Action action = null)
        {
            onInterrupted = action;
            EditorApplication.ExitPlaymode();
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
            {
                EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
                onInterrupted.Invoke();
                onInterrupted = null;
            }
        }
    }
}
