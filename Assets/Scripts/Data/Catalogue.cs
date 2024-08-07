using System.Collections.Generic;

[System.Serializable]
public class Catalogue
{
    public int id;
    public string name;
    public int currentQuestionId;
    public int totalTimeSpent;
    public List<Question> questions;
    public List<CatalogueSessionHistory> sessionHistories;

    public Catalogue (int id, string name, int currentQuestionId, int totalTimeSpent, List<Question> questions, List<CatalogueSessionHistory> sessionHistories)
    {
        this.id = id;
        this.name = name;
        this.currentQuestionId = currentQuestionId;
        this.totalTimeSpent = totalTimeSpent;
        this.questions = questions;
        this.sessionHistories = sessionHistories;
    }
}
