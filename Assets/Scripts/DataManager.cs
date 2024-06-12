using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class DataManager
{

    public static List<QuestionResult> QuestionResults = new();
    
    public struct QuestionRound
    {
        public int CatalogueIndex; // Index in der Liste aller Kataloge
        public List<int> Questions; // Indices der Fragen im Katalog
        public int QuestionCounter; // Z�hlt hoch bis AnzahlFragenProFragerunde, danach endet die Fragerunde.
        public int QuestionLimit; // �Number of questions in a random quiz
    }

    public struct QuestionResult
    {
        public string questionText;
        public string answerText;
        // TODO: add questionImage
        // TODO: add answerImage
        public bool isCorrect;
    }

    public static void AddAnswer(int questionIndex, int answerIndex, Catalogue catalogue)
    {
        bool isCorrect = answerIndex == 0;
        Question question = catalogue.questions[questionIndex];
        Answer answer = question.answers[answerIndex];

        QuestionResults.Add(new QuestionResult
        {
            questionText = question.text,
            answerText = answer.text,
            // TODO: questionImage = question.image
            // TODO: answerImage = answer.image
            isCorrect = isCorrect
        });
    }

    public static void ClearResults()
    {
        QuestionResults.Clear();
    }

}
