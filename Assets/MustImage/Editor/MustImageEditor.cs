using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MustImage))][ExecuteInEditMode]
public class MustImageEditor : UnityEditor.UI.ImageEditor
{
    MustImage mustImage;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        this.mustImage = (MustImage)target;
        mustImage.secondSprite = 
            (Sprite)EditorGUILayout.ObjectField("Second Sprite", mustImage.secondSprite, 
                    typeof(Sprite), false, GUILayout.Height(EditorGUIUtility.singleLineHeight));
    }
}
