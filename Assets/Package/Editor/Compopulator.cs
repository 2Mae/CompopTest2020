using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Compopulate
{


    public static class Compopulator
    {
        public struct CField //Todo: Make interface? better name.
        {
            public readonly Component component;
            public readonly FieldInfo fieldInfo;
            public CField(Component component, FieldInfo fieldInfo)
            {
                this.component = component;
                this.fieldInfo = fieldInfo;
            }
        }

        public class LogEntry
        {
            CField field;

            public bool changed = false;

            public LogEntry(CField field)
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
            Component[] allComponents = GameObject.FindObjectsOfType(typeof(Component)) as Component[];
            var log = Compopulator.ProcessComponents(allComponents);
            Debug.Log($"Compopulated {log.changes} fields.");
            return log;
        }

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
        public static LogEntry[] ProcessFields(CField[] cfields)
        {
            LogEntry[] logEntries = new LogEntry[cfields.Length];
            for (int i = 0; i < cfields.Length; i++)
            {
                logEntries[i] = ProcessField(cfields[i]);
            }

            return logEntries;
        }

        public static bool CanCompop(CField cfield, out Component oldValue, out Component newValue)
        {
            oldValue = cfield.fieldInfo.GetValue(cfield.component) as Component;
            newValue = cfield.component.GetComponent(cfield.fieldInfo.FieldType);
            return (oldValue != newValue);
        }

        public static LogEntry ProcessField(CField cfield)
        {
            LogEntry log = new LogEntry(cfield);

            Undo.RecordObject(cfield.component, $"Compopulate: {cfield.component}");

            if (CanCompop(cfield, out var a, out var b))
            {
                cfield.fieldInfo.SetValue(cfield.component, b);
                log.changed = true;
            }
            return log;
        }

        public static CField[] FindFields(Component[] components)
        {
            List<CField> cfields = new List<CField>();
            for (int i = components.Length - 1; i >= 0; i--)//FORR since list is reverse to hierarchy order by default. //Todo: Confirm this with hierarchies
            {
                Component component = components[i];

                const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public; // why these particular flags again?
                FieldInfo[] fields = component.GetType().GetFields(bindingFlags);

                for (int j = 0; j < fields.Length; j++)
                {
                    var fieldInfo = fields[j];

                    Compop compopulateAttribute = Attribute.GetCustomAttribute(fieldInfo, typeof(Compop)) as Compop;
                    if (compopulateAttribute != null)
                    {
                        CField cf = new CField(component, fieldInfo);

                        if (CanCompop(fieldInfo))
                        {
                            cfields.Add(cf);
                        }
                        else
                        {
                            Debug.LogWarning($"Ineffectual use of {compopulateAttribute.GetType()}: {components[i].GetType()}.{fieldInfo.Name}\nOnly works with public or [SerialiseField]");
                        }
                    }
                }
            }

            return cfields.ToArray();
        }

        public static bool CanCompop(FieldInfo fieldInfo)
        {
            bool hasSerializeField = (Attribute.GetCustomAttribute(fieldInfo, typeof(SerializeField)) != null);
            bool notHideInInspector = (Attribute.GetCustomAttribute(fieldInfo, typeof(HideInInspector)) == null);
            return notHideInInspector && (hasSerializeField || fieldInfo.IsPublic);
        }
    }
}