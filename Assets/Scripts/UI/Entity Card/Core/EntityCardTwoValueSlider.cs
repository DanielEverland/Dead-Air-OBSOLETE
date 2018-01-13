using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class EntityCardTwoValueSlider : MonoBehaviour, IEntityCardBehaviour
{
    [SerializeField]
    private TMP_Text _textElement;
    [SerializeField]
    private Image _bottomSlider;
    [SerializeField]
    private Image _topSlider;
    [SerializeField]
    private EntityCard.DataTypes _firstValue;
    [SerializeField]
    private EntityCard.DataTypes _secondValue;
    [Tooltip("This is used when calculating fill values for the sliders")]
    [SerializeField]
    private EntityCard.DataTypes _relativeValue;
    [SerializeField]
    private bool _disableIfEmpty = true;
    [SerializeField]
    private OnReceivedDataEvent OnReceviedData;

    public EntityCard.DataTypes FirstValue { get { return _firstValue; } }
    public EntityCard.DataTypes SecondValue { get { return _secondValue; } }
    public EntityCard.DataTypes RelativeValue { get { return _relativeValue; } }

    public float MinValue { get; private set; }
    public float MaxValue { get; private set; }

    public void Initialize(EntityCard.Data data)
    {
        if (data.HasData(FirstValue) && data.HasData(SecondValue) && data.HasData(RelativeValue))
        {
            Enable();

            float firstValue = data.GetData<int>(FirstValue);
            float secondValue = data.GetData<int>(SecondValue);
            float relative = data.GetData<int>(RelativeValue);

            MinValue = Mathf.Min(firstValue, secondValue);
            MaxValue = Mathf.Max(firstValue, secondValue);

            _bottomSlider.fillAmount = MaxValue / relative;
            _topSlider.fillAmount = MinValue / relative;

            if (_textElement != null)
            {
                if (MinValue == MaxValue)
                {
                    _textElement.text = string.Format("{0}", relative);
                }
                else
                {
                    _textElement.text = string.Format("{0} / {1} / {2}", MinValue, MaxValue, relative);
                }
            }
        }
        else if (_disableIfEmpty)
        {

            Disable();
        }

        if (OnReceviedData != null)
            OnReceviedData.Invoke(data);
    }
    public void Enable()
    {
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }
    [System.Serializable]
    private class OnReceivedDataEvent : UnityEvent<EntityCard.Data> { }
}
