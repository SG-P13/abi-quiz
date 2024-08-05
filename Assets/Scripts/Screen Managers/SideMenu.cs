using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class SideMenu : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform sideMenuRectTransform;
    [SerializeField] private RectTransform sideMenuGearIcon;
    [SerializeField] private Button settingsButton;

    // Positioning
    private float width;
    private float startPositionX;
    private float startingAnchoredPositionX;
    private float onScreenPosition;
    private float offScreenPosition;
    private bool isOnScreen = false;

    // Animation
    private bool isInAnimation = false;
    private float targetPositionForCurrentAnimation = 0.0f;
    private float startPositionForCurrentAnimation = 0.0f;
    private float currentAnimationTime = 0.0f;
    private const float animationTime = 0.4f;

    void Start()
    {
        width = Screen.width;
        onScreenPosition = width * 0.5f;
        offScreenPosition = width * 1.5f;

        sideMenuRectTransform.anchoredPosition = new Vector2(offScreenPosition, 0);

        settingsButton.onClick.AddListener(ToggleMenu);
    }

    void Update()
    {
        if (isInAnimation)
        {
            currentAnimationTime += Time.deltaTime;
            if (currentAnimationTime < animationTime)
            {
                sideMenuRectTransform.anchoredPosition = new Vector2(
                    Mathf.Lerp(startPositionForCurrentAnimation, targetPositionForCurrentAnimation,
                    Mathf.Pow(currentAnimationTime / animationTime, 2.0f)), 0);
                RotateGear();
            } else
            {
                sideMenuRectTransform.anchoredPosition = new Vector2(targetPositionForCurrentAnimation, 0);  // Ensure exact final position
                RotateGear();
                isInAnimation = false;
            }
        }
    }

    private void RotateGear()
    {
        sideMenuGearIcon.rotation = Quaternion.Euler(0, 0, 360 * sideMenuRectTransform.anchoredPosition.x / (Mathf.Abs(onScreenPosition - offScreenPosition)));
    }

    public void OnDrag(PointerEventData eventData)
    {
        sideMenuRectTransform.anchoredPosition = new Vector2(
            Mathf.Clamp(startingAnchoredPositionX - (startPositionX - eventData.position.x), onScreenPosition, offScreenPosition),
            0);
        RotateGear();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StopAllCoroutines();
        startPositionX = eventData.position.x;
        startingAnchoredPositionX = sideMenuRectTransform.anchoredPosition.x;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        bool shouldOpen = isAfterHalfPoint();
        targetPositionForCurrentAnimation = shouldOpen ? onScreenPosition : offScreenPosition;
        startPositionForCurrentAnimation = sideMenuRectTransform.anchoredPosition.x;
        currentAnimationTime = 0.0f;
        isInAnimation = true;
        isOnScreen = shouldOpen; 
    }

    private bool isAfterHalfPoint()
    {
        return sideMenuRectTransform.anchoredPosition.x < width;
    }

    private void ToggleMenu()
    {
        targetPositionForCurrentAnimation = isOnScreen ? offScreenPosition : onScreenPosition;
        startPositionForCurrentAnimation = sideMenuRectTransform.anchoredPosition.x;
        currentAnimationTime = 0.0f;
        isInAnimation = true;
        isOnScreen = !isOnScreen; 
    }
}