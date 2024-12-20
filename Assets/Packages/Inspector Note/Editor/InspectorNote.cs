using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InformationNote))]
public class InspectorNote : Editor
{
    // VARIABLES
    private string buttonText = "Start typing";
    private int selectedValue = 5;
    private string[] displayOptions = new string[] { "Line Label", "Box Text ", "Box Info", "Box Warning", "Box Error" };
    private int[] finalDisplayOption = new int[] { 0, 1, 2, 3, 4 };

    public override void OnInspectorGUI()
    {
        InformationNote inMyScript = (InformationNote)target;

        if (!inMyScript.isReady)
        {
            // User adding Input text in the inspector

            switch (selectedValue)
            {
                case 0:
                    if (EditorGUILayout.Toggle(inMyScript.isReady)) inMyScript.SwitchToggle();  // Toggle
                    {
                        EditorGUILayout.LabelField(inMyScript.TextInfo);                    // A small line text
                    }
                    break;

                case 1:
                    if (GUILayout.Button(buttonText)) inMyScript.SwitchToggle();
                    {
                        buttonText = "INFO";
                        EditorGUILayout.HelpBox(inMyScript.TextInfo, MessageType.None);     // This is a small box
                    }
                    break;

                case 2:
                    goto default;// same as default

                case 3:
                    if (GUILayout.Button(buttonText)) inMyScript.SwitchToggle();
                    {
                        buttonText = "ALERT";
                        EditorGUILayout.HelpBox(inMyScript.TextInfo, MessageType.Warning);  // This is a Warning box
                    }
                    break;
                case 4:
                    if (GUILayout.Button(buttonText)) inMyScript.SwitchToggle();
                    {
                        buttonText = "ERROR";
                        EditorGUILayout.HelpBox(inMyScript.TextInfo, MessageType.Error);    // This is a Error box
                    }
                    break;
                default:
                    if (GUILayout.Button(buttonText)) inMyScript.SwitchToggle();            // Button
                    {
                        buttonText = "README";
                        EditorGUILayout.HelpBox(inMyScript.TextInfo, MessageType.Info);     // This is a help box
                    }
                    break;
            }
        }
        else
        {
            // Visualisation of final text in the inspector.
            buttonText = "LOCK";

            // Display [ LOCK ] Button and switch if is press
            if (GUILayout.Button(buttonText)) inMyScript.SwitchToggle();

            // [ Input text ]
            inMyScript.TextInfo = EditorGUILayout.TextArea(inMyScript.TextInfo);

            // selection
            selectedValue = EditorGUILayout.IntPopup("Text Type :", selectedValue, displayOptions, finalDisplayOption);

            // warning
            EditorGUILayout.HelpBox(" Press LOCK at the top when finish. ", MessageType.Warning); // A Warning box
        }
    }
}
