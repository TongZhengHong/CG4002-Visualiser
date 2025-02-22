using UnityEngine;
using UnityEngine.UI;

public class ButtonsVisibility : MonoBehaviour
{
    [SerializeField] private GameObject buttonGroupObject;

    [SerializeField] private Image iconImage;

    [SerializeField] private Sprite visibilityOnIcon;

    [SerializeField] private Sprite visibilityOffIcon;

    private Button toggleVisibilityButton;

    private Image buttonImage;

    private bool showButtons = true;

    void Start()
    {
        toggleVisibilityButton = GetComponent<Button>();
        toggleVisibilityButton.onClick.AddListener(ToggleVisibility);
    }

    private void ToggleVisibility()
    {
        showButtons = !showButtons;
        iconImage.sprite = showButtons ? visibilityOffIcon : visibilityOnIcon;
        buttonGroupObject.SetActive(showButtons);
    }

}
