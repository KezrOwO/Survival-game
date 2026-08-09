using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField]
    private Text headerField;
    [SerializeField]
    private Text contentField;

    [SerializeField]
    private LayoutElement layoutElement;

    [SerializeField]
    private int maxCharacter;

    [SerializeField]
    private RectTransform rectTransform;

    public void SetText(string content, string header = "")
    {
        if(header == "")
        {
            headerField.gameObject.SetActive(false);
        }
        else
        {
            headerField.gameObject.SetActive(true);
            headerField.text = header; 
        }

        contentField.text = content;

        int hearderLength = headerField.text.Length;
        int contentLength = contentField.text.Length;

        layoutElement.enabled = (hearderLength > maxCharacter || contentLength > maxCharacter) ? true : false;
    }

    private void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        float pivotX = mousePos.x / Screen.width;
        float pivotY = mousePos.y / Screen.height;

        rectTransform.pivot = new Vector2(pivotX, pivotY);

        transform.position = mousePos;
    }
}
