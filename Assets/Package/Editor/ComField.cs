using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Compopulate
{
    public struct ComField //Todo: Make interface? better name.
    {
        public readonly Component component;
        public readonly FieldInfo fieldInfo;

        public Component initial;
        public Component expected;

        public ComField(Component component, FieldInfo fieldInfo)
        {
            this.component = component;
            this.fieldInfo = fieldInfo;

            initial = fieldInfo.GetValue(component) as Component;
            expected = component.GetComponent(fieldInfo.FieldType);
        }

        public void Commit()
        {
            fieldInfo.SetValue(component, expected);
        }
    }
}