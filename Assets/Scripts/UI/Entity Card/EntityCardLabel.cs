
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class EntityCardLabel : EntityCardBehaviour {

    [SerializeField]
    private TMP_Text text;
    [SerializeField]
    private string format = "{0}";

    protected override void InitializeData(EntityCard.Data data)
    {
        text.text = string.Format(format, data.GetData<object>(DataType).ToString());
    }
    private void OnValidate()
    {
        text = GetComponent<TMP_Text>();
    }
}
