using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public Button catButtonGesundheit;
    public Button catButtonErziehung;
    public GameObject panelGesundheit;
    public GameObject panelErziehung;
    public float panelWidth = 800f;  // Set desired panel width
    public float panelHeight = 200f; // Set desired panel height
    public float buttonSpacing = 20f; // Space between buttons and panels

    private GameObject activePanel; // Track the currently active panel

    private void Start()
    {
        // Ensure the panels are initially hidden
        panelGesundheit.SetActive(false);
        panelErziehung.SetActive(false);

        // Add listeners to buttons
        catButtonGesundheit.onClick.AddListener(() => TogglePanel(panelGesundheit));
        catButtonErziehung.onClick.AddListener(() => TogglePanel(panelErziehung));

        // Position the panels below the buttons
        PositionPanelBelowButton(catButtonGesundheit, panelGesundheit);
        PositionPanelBelowButton(catButtonErziehung, panelErziehung);
        
        // Adjust catButtonErziehung position initially
        AdjustButtonPosition();
    }

    private void TogglePanel(GameObject panel)
    {
        if (activePanel == panel)
        {
            // If the clicked panel is already active, hide it
            panel.SetActive(false);
            activePanel = null; // Clear the active panel
        }
        else
        {
            // Hide the currently active panel if there is one
            if (activePanel != null)
            {
                activePanel.SetActive(false);
            }

            // Show the clicked panel and set it as active
            panel.SetActive(true);
            activePanel = panel;
        }

        // Adjust button positions
        AdjustButtonPosition();
    }

    private void PositionPanelBelowButton(Button button, GameObject panel)
    {
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        RectTransform panelRect = panel.GetComponent<RectTransform>();

        // Set the size of the panel
        panelRect.sizeDelta = new Vector2(panelWidth, panelHeight);

        // Set the anchor of the panel to the top left of the button
        panelRect.anchorMin = new Vector2(0, 1);
        panelRect.anchorMax = new Vector2(0, 1);

        // Position the panel below the button
        panelRect.anchoredPosition = new Vector2(buttonRect.anchoredPosition.x, buttonRect.anchoredPosition.y - buttonRect.sizeDelta.y - buttonSpacing);
    }

    private void AdjustButtonPosition()
    {
        RectTransform buttonGesundheitRect = catButtonGesundheit.GetComponent<RectTransform>();
        RectTransform buttonErziehungRect = catButtonErziehung.GetComponent<RectTransform>();

        // Adjust position based on the state of panelGesundheit
        if (panelGesundheit.activeSelf)
        {
            // Position buttonErziehung below panelGesundheit
            RectTransform panelGesundheitRect = panelGesundheit.GetComponent<RectTransform>();
            buttonErziehungRect.anchoredPosition = new Vector2(buttonGesundheitRect.anchoredPosition.x, panelGesundheitRect.anchoredPosition.y - panelGesundheitRect.sizeDelta.y - buttonSpacing);
        }
        else
        {
            // Position buttonErziehung below buttonGesundheit
            buttonErziehungRect.anchoredPosition = new Vector2(buttonGesundheitRect.anchoredPosition.x, buttonGesundheitRect.anchoredPosition.y - buttonGesundheitRect.sizeDelta.y - buttonSpacing);
        }
    }
}