using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Compopulate
{
    public static class Compopulator
    {
        public class LogEntry
        {
            ComField field;

            public bool changed = false;

            public LogEntry(ComField field)
            {
                this.field = field;
            }
        }

        public class Log
        {
            public LogEntry[] entries;
            List<LogEntry> changedEntries = new List<LogEntry>();
            public int changes => changedEntries.Count;
            public Log(LogEntry[] entries)
            {
                this.entries = entries;

                foreach (var entry in entries)
                {
                    if (entry.changed) { changedEntries.Add(entry); }
                }
            }
        }

        public static Log FindAndProcessAll()
        {
            Component[] allComponents = FindAllComponents();
            var log = Compopulator.ProcessComponents(allComponents);
            Debug.Log($"Compopulated {log.changes} fields.");
            return log;
        }

        public static Component[] FindAllComponents() => GameObject.FindObjectsOfType(typeof(Component)) as Component[];

        public static Log ProcessComponents(Component[] components)
        {
            Undo.SetCurrentGroupName("Process components");
            int group = Undo.GetCurrentGroup();
            var fields = FindFields(components);
            var entries = ProcessFields(fields);
            Undo.CollapseUndoOperations(group);

            Log log = new Log(entries);
            return log;
        }
        public static LogEntry[] ProcessFields(ComField[] fields)
        {
            LogEntry[] logEntries = new LogEntry[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                logEntries[i] = ProcessField(fields[i]);
            }

            return logEntries;
        }

        public static bool CanCompop(ComField field)
        {
            return (field.initial != field.expected);
        }

        public static LogEntry ProcessField(ComField field)
        {
            LogEntry log = new LogEntry(field);

            Undo.RecordObject(field.component, $"Compopulate: {field.component}");

            if (CanCompop(field))
            {
                field.Commit();
                log.changed = true;
            }
            return log;
        }

        public static ComField[] FindFields(Component[] components)
        {
            List<ComField> fields = new List<ComField>();
            for (int i = components.Length - 1; i >= 0; i--)//FORR since list is reverse to hierarchy order by default. //Todo: Confirm this with hierarchies
            {
                Component component = components[i];

                const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public; // why these particular flags again?
                FieldInfo[] fieldInfos = component.GetType().GetFields(bindingFlags);

                for (int j = 0; j < fieldInfos.Length; j++)
                {
                    var fieldInfo = fieldInfos[j];

                    Compop compopulateAttribute = Attribute.GetCustomAttribute(fieldInfo, typeof(Compop)) as Compop;
                    if (compopulateAttribute != null)
                    {
                        ComField field = new ComField(component, fieldInfo);

                        if (CanCompop(fieldInfo))
                        {
                            fields.Add(field);
                        }
                        else
                        {
                            Debug.LogWarning($"Ineffectual use of {compopulateAttribute.GetType()}: {components[i].GetType()}.{fieldInfo.Name}\nOnly works with public or [SerialiseField]");
                        }
                    }
                }
            }

            return fields.ToArray();
        }

        public static bool CanCompop(FieldInfo fieldInfo)
        {
            bool hasSerializeField = (Attribute.GetCustomAttribute(fieldInfo, typeof(SerializeField)) != null);
            bool notHideInInspector = (Attribute.GetCustomAttribute(fieldInfo, typeof(HideInInspector)) == null);
            return notHideInInspector && (hasSerializeField || fieldInfo.IsPublic);
        }
    }
}