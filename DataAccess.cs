using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace UI;
public static class DataAccess
{
    public static SQLiteConnection CreateConnection()
    {
        SQLiteConnection conn = new SQLiteConnection("Data Source=../../../HashesDB.sqlite;Version=3;");
        if (!File.Exists("../../../HashesDB.sqlite"))
        {
            SQLiteConnection.CreateFile("HashesDB.sqlite");
        }
        try
        {
            conn.Open();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return conn;
    }

    public static void CreateTable(SQLiteConnection conn, SQLiteCommand cmd)
    {
        string checkExists = "SELECT name FROM sqlite_master WHERE type='table' AND name='Hash';";
        SQLiteCommand checkCmd = new SQLiteCommand(checkExists, conn);
        SQLiteDataReader reader = checkCmd.ExecuteReader();
        if (!reader.HasRows)
        {
            string createSql = @"
            CREATE TABLE Hash(
                HashValue TEXT PRIMARY KEY
            );
            ";
            cmd.CommandText = createSql;
            cmd.ExecuteNonQuery();
        }
    }

    public static void InsertData(SQLiteConnection conn, SQLiteCommand cmd, string content)
    {
        cmd.CommandText = string.Format("INSERT INTO Hash(HashValue) VALUES ('{0}')", content);
        cmd.ExecuteNonQuery();
    }
}

