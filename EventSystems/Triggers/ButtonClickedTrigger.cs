using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickedTrigger : GameEventTrigger
{
    private Button button;
    private bool triggered = false;

    public override void Start()
    {
        base.Start();
        button = GetComponent<Button>();
        button.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        triggered = !triggered;
        if (triggered)
            TriggerEnter(null);
        else
            TriggerExit(null);
    }


}
