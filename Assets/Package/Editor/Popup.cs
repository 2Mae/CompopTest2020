using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.ShortcutManagement;
using UnityEngine.UIElements;

namespace Compopulate
{
    public class Compopup : EditorWindow
    {
        private const KeyCode hotkey = KeyCode.BackQuote;
        bool close = false;
        public Session session;

        [MenuItem("Compopulate/Popup %T")]
        public static void ShowWindow()
        {
            Compopup window = ScriptableObject.CreateInstance<Compopup>();
            var main = EditorGUIUtility.GetMainWindowPosition();
            Vector2 size = new Vector2(200, 200);
            window.position = new Rect(main.position + (main.size - size) / 2, size);
            window.ShowPopup();
            window.session = new Session(Compopulator.FindFields(Compopulator.FindAllComponents()));
            window.OnShow();
        }
        Label header;
        Label todoCount;
        Button button;

        public void OnShow()
        {
            header = new Label("Compopulate");
            todoCount = new Label($"{session.todo.Count}/{session.fields.Length}");
            button = new Button(OnButton);
            button.Add(new Label("Commit"));

            rootVisualElement.Add(header);
            rootVisualElement.Add(todoCount);
            rootVisualElement.Add(button);
        }

        void OnButton()
        {
            session.CommitAll();
            close = true;
        }

        public static void ShowSession(Session session)
        {
            Compopup window = ScriptableObject.CreateInstance<Compopup>();
            var main = EditorGUIUtility.GetMainWindowPosition();
            Vector2 size = new Vector2(200, 200);
            window.position = new Rect(main.position + (main.size - size) / 2, size);
            window.session = session;
            window.ShowPopup();
            window.OnShow();
        }

        private void OnGUI()
        {
            var e = Event.current;

            switch (e.keyCode)
            {
                case KeyCode.Escape:
                    close = true;
                    break;
                case KeyCode.Space:
                case KeyCode.Return:
                    OnButton();
                    break;
            }
        }

        private void OnLostFocus() { close = true; }

        private void Update()
        {
            //Had some issues when calling Close() outside Update.
            if (close)
            {
                Close();
            }
        }
    }
}