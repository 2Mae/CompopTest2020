using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Compopulate;

public class TestComponent : MonoBehaviour
{
    [Compop] public Rigidbody body;
    [Compop] public MeshFilter meshFilter;
}
