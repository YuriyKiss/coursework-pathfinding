using UnityEngine;

[AddComponentMenu("Miscellaneous/Inspector Text Info Note")]
public class InformationNote : MonoBehaviour
{
    public bool isReady = true;

    public string TextInfo = "Type your message here and press enter to send";

    public string spaceMessage = ""; // use in case of space or message

    private void Awake()
    {
        this.enabled = false; // Disable this component when game start
    }

    public void SwitchToggle()
    {
        isReady = !isReady;
    }
}
