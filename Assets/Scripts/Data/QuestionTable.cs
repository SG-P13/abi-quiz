using Mono.Data.Sqlite;
using System.Data;

public class QuestionTable
{
    private const string TABLE_NAME = "Question";
    private IDbConnection dbConnection;

    public QuestionTable(IDbConnection dbConnection)
    {
        this.dbConnection = dbConnection;
    }

    public void InsertData(Question question)
    {
        IDbCommand dbcmd = dbConnection.CreateCommand();
        
        dbcmd.CommandText = "INSERT INTO " + TABLE_NAME + " (CatalogueId, Text) VALUES (@CatalogueId, @Text)";
        dbcmd.Parameters.Add(new SqliteParameter("@CatalogueId", question.catalogueId));
        dbcmd.Parameters.Add(new SqliteParameter("@Text", question.text));
        dbcmd.ExecuteNonQuery();
        
    }
}
