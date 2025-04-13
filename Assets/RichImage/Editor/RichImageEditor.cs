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
    SerializedProperty
        m_Color,
        m_RaycastTarget,
        m_RayCastPadding,
        m_Maskable;

    SerializedProperty
        m_MainSprite,
        m_SecondarySprite,
        m_CustomMesh;


    public void OnEnable()
    {
        m_MainSprite = serializedObject.FindProperty("m_MainSprite");
        m_SecondarySprite = serializedObject.FindProperty("m_SecondarySprite");
        m_CustomMesh = serializedObject.FindProperty("m_CustomMesh");
        m_Color = serializedObject.FindProperty("m_Color");
        m_RaycastTarget = serializedObject.FindProperty("m_RaycastTarget");
        m_RayCastPadding = serializedObject.FindProperty("m_RaycastPadding");
        m_Maskable = serializedObject.FindProperty("m_Maskable");

    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(m_MainSprite);
        EditorGUILayout.PropertyField(m_SecondarySprite);
        EditorGUILayout.PropertyField(m_CustomMesh);
        EditorGUILayout.PropertyField(m_Color);

        EditorGUILayout.PropertyField(m_RaycastTarget);
        EditorGUILayout.PropertyField(m_RayCastPadding);
        EditorGUILayout.PropertyField(m_Maskable);

        if (GUILayout.Button("Set Native Size"))
        {
            RichImage richImage = (RichImage)target;
            richImage.SetToNativeSize();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
