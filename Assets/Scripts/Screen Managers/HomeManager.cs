using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    [SerializeField] private Button startDailyTask;
    [SerializeField] private Background background;
    private string targetScene;

    private int catalogueCount;
    CatalogueTable catalogueTable;
    List<Catalogue> catalogues;

    string currentDate;


    void Start()
    {
        catalogueTable = SQLiteSetup.Instance.catalogueTable;
        catalogues = catalogueTable.FindAllCatalogues();
        catalogueCount = catalogues.Count;

        currentDate = DateTime.Now.ToString("yyyy-MM-dd");
        
        if (IsNewDay())
        {
            ResetDailyTask();
            Debug.Log("Daily Task reset");
        }
    }

  
    public void StartDailyTaskClickedEvent()
    {
        Debug.Log("Daily Task has already been completed: " + IsDailyTaskCompleted());
        if (IsDailyTaskCompleted())
        {
            LoadDailyTaskScene("DailyTaskEvaluation");
            return;
        }

        // load chosen catalogue into global data
        Global.CurrentDailyTask.catalogueIndex = UnityEngine.Random.Range(1, catalogueCount + 1);
        Debug.Log("Chose Catalogue: " + Global.CurrentDailyTask.catalogueIndex);
        Global.CurrentDailyTask.catalogue = catalogueTable.FindCatalogueById(Global.CurrentDailyTask.catalogueIndex);
        PlayerPrefs.SetInt("DailyTaskCatalogueId", Global.CurrentDailyTask.catalogueIndex);
        PlayerPrefs.Save();
        // initialize daily task
        Global.CurrentDailyTask.questions = new();
        int[] iota = Enumerable.Range(0, Global.CurrentDailyTask.catalogue.questions.Count).ToArray(); // [0, 1, 2, ..., Count - 1] (question indices)
        Functions.Shuffle(iota); // shuffle question indices
        Global.CurrentDailyTask.questionLimit = Mathf.Min(Global.DailyTaskSize, Global.CurrentDailyTask.catalogue.questions.Count);
        for (int i = 0; i < Global.CurrentDailyTask.questionLimit; i++) // select first n questions of randomized questions
        {
            Global.CurrentDailyTask.questions.Add(iota[i]);
        }

        // start daily task
        Global.InsideQuestionRound = true;
        LoadDailyTaskScene("DailyTask");
    }

    public bool IsNewDay()
    {
        string currentDate = DateTime.Now.ToString("yyy-MM-dd");
        string lastResetDate = PlayerPrefs.GetString(Global.LastResetDateKey, "");
        if (currentDate != lastResetDate)
        {
            return true;
        }
        return false;
    }


    public void ResetDailyTask()
    {
        Global.CurrentDailyTask = new DataManager.DailyTask();
        PlayerPrefs.SetString(Global.LastResetDateKey, currentDate);
        PlayerPrefs.SetString(Global.IsDailyTaskCompletedKey, "false");
        PlayerPrefs.Save();
    }


    public bool IsDailyTaskCompleted()
    {
        return PlayerPrefs.GetString(Global.IsDailyTaskCompletedKey) == "true"; ;
    }

    public void LoadDailyTaskScene(string sceneName)
    {
        targetScene = sceneName;
        StartSceneTransition();
    }

    public void LoadLinearGameSelection()
    {
        targetScene = "NewGame";
        Global.CurrentQuestionRound.gameMode = Global.GameMode.LinearQuiz;
        StartSceneTransition();
    }

    public void LoadRandomGameSelection()
    {
        targetScene = "NewGame";
        Global.CurrentQuestionRound.gameMode = Global.GameMode.RandomQuiz;
        StartSceneTransition();
    }

    public void StartSceneTransition()
    {
        if (background != null)
        {
            float timeNeeded = background.TriggerEndSequence();
            Invoke(nameof(LoadSceneInternal), timeNeeded);
        }
        else
        {
            LoadSceneInternal();
        }
    }

    public void LoadSceneInternal()
    {
        SceneManager.LoadScene(targetScene);
    }

    

}
