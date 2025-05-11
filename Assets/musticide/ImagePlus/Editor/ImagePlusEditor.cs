using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace Musticide.UI
{
    [CustomEditor(typeof(ImagePlus))]
    public class ImagePlusEditor : UnityEditor.Editor
    {
        SerializedProperty
            m_Color,
            m_RaycastTarget,
            m_Material;
        // m_RayCastPadding,
        // m_Maskable,
        // m_Type;

        SerializedProperty
            m_MainSprite,
            m_SecondarySprite,
            m_SecondarySpriteUserScaleOffset,
            m_TexBlendMode,
            m_TileSecondarySprite,
            m_CustomMesh,
            m_IsGleam,
            m_Gleam;



        public void OnEnable()
        {
            m_MainSprite = serializedObject.FindProperty("m_MainSprite");
            m_SecondarySprite = serializedObject.FindProperty("m_SecondarySprite");
            m_SecondarySpriteUserScaleOffset = serializedObject.FindProperty("m_SecondarySpriteUserScaleOffset");
            m_TileSecondarySprite = serializedObject.FindProperty("m_TileSecondarySprite");

            m_CustomMesh = serializedObject.FindProperty("m_CustomMesh");
            m_Color = serializedObject.FindProperty("m_Color");

            m_IsGleam = serializedObject.FindProperty("m_IsGleam");
            m_Gleam = serializedObject.FindProperty(("m_Gleam"));

            m_Material = serializedObject.FindProperty("m_Material");

            m_RaycastTarget = serializedObject.FindProperty("m_RaycastTarget");

            // m_RayCastPadding = serializedObject.FindProperty("m_RaycastPadding");
            // m_Maskable = serializedObject.FindProperty("m_Maskable");
            // m_Maskable = serializedObject.FindProperty("m_Type");

            m_TexBlendMode = serializedObject.FindProperty("m_TexBlendMode");

        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_MainSprite);

            EditorGUILayout.PropertyField(m_TexBlendMode);
            EditorGUILayout.PropertyField(m_SecondarySprite);
            DrawScaleOffset(m_SecondarySpriteUserScaleOffset);
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_TileSecondarySprite, new GUIContent("Tiling"));
            EditorGUI.indentLevel--;
            EditorGUILayout.Separator();

            EditorGUILayout.PropertyField(m_CustomMesh);
            EditorGUILayout.PropertyField(m_Color);
            EditorGUILayout.PropertyField(m_Material);
            EditorGUILayout.Separator();

            m_IsGleam.boolValue = DrawGleam(m_Gleam, m_IsGleam.boolValue);
            EditorGUILayout.Separator();


            EditorGUILayout.PropertyField(m_RaycastTarget);
            // EditorGUILayout.PropertyField(m_RayCastPadding);
            // EditorGUILayout.PropertyField(m_Maskable);

            if (GUILayout.Button("Set Native Size"))
            {
                ImagePlus imagePlus = (ImagePlus)target;
                imagePlus.SetToNativeSize();
            }
            serializedObject.ApplyModifiedProperties();
        }

        void DrawScaleOffset(SerializedProperty vector4Prop)
        {
            Vector4 vec = vector4Prop.vector4Value;
            Vector2 scale = new Vector2(vec.x, vec.y);
            Vector2 offset = new Vector2(vec.z, vec.w);

            EditorGUI.indentLevel++;
            scale = EditorGUILayout.Vector2Field("Scale", scale);
            offset = EditorGUILayout.Vector2Field("Offset", offset);
            EditorGUI.indentLevel--;

            vector4Prop.vector4Value = new Vector4(scale.x, scale.y, offset.x, offset.y);
        }

        bool DrawGleam(SerializedProperty vector4Prop, bool a_IsGleam)
        {
            a_IsGleam = EditorGUILayout.ToggleLeft("Enable Gleam", a_IsGleam);
            if (a_IsGleam)
            {
                Vector4 vec = vector4Prop.vector4Value;
                float width = vec.x;
                float angle = vec.y;
                float speed = vec.z;
                float space = vec.w;

                EditorGUI.indentLevel += 2;
                width = EditorGUILayout.Slider("Width", width, 0.0f, 1.0f);
                angle = EditorGUILayout.Slider("Angle", angle, 0.0f, 1.0f);
                speed = EditorGUILayout.Slider("Speed", speed, -10.0f, 10.0f);
                space = EditorGUILayout.IntSlider("Space", (int)space, 1, 15);
                EditorGUI.indentLevel -= 2;

                vector4Prop.vector4Value = new Vector4(width, angle, speed, space);
            }
            return a_IsGleam;
        }
    }
}
