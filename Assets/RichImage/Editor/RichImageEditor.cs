using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(RichImage))]
public class RichImageEditor : UnityEditor.Editor 
{
    SerializedProperty
        blUV,
        tlUV,
        trUV,
        brUV;

    public void OnEnable()
    {
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }
}
