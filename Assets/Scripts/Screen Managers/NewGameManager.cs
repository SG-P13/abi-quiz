using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NewGameManager : MonoBehaviour
{

    [SerializeField] private TMP_Dropdown catalogueSelection;
    [SerializeField] private Button startLinearRound;
    [SerializeField] private ButtonNavigation startLinearRoundNavigation;
    [SerializeField] private Button startRandomRound;
    [SerializeField] private ButtonNavigation startRandomRoundNavigation;

    [SerializeField] private GameObject catalogueButtonPrefab;
    [SerializeField] private Transform buttonContainer;

    [HideInInspector] public static CatalogueTable catalogueTable;
    [HideInInspector] public static int catalogueCount;
    [HideInInspector] public static List<Catalogue> catalogues;
    [HideInInspector] public static Global.GameMode gameMode;

    void Start()
    {
        // Failsave
        if (SceneManager.GetActiveScene().name != "NewGame")
        {
            print("ERROR [NewGameManager.cs:Start()]: Dont use this script in any scene other than \"" + SceneManager.GetActiveScene().name + "\"!");
            return;
        }
        catalogueTable = SQLiteSetup.Instance.catalogueTable;
        catalogues = catalogueTable.FindAllCatalogues();
        catalogueCount = catalogues.Count;
        SetCatalogueButtons();
        gameMode = Global.CurrentQuestionRound.gameMode;
        Debug.Log(gameMode);
    }

    private void SetCatalogueButtons()
    {
        for (int i = 0; i < catalogues.Count; i++)
        {
            GameObject catalogueButton = Instantiate(catalogueButtonPrefab, buttonContainer);
            TextMeshProUGUI buttonLabel = catalogueButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonLabel.text = catalogues[i].name;
        }
    }
}
