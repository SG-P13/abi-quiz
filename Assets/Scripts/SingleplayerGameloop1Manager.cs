using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DataManager;

public class SingleplayerGameloop1Manager : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI Fragenummer;
    [SerializeField] private UnityEngine.UI.Button QButton;
    [SerializeField] private UnityEngine.UI.Button AButton1;
    [SerializeField] private UnityEngine.UI.Button AButton2;
    [SerializeField] private UnityEngine.UI.Button AButton3;
    [SerializeField] private UnityEngine.UI.Button AButton4;
    private TextMeshProUGUI QButton_Label;
    private TextMeshProUGUI AButton1_Label;
    private TextMeshProUGUI AButton2_Label;
    private TextMeshProUGUI AButton3_Label;
    private TextMeshProUGUI AButton4_Label;
    private RectTransform QButton_Transform;
    private RectTransform AButton1_Transform;
    private RectTransform AButton2_Transform;
    private RectTransform AButton3_Transform;
    private RectTransform AButton4_Transform;

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "Screen_SingleplayerGameloop_1")
        {
            print("ERROR [NewGameManager.cs:Start()]: Dont use this script in any scene other than \"" + SceneManager.GetActiveScene().name + "\"!");
        }
        QButton_Label = QButton.GetComponentInChildren<TextMeshProUGUI>();
        AButton1_Label = AButton1.GetComponentInChildren<TextMeshProUGUI>();
        AButton2_Label = AButton2.GetComponentInChildren<TextMeshProUGUI>();
        AButton3_Label = AButton3.GetComponentInChildren<TextMeshProUGUI>();
        AButton4_Label = AButton4.GetComponentInChildren<TextMeshProUGUI>();
        QButton_Transform = QButton.transform.GetComponent<RectTransform>();
        AButton1_Transform = AButton1.transform.GetComponent<RectTransform>();
        AButton2_Transform = AButton2.transform.GetComponent<RectTransform>();
        AButton3_Transform = AButton3.transform.GetComponent<RectTransform>();
        AButton4_Transform = AButton4.transform.GetComponent<RectTransform>();
        SetRandomizedPositions();
        SetContents();
    }

    private void Advance(int chosenAnswer)
    {
        Global.AktuelleFragerunde.ChosenAnswers[Global.AktuelleFragerunde.QuestionCounter] = chosenAnswer;
        if (Global.AktuelleFragerunde.QuestionCounter == Global.NumQuestionsPerRound - 1)
        {
            // Alle Fragen beantwortet
            SceneManager.LoadScene("Screen_SingleplayerGameloop_2");
        } else
        {
            Global.AktuelleFragerunde.QuestionCounter += 1;
            SetContents();
            SetRandomizedPositions();
        }
    }

    public void AButton1ClickEvent()
    {
        if (CurrentQuestionIsOutOfBounds())
        {
            return;
        }
        Advance(0);
    }

    public void AButton2ClickEvent()
    {
        if (CurrentQuestionIsOutOfBounds())
        {
            return;
        }
        Advance(1);
    }

    public void AButton3ClickEvent()
    {
        if (CurrentQuestionIsOutOfBounds())
        {
            return;
        }
        Advance(2);
    }

    public void AButton4ClickEvent()
    {
        if (CurrentQuestionIsOutOfBounds())
        {
            return;
        }
        Advance(3);
    }

    private void SetRandomizedPositions()
    {
        Rect[] positions = { 
            UIDesign.Positions.Buttons.SingleplayerGameloop1.Antwort1,
            UIDesign.Positions.Buttons.SingleplayerGameloop1.Antwort2,
            UIDesign.Positions.Buttons.SingleplayerGameloop1.Antwort3,
            UIDesign.Positions.Buttons.SingleplayerGameloop1.Antwort4
        };
        Functions.Shuffle(positions);
        SetPos(ref QButton_Transform, UIDesign.Positions.Buttons.SingleplayerGameloop1.Frage);
        SetPos(ref AButton1_Transform, positions[0]);
        SetPos(ref AButton2_Transform, positions[1]);
        SetPos(ref AButton3_Transform, positions[2]);
        SetPos(ref AButton4_Transform, positions[3]);
    }

    private void SetContents()
    {
        if (CurrentQuestionIsOutOfBounds())
        {
            return;
        }
        Question currentQuestion = Storage.Catalogues[Global.AktuelleFragerunde.CatalogueIndex].questions[Global.AktuelleFragerunde.Questions[Global.AktuelleFragerunde.QuestionCounter]];
        QButton_Label.text = currentQuestion.question.text;
        AButton1_Label.text = currentQuestion.answers[0].text;
        AButton2_Label.text = currentQuestion.answers[1].text;
        AButton3_Label.text = currentQuestion.answers[2].text;
        AButton4_Label.text = currentQuestion.answers[3].text;
        Fragenummer.text = 
            "Frage " + Global.AktuelleFragerunde.QuestionCounter + "\n" + 
            "(Katalog: " + Global.AktuelleFragerunde.CatalogueIndex + 
            ", Frage: " + Global.AktuelleFragerunde.Questions[Global.AktuelleFragerunde.QuestionCounter] + 
            ")";
    }

    private bool CurrentQuestionIsOutOfBounds()
    {
        if(!Global.InsideFragerunde)
        {
            print("ERROR [NewGameManager.cs:Start()]: Global.InsideFragerunde == false, wie bist du �berhaupt hier gelandet?!");
            return true;
        }
        if (Global.AktuelleFragerunde.CatalogueIndex >= Storage.Catalogues.Count)
        {
            print("ERROR [ButtonManager.cs.SetContents()]: Global.AktuelleFragerunde.CatalogueIndex >= DataManager.Storage.Catalogues.Count");
            return true;
        }
        if (Global.AktuelleFragerunde.QuestionCounter >= Global.AktuelleFragerunde.Questions.Count)
        {
            print("ERROR [ButtonManager.cs.SetContents()]: Global.AktuelleFragerunde.QuestionCounter >= Global.AktuelleFragerunde.Questions.Count");
            return true;
        }
        if (Global.AktuelleFragerunde.Questions[Global.AktuelleFragerunde.QuestionCounter] >= Storage.Catalogues[Global.AktuelleFragerunde.CatalogueIndex].questions.Count)
        {
            print("ERROR [ButtonManager.cs.SetContents()]: Global.AktuelleFragerunde.Questions[Global.AktuelleFragerunde.QuestionCounter] >= DataManager.Storage.Catalogues[Global.AktuelleFragerunde.CatalogueIndex].questions.Count");
            return true;
        }
        return false;
    }
    private void SetPos(ref RectTransform t, Rect rect)
    {
        SetPos(ref t, rect.x, rect.y, rect.width, rect.height);
    }

    private void SetPos(ref RectTransform t, float x, float y, float w, float h)
    {
        // MS:
        // x, y - Relative Bildschirmposition f�r das CENTER des Objekts
        // w, h - Prozentsatz der Bildschirmbreite & H�he
        // Koordinaten:
        // 0/1 ------- 1/1
        //  |           |
        //  |           |
        //  |           |
        //  |  0.5/0.5  |
        //  |           |
        //  |           |
        //  |           |
        // 0/0 ------- 1/0
        t.offsetMin = new Vector2(-w / 2.0f * UIDesign.Positions.Global.width, -h / 2.0f * UIDesign.Positions.Global.height);
        t.offsetMax = new Vector2(w / 2.0f * UIDesign.Positions.Global.width, h / 2.0f * UIDesign.Positions.Global.height);
        t.anchoredPosition = new Vector2((x - 0.5f) * UIDesign.Positions.Global.width, (y - 0.5f) * UIDesign.Positions.Global.height);
    }
}
