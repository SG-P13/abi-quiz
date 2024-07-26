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

    private int catalogueCount;
    CatalogueTable catalogueTable;
    List<Catalogue> catalogues;
    private Global.GameMode gameMode;

    void Start()
    {
        // Failsave
        if (SceneManager.GetActiveScene().name != "NewGame")
        {
            print("ERROR [NewGameManager.cs:Start()]: Dont use this script in any scene other than \"" + SceneManager.GetActiveScene().name + "\"!");
        }
        catalogueTable = SQLiteSetup.Instance.catalogueTable;
        catalogues = catalogueTable.FindAllCatalogues();
        catalogueCount = catalogues.Count;
        SetContents();
        gameMode = Global.CurrentQuestionRound.gameMode;
        Debug.Log(gameMode);
    }

    private void SetContents()
    {
        catalogueSelection.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new();
        for (int i = 0; i < catalogues.Count; i++)
        {
            Catalogue catalogue = catalogues[i];
            options.Add(new(catalogue.name));
        }
        if (options.Count == 0)
        {
            options.Add(new("Nicht verf�gbar"));
        }
        catalogueSelection.AddOptions(options);
        CatalogueSelectionChangedEvent();
    }

    public void CatalogueSelectionChangedEvent()
    {
        if (catalogueCount == 0)
        {
            Global.CurrentQuestionRound.catalogueIndex = 0;
            startLinearRound.interactable = false;
            startRandomRound.interactable = false;
        }
        else
        {
            Global.CurrentQuestionRound.catalogueIndex = catalogueTable.FindCatalogueByName(catalogueSelection.options[catalogueSelection.value].text).id;
            startLinearRound.interactable = true;
            startRandomRound.interactable = true;
        }
    }

    public void StartQuiz()
    {
        switch(gameMode)
        {
            case Global.GameMode.LinearQuiz:
                StartLinearRoundClickedEvent();
                break;
            case Global.GameMode.RandomQuiz:
                StartRandomRoundClickedEvent(); 
                break;
            default: 
                break;
        }
    }

    public void StartLinearRoundClickedEvent()
    {
        // invalid catalogue index
        if (!catalogues.Any(catalogue => catalogue.id == Global.CurrentQuestionRound.catalogueIndex))
        {
            print("ERROR: Fragerunde mit Katalognummer " + Global.CurrentQuestionRound.catalogueIndex + " ist OutOfBounds. Es gibt " + catalogueCount + " Fragenkataloge.");
            return;
        }

        // start quiz round
        Global.CurrentQuestionRound.catalogue = catalogueTable.FindCatalogueById(Global.CurrentQuestionRound.catalogueIndex);
        Global.InsideQuestionRound = true;
        startLinearRoundNavigation.LoadScene("LinearQuiz");
    }

    public void StartRandomRoundClickedEvent()
    {
        // check if chosen catalogue index is out of bounds
        if (!catalogues.Any(catalogue => catalogue.id == Global.CurrentQuestionRound.catalogueIndex))
        {
            print("ERROR [NewGameManager.cs.StartZufallsRundeClickedEvent()]: Fragerunde mit Katalognummer " 
                + Global.CurrentQuestionRound.catalogueIndex + " ist OutOfBounds. Es gibt " 
                + catalogueCount + " Fragenkataloge.");
            return;
        }

        // load chosen catalogue into global data
        Global.CurrentQuestionRound.catalogue = catalogueTable.FindCatalogueById(Global.CurrentQuestionRound.catalogueIndex);
        int catalogueSize = Global.CurrentQuestionRound.catalogue.questions.Count;

        // initialize question round
        Global.CurrentQuestionRound.questions = new();
        int[] iota = Enumerable.Range(0, Global.CurrentQuestionRound.catalogue.questions.Count).ToArray(); // [0, 1, 2, ..., Count - 1] (question indices)
        Functions.Shuffle(iota); // shuffle question indices
        Global.CurrentQuestionRound.questionLimit = Mathf.Min(Global.RandomQuizSize, catalogueSize);
        for (int i = 0; i < Global.CurrentQuestionRound.questionLimit; i++) // select first n questions of randomized questions
        {
            Global.CurrentQuestionRound.questions.Add(iota[i]);
        }
        Global.InsideQuestionRound = true;
        startRandomRoundNavigation.LoadScene("RandomQuiz");
    }
}
