using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System;
using System.Collections.Generic;

public class SQLiteSetup : MonoBehaviour
{
    private const string databaseName = "Quiz_Database";
    private string dbConnectionString;
    private IDbConnection dbConnection;

    public static SQLiteSetup Instance { get; private set; }

    public CatalogueTable catalogueTable { get; private set; }
    public QuestionTable questionTable { get; private set; }
    public AnswerTable answerTable { get; private set; }
    public AnswerHistoryTable answerHistoryTable { get; private set; }
    public CatalogueSessionHistoryTable catalogueSessionHistoryTable { get; private set; }
    public AchievementTable achievementTable { get; private set; }
    public DailyTaskHistoryTable dailyTaskHistoryTable { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            dbConnectionString = "URI=file:" + Application.persistentDataPath + "/" + databaseName;
            dbConnection = new SqliteConnection(dbConnectionString);
            dbConnection.Open();

            IDbCommand enableForeignKeyCommand = dbConnection.CreateCommand();
            enableForeignKeyCommand.CommandText = "PRAGMA foreign_keys = ON;";
            enableForeignKeyCommand.ExecuteNonQuery();

            CreateTables();

            answerHistoryTable = new AnswerHistoryTable(dbConnection);
            questionTable = new QuestionTable(dbConnection);
            answerTable = new AnswerTable(dbConnection);
            catalogueSessionHistoryTable = new CatalogueSessionHistoryTable(dbConnection);
            catalogueTable = new CatalogueTable(dbConnection, questionTable, answerTable, answerHistoryTable, catalogueSessionHistoryTable);
            dailyTaskHistoryTable = new DailyTaskHistoryTable(dbConnection);
            achievementTable = new AchievementTable(dbConnection, dailyTaskHistoryTable, catalogueTable, catalogueSessionHistoryTable);

            AddInitialAchievementsIfNeeded();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void CreateTables()
    {
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = @"
            CREATE TABLE IF NOT EXISTS Catalogue (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT,
                CurrentQuestionId INTEGER,
                TotalTimeSpent INTEGER DEFAULT 0,
                SessionCount INTEGER DEFAULT 0,
                ErrorFreeSessionCount INTEGER DEFAULT 0,
                CompletedRandomQuizCount INTEGER DEFAULT 0,
                ErrorFreeRandomQuizCount INTEGER DEFAULT 0,
                FOREIGN KEY(CurrentQuestionId) REFERENCES Question(Id)
            );
            CREATE TABLE IF NOT EXISTS Question (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CatalogueId INTEGER,
                Text TEXT,
                Name TEXT,
                CorrectAnsweredCount INTEGER DEFAULT 0,
                TotalAnsweredCount INTEGER DEFAULT 0,
                FOREIGN KEY(CatalogueId) REFERENCES Catalogue(Id) ON DELETE CASCADE
            );
            CREATE TABLE IF NOT EXISTS Answer (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                QuestionId INTEGER,
                Text TEXT,
                IsCorrect BOOLEAN,
                FOREIGN KEY(QuestionId) REFERENCES Question(Id) ON DELETE CASCADE
            );
            CREATE TABLE IF NOT EXISTS AnswerHistory (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                QuestionId INTEGER,
                AnswerDate DATETIME,
                WasCorrect BOOLEAN,
                SessionId INTEGER,
                FOREIGN KEY(QuestionId) REFERENCES Question(Id) ON DELETE CASCADE,
                FOREIGN KEY(SessionId) REFERENCES CatalogueSessionHistory(Id) ON DELETE CASCADE
            );
            CREATE TABLE IF NOT EXISTS CatalogueSessionHistory (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                CatalogueId INTEGER,
                SessionDate DATETIME,
                TimeSpent INTEGER,
                IsCompleted BOOLEAN,
                IsErrorFree BOOLEAN DEFAULT TRUE,
                FOREIGN KEY(CatalogueId) REFERENCES Catalogue(Id) ON DELETE CASCADE
            );
            CREATE TABLE IF NOT EXISTS Achievement (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Grade TEXT NOT NULL DEFAULT 'None',
                Description TEXT NOT NULL,
                PopupText TEXT NOT NULL,
                IsAchieved BOOLEAN DEFAULT FALSE,
                AchievedAt DATETIME,
                UNIQUE(Name, Grade)
            );
            CREATE TABLE IF NOT EXISTS DailyTaskHistory (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TaskDate DATE NOT NULL UNIQUE,
                CorrectAnswersCount INTEGER DEFAULT 0,
                TaskCompleted BOOLEAN DEFAULT FALSE
            );
        ";
        dbCommand.ExecuteNonQuery();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            dbConnection.Close();

        }
    }

    private void AddInitialAchievementsIfNeeded()
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        dbcmd.CommandText = "SELECT COUNT(*) FROM Achievement";
        int achievementCount = Convert.ToInt32(dbcmd.ExecuteScalar());

