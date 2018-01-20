using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CategoryAligner : UIBehaviour, ILayoutSelfController {

    [SerializeField]
    private RectTransform _titleElement;
    [SerializeField]
    private RectTransform _contentElement;

    private DrivenRectTransformTracker m_Tracker;

    [System.NonSerialized] private RectTransform m_Rect;
    private RectTransform rectTransform
    {
        get
        {
            if (m_Rect == null)
                m_Rect = GetComponent<RectTransform>();
            return m_Rect;
        }
    }

    protected CategoryAligner()
    { }

    public void SetLayoutHorizontal()
    {

    }
    public void SetLayoutVertical()
    {
        m_Tracker.Clear();
        _contentElement.anchoredPosition = new Vector2(0, -_titleElement.GetWorldRect().height);

        float size = _contentElement.GetWorldRect().height + _titleElement.GetWorldRect().height;
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
    }

    #region Unity Lifetime calls

    protected override void OnEnable()
    {
        base.OnEnable();
        SetDirty();
    }

    protected override void OnDisable()
    {
        m_Tracker.Clear();
        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        base.OnDisable();
    }

    #endregion

    protected override void OnRectTransformDimensionsChange()
    {
        SetDirty();
    }
    
    protected void SetDirty()
    {
        if (!IsActive())
            return;

        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        SetDirty();
    }

#endif

    
}
