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
        bool close = false;
        public Session session;

        [MenuItem("Window/Compopulate")]
        public static void ShowWindow()
        {
            Compopup window = ScriptableObject.CreateInstance<Compopup>();
            window.position = new Rect(500, 500, 200, 100);
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