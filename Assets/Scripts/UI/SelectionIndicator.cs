using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionIndicator : MonoBehaviour {

    private float AnimationCoefficient { get { return Mathf.Pow(animationTime / ANIMATION_TIME_TOTAL, 2); } }
    private RectTransform RectTransform { get { return (RectTransform)transform; } }

    private Entity selectedEntity;
    private float animationTime;

    private const float ANIMATION_TIME_SCALE = 1;
    private const float ANIMATION_TIME_TOTAL = 0.2f;

    private const float SIZE_ANIMATION_END_SCALE = 1.2f;
    private const float SIZE_ANIMATION_START_SCALE = 2;

    public void Clear()
    {
        selectedEntity = null;
    }
	public void Select(Entity entity)
    {
        selectedEntity = entity;
        animationTime = 0;
    }
    private void Update()
    {
        if (selectedEntity == null)
            return;

        UpdateAnimation();
        Transform();
        Scale();
    }
    private void UpdateAnimation()
    {
        animationTime += Time.unscaledDeltaTime * ANIMATION_TIME_SCALE;
        animationTime = Mathf.Clamp(animationTime, 0, ANIMATION_TIME_TOTAL);
    }
    private void Transform()
    {
        transform.position = Camera.main.WorldToScreenPoint(selectedEntity.transform.position);
    }
    private void Scale()
    {
        RectTransform.sizeDelta = GetScale();
    }
    private Vector2 GetScale()
    {
        CornerPoints cornerPoints = new CornerPoints(selectedEntity.Rect);
        cornerPoints.Transform(x => Camera.main.WorldToScreenPoint(x));

        Vector2 startSize = cornerPoints.Size * SIZE_ANIMATION_START_SCALE;
        Vector2 endSize = cornerPoints.Size * SIZE_ANIMATION_END_SCALE;

        return Vector2.Lerp(startSize, endSize, AnimationCoefficient);
    }
}
