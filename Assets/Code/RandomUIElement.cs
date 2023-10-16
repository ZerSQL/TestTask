using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomUIElement : MonoBehaviour
{
    [SerializeField] private RectTransform selfRect;
    [SerializeField] private UnityEngine.UI.Image notificatorImage;

    public RectTransform SelfRect => selfRect;

    private bool couldBeMarked = true;
    private bool isMarkedForHide = false;


    private void OnEnable()
    {
        couldBeMarked = true;
        notificatorImage.gameObject.SetActive(!isMarkedForHide);
    }

    public void MarkNotificatorForHide()
    {
        if (!couldBeMarked)
            return;

        isMarkedForHide = true;
    }

    public void Reset()
    {
        couldBeMarked = false;
        isMarkedForHide = false;
    }
}
