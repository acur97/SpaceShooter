using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(RoundScriptable)), CanEditMultipleObjects]
public class RoundScriptableEditor : Editor
{
    private VisualTreeAsset m_InspectorXML;
    private VisualElement inspector;

    public override VisualElement CreateInspectorGUI()
    {
        inspector = new();
        inspector.Add(new Label("Custom Inspector"));

        m_InspectorXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_Custom Inspectors/Editor/Round scriptable/RoundScriptable_Inspector.uxml");
        inspector = m_InspectorXML.Instantiate();

        return inspector;
    }
}