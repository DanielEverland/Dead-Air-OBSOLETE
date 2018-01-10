using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class EntityCardLabel : EntityCardBehaviour {

    [SerializeField]
    private Text text;

    protected override void InitializeData(EntityCard.Data data)
    {
        text.text = data.GetData<string>(DataType);
    }
    private void OnValidate()
    {
        text = GetComponent<Text>();
    }
}
