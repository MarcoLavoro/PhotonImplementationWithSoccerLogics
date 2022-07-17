#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorNote : MonoBehaviour 
{

    [TextArea(3, 1000)]
    public string Note = "Text...";
}

# endif

