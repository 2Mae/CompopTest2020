using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Compopulate
{
    public class Session
    {
        public List<ComField> todo = new List<ComField>();
        public List<ComField> done = new List<ComField>();

        public ComField[] fields;

        public Session(ComField[] fields)
        {
            this.fields = fields;
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.expected != field.initial)
                { todo.Add(field); }
                else
                { done.Add(field); }
            }
        }

        public void CommitAll()
        {
            foreach (var field in todo)
            {
                Undo.RecordObject(field.component, "Compopulate");
                field.Commit();
            }
        }
    }
}