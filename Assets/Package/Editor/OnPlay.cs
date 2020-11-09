using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Compopulate
{
    public static class OnPlay
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        [UnityEditor.MenuItem("Window/Compopulate")]
        static void OnBeforeSceneLoadRuntimeMethod()
        {
            var fields = Compopulator.FindFields(Compopulator.FindAllComponents());
            Session session = new Session(fields);
            if (session.todo.Count > 0)
            {
                //exiting playmode destroys references to fields
                InterruptPlay(() => Compopup.ShowWindow());
            }
            else
            {
                int number = session.fields.Length;
                EditorWindow.focusedWindow.ShowNotification(new GUIContent($"Nothing to compopulate (0/{number})"));
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
