using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ColorField))]
public class ColorFieldPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ColorField field = attribute as ColorField;
        Color originalColor = GUI.color;

        GUI.color = field.color; //Set the color of the GUI

        EditorGUI.LabelField(position, label);

        GUI.color = originalColor; //Reset the color of the GUI

        //EditorGUI.PropertyField(position, property, label); //Draw the GUI

        // Draw the property field, excluding the label
        EditorGUI.PropertyField(
            new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth, position.height),
            property,
            GUIContent.none
        );
    }
}