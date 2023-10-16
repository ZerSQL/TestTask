using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] private List<RandomUIElement> UIItemTransforms;
    [SerializeField] private Character character;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private UnityEngine.UI.Button toggleScrollButton;
    [SerializeField] private UnityEngine.UI.Button resetScrollInfoButton;
    [SerializeField] private GameObject scrollViewGO;

    private bool scrollViewShowing = false;


    private void Start()
    {
        toggleScrollButton.onClick.RemoveAllListeners();
        toggleScrollButton.onClick.AddListener(OnToggleButtonClicked);

        resetScrollInfoButton.onClick.RemoveAllListeners();
        resetScrollInfoButton.onClick.AddListener(OnResetInfoButtonClicked);
    }

    private void Update()
    {
        UpdateInput();
        UpdateUI();
    }

    private void UpdateInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            character.MoveToPoint(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            character.MoveToPoint(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            character.MoveToPoint(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            character.MoveToPoint(3);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            character.CancelToken();
        }
    }

    private void UpdateUI()
    {
        if (!scrollViewShowing)
            return;

        for (int i = 0; i < UIItemTransforms.Count; i++)
        {
            if (UIItemTransforms[i].SelfRect.IsRectVisible(mainCamera))
            {
                UIItemTransforms[i].MarkNotificatorForHide();
            }
        }
    }

    private void OnToggleButtonClicked()
    {
        scrollViewShowing = !scrollViewShowing;
        scrollViewGO.SetActive(scrollViewShowing);
    }

    private void OnResetInfoButtonClicked()
    {
        foreach (var item in UIItemTransforms)
        {
            item.Reset();
        }
    }
}
