using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCardNumberEnabler : MonoBehaviour {

    [SerializeField]
    private EntityCard.DataTypes TargetData;
    [SerializeField]
    private float TriggerValue;
    [SerializeField]
    private Condition TriggerCondition = Condition.Equal;
    [SerializeField]
    private bool DisableOnTrigger = true;

	public void OnReceviedData(EntityCard.Data data)
    {
        if (data.CanCast<float>(TargetData))
            HandleInput(data.GetData<float>(TargetData));
    }
    private void HandleInput(float dataValue)
    {
        switch (TriggerCondition)
        {
            case Condition.More:
                PollTrigger(dataValue > TriggerValue);
                break;
            case Condition.Equal:
                PollTrigger(dataValue == TriggerValue);
                break;
            case Condition.Less:
                PollTrigger(dataValue < TriggerValue);
                break;
            default:
                throw new System.NotImplementedException("Trigger condition " + TriggerCondition + " is not recognized");
        }
    }
    private void PollTrigger(bool shouldTrigger)
    {
        if (shouldTrigger)
            Trigger();
    }
    private void Trigger()
    {
        gameObject.SetActive(!DisableOnTrigger);
    }

    private enum Condition
    {
        More,
        Equal,
        Less,
    }
}
