using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(ShipScriptable)), CanEditMultipleObjects]
public class ShipScriptableEditor : Editor
{
    private VisualTreeAsset m_InspectorXML;
    private VisualElement inspector;

    private VisualElement _sprite;
    private FloatField _spaceCoolDown;
    private FloatField _spaceCoolDown2;
    private FloatField _bulletTime;
    private FloatField _timeToContinue;
    private Slider _direct;

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
        _spaceCoolDown2 = inspector.Q<FloatField>("Field_spaceCoolDown2");
        toggle_SpaceCoolDown_Field.RegisterValueChangedCallback(OnToggle_SpaceCoolDown_Change);

        Toggle toggle_BulletTime_Field = inspector.Q<Toggle>("Toggle_bulletTime");
        _bulletTime = inspector.Q<FloatField>("Field_bulletTime");
        toggle_BulletTime_Field.RegisterValueChangedCallback(OnToggle_BulletTime_Change);

        Toggle toggle_direct_Field = inspector.Q<Toggle>("Toggle_direct");
        _direct = inspector.Q<Slider>("Slider_direct");
        toggle_direct_Field.RegisterValueChangedCallback(OnToggle_direct_Change);

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
        _spaceCoolDown?.SetEnabled(evt.newValue);
        _spaceCoolDown2?.SetEnabled(evt.newValue);
    }

    private void OnToggle_BulletTime_Change(ChangeEvent<bool> evt)
    {
        _bulletTime?.SetEnabled(evt.newValue);
    }

    private void OnToggle_direct_Change(ChangeEvent<bool> evt)
    {
        _direct?.SetEnabled(evt.newValue);
    }

    private void OnToggle_timeToContinue_Change(ChangeEvent<bool> evt)
    {
        _timeToContinue?.SetEnabled(evt.newValue);
    }
}