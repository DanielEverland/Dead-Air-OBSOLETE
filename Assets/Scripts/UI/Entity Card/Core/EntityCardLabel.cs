
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class EntityCardLabel : EntityCardBehaviour {

    public string Text { get { return textElement.text; } set { textElement.text = value; } }

    [SerializeField]
    private TMP_Text textElement;
    [SerializeField]
    private string format = "{0}";

    protected override void InitializeData(EntityCard.Data data)
    {
        textElement.text = string.Format(format, data.GetData<object>(DataType).ToString());
    }
    private void OnValidate()
    {
        textElement = GetComponent<TMP_Text>();
    }
}
