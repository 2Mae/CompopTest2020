using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Compopulate;
using UnityEditor;

public class TestMenuItems : MonoBehaviour
{
    [UnityEditor.MenuItem("Test/Test Compop %Q")]
    public static void TestSelection()
    {
        Compopulator.FindAndProcessAll();
    }
}
