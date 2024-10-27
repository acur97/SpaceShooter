using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(ShipScriptable))]
public class ShipScriptableEditor : Editor
{
    private VisualTreeAsset m_InspectorXML;
    private VisualElement inspector;

    private VisualElement _sprite;
    private FloatField _spaceCoolDown;
    private FloatField _timeToContinue;

    public override VisualElement CreateInspectorGUI()
    {
        inspector = new();
        inspector.Add(new Label("Custom Inspector"));

        m_InspectorXML = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/_Custom Inspectors/Editor/ShipScriptable_Inspector.uxml");
        inspector = m_InspectorXML.Instantiate();

        ObjectField sprite_Field = inspector.Q<ObjectField>("Field_ShipSprite");
        _sprite = inspector.Q<VisualElement>("VisualSprite");
        sprite_Field.RegisterValueChangedCallback(OnSpriteChange);

        Toggle toggle_SpaceCoolDown_Field = inspector.Q<Toggle>("Toggle_spaceCoolDown");
        _spaceCoolDown = inspector.Q<FloatField>("Field_spaceCoolDown");
        toggle_SpaceCoolDown_Field.RegisterValueChangedCallback(OnToggle_SpaceCoolDown_Change);

        Toggle toggle_timeToContinue_Field = inspector.Q<Toggle>("Toggle_timeToContinue");
        _timeToContinue = inspector.Q<FloatField>("Field_timeToContinue");
        toggle_timeToContinue_Field.RegisterValueChangedCallback(OnToggle_timeToContinue_Change);

        return inspector;
    }

    private void OnSpriteChange(ChangeEvent<Object> evt)
    {
        _sprite.style.backgroundImage = new StyleBackground(evt.newValue as Sprite);
    }

    private void OnToggle_SpaceCoolDown_Change(ChangeEvent<bool> evt)
    {
        _spaceCoolDown.SetEnabled(evt.newValue);
    }

    private void OnToggle_timeToContinue_Change(ChangeEvent<bool> evt)
    {
        _timeToContinue.SetEnabled(evt.newValue);
    }
}