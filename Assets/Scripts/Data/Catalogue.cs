using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Catalogue
{
    public int id;
    public string name;
    public List<Question> questions;

    public Catalogue (int id, string name, List<Question> questions)
    {
        this.id = id;
        this.name = name;
        this.questions = questions;
    }
}
