using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    public int id;
    public string text;
    public int catalogueId;
    // TODO: questionImage
    public List<Answer> answers;

    public Question(int id, string text, int catalogueId, List<Answer> answers)
    {
        this.id = id;
        this.text = text;
        this.catalogueId = catalogueId;
        this.answers = answers;
    }
}
