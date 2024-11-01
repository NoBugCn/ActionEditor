using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NBC.ActionEditor
{
    public abstract class BaseTest
    {
        public string Name;
    }

    public class Test1 : BaseTest
    {
        public int I1;
        public Vector2 V2;
    }

    public class Test2 : BaseTest
    {
        public float F1;
        public Color C1;
    }

    public class Test
    {
        [MenuItem("NBC/Test", false, 0)]
        public static void OpenDirectorWindow()
        {
            
        }
    }
}