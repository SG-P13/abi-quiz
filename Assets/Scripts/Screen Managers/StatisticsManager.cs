using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class TogglePanel : MonoBehaviour
{
    private GameObject panel;
    private bool isPanelActive = false;

    public void SetPanel(GameObject panelToToggle)
    {
        panel = panelToToggle;
        panel.SetActive(isPanelActive);
    }

    public void Toggle()
    {
        isPanelActive = !isPanelActive;
        panel.SetActive(isPanelActive);
    }
}


public class StatisticsManager : MonoBehaviour
{
    public CataloguesManager  cataloguesManager;
    CatalogueTable catalogueTable;
    List<Catalogue> catalogues;


    void Start()
    {
        // Failsave
        if (SceneManager.GetActiveScene().name != "Statistics")
        {
            print("ERROR [StatisticsManager.cs:Start()]: Dont use this script in any scene other than \"" + SceneManager.GetActiveScene().name + "\"!");
        }
        catalogueTable = SQLiteSetup.Instance.catalogueTable;
        catalogues = catalogueTable.FindAllCatalogues();
        setContents();
    }
    void setContents()
    {
        foreach (Catalogue catalogue in catalogues)
        {
            // Create button
            GameObject button = Instantiate(cataloguesManager.buttonPrefab, cataloguesManager.buttonParent);
            button.GetComponentInChildren<Text>().text = catalogue.name;

            // Create panel
            GameObject panel = Instantiate(cataloguesManager.panelPrefab, cataloguesManager.panelParent);
            panel.SetActive(false);

            // Set up TogglePanel component
            TogglePanel togglePanel = button.AddComponent<TogglePanel>();
            togglePanel.SetPanel(panel);

            // Add button click listener
            Button btn = button.GetComponent<Button>();
            btn.onClick.AddListener(togglePanel.Toggle);
        }
    }
}
