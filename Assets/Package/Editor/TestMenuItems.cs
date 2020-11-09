using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Compopulate;
using UnityEditor;
namespace Compopulate
{

    public class TestMenuItems : MonoBehaviour
    {
        [UnityEditor.MenuItem("Compopulate/Process all %Q")]
        public static void TestSelection()
        {
            // Compopulator.FindAndProcessAll();
            var fields = Compopulator.FindFields(Compopulator.FindAllComponents());
            Session session = new Session(fields);
            if (session.todo.Count == 0)
            {
                Debug.Log("All compopulated");
                return;
            }
            else
            {
                Compopup.ShowSession(session);
            }
        }
    }
}