        if (achievementCount == 0)
        {
            AddInitialAchievements();
        }
    }

    private void AddInitialAchievements()
    {
        List<Achievement> achievements = new()
        {
            // die description sollten wir wahrscheinlich zu dem Text �ndern, den wir dem Nutzer anzeigen wollen
            new Achievement("Flawless", AchievementPopup.Grade.Bronze, "Schlie�e zum ersten Mal einen gesamten Katalog ohne einen einzigen Fehler ab", "Du hast zum ersten Mal einen Katalog ohne Fehler abgeschlossen!", false, null),
            new Achievement("Flawless", AchievementPopup.Grade.Silver, "Meistere einen Katalog f�nfmal in Folge ohne Fehler", "Du hast einen Katalog f�nfmal ohne Fehler abgeschlossen!", false, null),
            new Achievement("Flawless", AchievementPopup.Grade.Gold, "Erreiche Perfektion, indem du einen Katalog zehnmal hintereinander fehlerfrei abschlie�t", "Du hast einen Katalog zehnmal ohne Fehler abgeschlossen!", false, null),
           
            new Achievement("Multitalent", AchievementPopup.Grade.Bronze, "Beweise deine Vielseitigkeit, indem du f�nf verschiedene Kataloge ohne Fehler meisterst", "Du hast f�nf Katalog-Durchl�ufe ohne Fehler abgeschlossen!", false, null),
            new Achievement("Multitalent", AchievementPopup.Grade.Silver, "Zeige deine umfassende Expertise, indem du zehn verschiedene Kataloge fehlerfrei abschlie�t", "Du hast zehn Katalog-Durchl�ufe ohne Fehler abgeschlossen!", false, null),
            new Achievement("Multitalent", AchievementPopup.Grade.Gold, "Demonstriere dein K�nnen, indem du 25 verschiedene Kataloge ohne einen einzigen Fehler meisterst", "Du hast 25 Katalog-Durchl�ufe ohne Fehler abgeschlossen!", false, null),

            new Achievement("Besserwisser", AchievementPopup.Grade.Bronze, "Beantworte 50 Fragen korrekt und zeige, dass du auf dem richtigen Weg bist", "Du hast 50 Fragen richtig beantwortet!", false, null),
            new Achievement("Besserwisser", AchievementPopup.Grade.Silver, "Beweise dein Wissen mit 500 richtig beantworteten Fragen", "Du hast 500 Fragen richtig beantwortet!", false, null),
            new Achievement("Besserwisser", AchievementPopup.Grade.Gold, "Zeige deine herausragenden Kenntnisse, indem du 1000 Fragen korrekt beantwortest", "Du hast 1000 Fragen richtig beantwortet!", false, null),

            // new Achievement("Streak", AchievementPopup.Grade.Bronze, "10 Tage in Folge 5 Fragen beantwortet", false, DateTime.Now),
            // new Achievement("Streak", AchievementPopup.Grade.Silver, "25 Tage in Folge 5 Fragen beantwortet", false, DateTime.Now),
            // new Achievement("Streak", AchievementPopup.Grade.Gold, "50 Tage in Folge 5 Fragen beantwortet", false, DateTime.Now),

            new Achievement("Daylies", AchievementPopup.Grade.Bronze, "Schlie�e f�nfmal den Daily Task erfolgreich ab", "Du hast den Daily Task f�nfmal abgeschlossen!", false, null),
            new Achievement("Daylies", AchievementPopup.Grade.Silver, "Erreiche das Ziel, indem du f�nfzehnmal den Daily Task meisterst", "Du hast den Daily Task f�nfzehnmal abgeschlossen!", false, null),
            new Achievement("Daylies", AchievementPopup.Grade.Gold, "Zeige Ausdauer, indem du drei�igmal den Daily Task erfolgreich abschlie�t", "Du hast den Daily Task drei�igmal abgeschlossen!", false, null),

            // new Achievement("Level", AchievementPopup.Grade.Bronze, "Level 10 erreicht", false, DateTime.Now),
            // new Achievement("Level", AchievementPopup.Grade.Silver, "Level 25 erreicht", false, DateTime.Now),
            // new Achievement("Level", AchievementPopup.Grade.Gold, "Level 50 erreicht", false, DateTime.Now),

            // new Achievement("Ersteller", AchievementPopup.Grade.Bronze, "1 Katalog erstellt", false, DateTime.Now),
            // new Achievement("Ersteller", AchievementPopup.Grade.Silver, "5 Kataloge erstellt", false, DateTime.Now),
            // new Achievement("Ersteller", AchievementPopup.Grade.Gold, "10 Kataloge erstellt", false, DateTime.Now),

            // new Achievement("Vernetzt", AchievementPopup.Grade.Bronze, "1 Katalog exportiert", false, DateTime.Now),
            // new Achievement("Vernetzt", AchievementPopup.Grade.Silver, "5 Kataloge exportiert", false, DateTime.Now),
            // new Achievement("Vernetzt", AchievementPopup.Grade.Gold, "10 Kataloge exportiert", false, DateTime.Now),

            new Achievement("Randomat", AchievementPopup.Grade.Bronze, "Schlie�e zehn Random Quiz Runden erfolgreich ab", "Du hast zehn Random Quiz Runden abgeschlossen!", false, null),
            new Achievement("Randomat", AchievementPopup.Grade.Silver, "Beweise deine Flexibilit�t, indem du f�nfzig Random Quiz Runden meisterst", "Du hast 50 Random Quiz Runden abgeschlossen!", false, null),
            new Achievement("Randomat", AchievementPopup.Grade.Gold, "Zeige deine Ausdauer, indem du hundert Random Quiz Runden erfolgreich abschlie�t", "DU hast 100 Random Quiz Runden abgeschlossen!", false, null),

            // new Achievement("Importeur", AchievementPopup.Grade.Bronze, "1 Katalog importiert", false, DateTime.Now),
            // new Achievement("Importeur", AchievementPopup.Grade.Silver, "5 Kataloge importiert", false, DateTime.Now),
            // new Achievement("Importeur", AchievementPopup.Grade.Gold, "10 Kataloge importiert", false, DateTime.Now),

            new Achievement("Hartn�ckig", AchievementPopup.Grade.Bronze, "Beantworte insgesamt 1000 Fragen", "Du hast 1000 Fragen beantwortet!", false, null),
            new Achievement("Hartn�ckig", AchievementPopup.Grade.Silver, "Beantworte insgesamt 5000 Fragen", "Du hast 5000 Fragen beantwortet!", false, null),
            new Achievement("Hartn�ckig", AchievementPopup.Grade.Gold, "Beantworte insgesamt 10000 Fragen", "Du hast 10000 Fragen beantwortet!", false, null),

            new Achievement("Fokus", AchievementPopup.Grade.Bronze, "Verbringe 30 Minuten in einem einzigen Katalog", "Du hast 30 Minuten in einem Katalog verbracht!", false, null),
            new Achievement("Fokus", AchievementPopup.Grade.Silver, "Verbringe 60 Minuten in einem einzigen Katalog", "Du hast 60 Minuten in einem Katalog verbracht!", false, null),
            new Achievement("Fokus", AchievementPopup.Grade.Gold, "Verbringe 120 Minuten in einem einzigen Katalog", "Du hast 120 Minuten in einem Katalog verbracht!", false, null),

            new Achievement("Zeitmanagement", AchievementPopup.Grade.Bronze, "Verbringe insgesamt 300 Minuten in Katalogen", "Du hast insgesamt 300 Minuten in Katalogen verbracht!", false, null),
            new Achievement("Zeitmanagement", AchievementPopup.Grade.Silver, "Verbringe insgesamt 600 Minuten in Katalogen", "Du hast insgesamt 600 Minuten in Katalogen verbracht!", false, null),
            new Achievement("Zeitmanagement", AchievementPopup.Grade.Gold, "Verbringe insgesamt 1200 Minuten in Katalogen", "Du hast insgesamt 1200 Minuten in Katalogen verbracht!", false, null),

            new Achievement("Random Flawless", AchievementPopup.Grade.Bronze, "Schlie�e zehn Random Quiz Runden ohne Fehler ab", "Du hast zehn Random Quiz Runden ohne Fehler abgeschlossen!", false, null),
            new Achievement("Random Flawless", AchievementPopup.Grade.Silver, "Schlie�e 25 Random Quiz Runden ohne Fehler ab", "Du hast 25 Random Quiz Runden ohne Fehler abgeschlossen!", false, null),
            new Achievement("Random Flawless", AchievementPopup.Grade.Gold, "Schlie�e 50 Random Quiz Runden ohne Fehler ab", "Du hast 50 Random Quiz Runden ohne Fehler abgeschlossen!", false, null),

            new Achievement("Intensiv", AchievementPopup.Grade.Bronze, "Verbringe 15 Minuten an einem Tag in Katalogen", "Du hast 15 Minuten an einem Tag in Katalogen verbracht!", false, null),
            new Achievement("Intensiv", AchievementPopup.Grade.Silver, "Verbringe 30 Minuten an einem Tag in Katalogen", "Du hast 30 Minuten an einem Tag in Katalogen verbracht!", false, null),
            new Achievement("Intensiv", AchievementPopup.Grade.Gold, "Verbringe 60 Minuten an einem Tag in Katalogen", "Du hast 60 Minuten an einem Tag in Katalogen verbracht!", false, null),

            new Achievement("Flei�ig", AchievementPopup.Grade.Bronze, "Schlie�e 25 Quiz-Durchl�ufe ab", "Du hast 25 Quiz-Durchl�ufe abgeschlossen!", false, null),
            new Achievement("Flei�ig", AchievementPopup.Grade.Silver, "Schlie�e 50 Quiz-Durchl�ufe ab", "Du hast 25 Quiz-Durchl�ufe abgeschlossen!", false, null),
            new Achievement("Flei�ig", AchievementPopup.Grade.Gold, "Schlie�e 100 Quiz-Durchl�ufe ab", "Du hast 25 Quiz-Durchl�ufe abgeschlossen!", false, null)
        };

        foreach (Achievement achievement in achievements)
        {
            achievementTable.AddAchievement(achievement);
        }
    }
}
