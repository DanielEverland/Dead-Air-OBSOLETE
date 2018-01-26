using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour {

    [SerializeField]
    private TMP_Text _textElement;
    [SerializeField]
    private Image _progressBar;

    private void Awake()
    {
        _progressBar.fillAmount = 0;
        _textElement.text = "Loading...";
    }
    public void SetText(string value)
    {
        _textElement.text = value;
    }
    public void SetProgress(float value)
    {
        _progressBar.fillAmount = Mathf.Max(_progressBar.fillAmount, value);
    }
    private void Update()
    {
        SanitizeHierarchy(); 
    }
    private void SanitizeHierarchy()
    {
        if(transform.parent != null)
        {
            if (!transform.root.GetComponent<Canvas>() && Canvas2D.Instance != null)
                transform.SetParent(Canvas2D.Instance.transform, false);
        }       

        transform.SetAsLastSibling();
    }
}
