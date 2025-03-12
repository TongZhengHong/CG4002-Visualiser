using UnityEngine;
using UnityEngine.UI;

public class FloorVisibility: MonoBehaviour
{
    [SerializeField] private GameObject floorObject;

    [SerializeField] private Image iconImage;

    [SerializeField] private Sprite visibilityOnIcon;

    [SerializeField] private Sprite visibilityOffIcon;

    private Button toggleVisibilityButton;

    private Image buttonImage;

    private bool showFloor = true;

    private MeshRenderer meshRenderer;

    void Start()
    {
        toggleVisibilityButton = GetComponent<Button>();
        toggleVisibilityButton.onClick.AddListener(ToggleVisibility);
        meshRenderer = floorObject.GetComponent<MeshRenderer>();
    }

    private void ToggleVisibility()
    {
        showFloor = !showFloor;
        iconImage.sprite = showFloor ? visibilityOffIcon : visibilityOnIcon;
        meshRenderer.enabled = showFloor;
    }

}
