using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(ButtonScaled), true)]
[CanEditMultipleObjects]
public class ButtonScaledEditor : SelectableEditor
{
    SerializedProperty m_OnClickProperty;
    SerializedProperty sp_Scale;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
        sp_Scale = serializedObject.FindProperty("scale");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        serializedObject.Update();

        EditorGUILayout.PropertyField(m_OnClickProperty);
        EditorGUILayout.PropertyField(sp_Scale);

        serializedObject.ApplyModifiedProperties();
    }
}