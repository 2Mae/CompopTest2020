using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Compopulate;
using UnityEditor;

public class TestMenuItems : MonoBehaviour
{
    [UnityEditor.MenuItem("Compopulate/Process all")]
    public static void TestSelection()
    {
        Compopulator.FindAndProcessAll();
    }
}
