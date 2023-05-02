﻿using ConsoleTableExt;
using LucianoNicolasArrieta.Flashcards.DTOs;
using LucianoNicolasArrieta.Flashcards.Model;
using System.Configuration;
using System.Data.SqlClient;

namespace LucianoNicolasArrieta.Flashcards.Persistence
{
    public class FlashcardRepository
    {
        private string connectionString = ConfigurationManager.AppSettings.Get("connectionString");

        public void Insert(Flashcard flashcard, int stackId)
        {
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Flashcard (Question, Answer, StackId) VALUES (@question, @answer, @stackid)";
                SqlCommand command = new SqlCommand(query, myConnection);

                myConnection.Open();

                command.Parameters.AddWithValue("@question", flashcard.Question);
                command.Parameters.AddWithValue("@answer", flashcard.Answer);
                command.Parameters.AddWithValue("@stackid", stackId);
                command.ExecuteNonQuery();
            }

            Console.Clear();
            Console.WriteLine($"Flashcard created successfully!");
        }

        public void PrintAllFromStack(int id)
        {
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                string query = $"SELECT * FROM Flashcard WHERE StackId='{id}'";
                SqlCommand command = new SqlCommand(query, myConnection);

                myConnection.Open();
                SqlDataReader reader = command.ExecuteReader();

                List<FlashcardDTO> flashcards = new List<FlashcardDTO>();
                var tableData = new List<List<object>>();
                int i = 1;
                while (reader.Read())
                {
                    FlashcardDTO aux = new FlashcardDTO();
                    aux.Id = Convert.ToInt32(reader[0]);
                    aux.Question = reader[2].ToString();
                    aux.Answer = reader[3].ToString();

                    // TODO: This can may not be neccesary, check later
                    flashcards.Add(aux);

                    List<object> row = new List<object> { $"{i}", aux.Question, aux.Answer };
                    tableData.Add(row);
                    i++;
                }

                ConsoleTableBuilder.From(tableData)
                    .WithCharMapDefinition(
                        CharMapDefinition.FramePipDefinition,
                        new Dictionary<HeaderCharMapPositions, char> {
                        {HeaderCharMapPositions.TopLeft, '╒' },
                        {HeaderCharMapPositions.TopCenter, '╤' },
                        {HeaderCharMapPositions.TopRight, '╕' },
                        {HeaderCharMapPositions.BottomLeft, '╞' },
                        {HeaderCharMapPositions.BottomCenter, '╪' },
                        {HeaderCharMapPositions.BottomRight, '╡' },
                        {HeaderCharMapPositions.BorderTop, '═' },
                        {HeaderCharMapPositions.BorderRight, '│' },
                        {HeaderCharMapPositions.BorderBottom, '═' },
                        {HeaderCharMapPositions.BorderLeft, '│' },
                        {HeaderCharMapPositions.Divider, '│' },
                        })
                    .WithTitle("FLASHCARDS")
                    .ExportAndWriteLine(TableAligntment.Left);
            }
        }

        public FlashcardDTO GetFlashcard(int id)
        {
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                string query = "SELECT * FROM Flashcard";
                SqlCommand command = new SqlCommand(query, myConnection);

                myConnection.Open();
                SqlDataReader reader = command.ExecuteReader();

                List<FlashcardDTO> flashcards = new List<FlashcardDTO>();
                while (reader.Read())
                {
                    FlashcardDTO aux = new FlashcardDTO();
                    aux.Id = Convert.ToInt32(reader[0]);
                    aux.Question = reader[2].ToString();
                    aux.Answer = reader[3].ToString();

                    flashcards.Add(aux);
                }

                if (flashcards.Count <= id - 1)
                {
                    Menu menu = new Menu();
                    Console.Clear();
                    Console.WriteLine("Please enter a valid ID. Try again");
                    menu.RunMainMenu();
                }

                return flashcards[id - 1];
            }
        }

        public void Delete(int id)
        {
            FlashcardDTO flashcard = GetFlashcard(id);

            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                string query = $"DELETE FROM Flashcard WHERE Id='{flashcard.Id}'";
                SqlCommand command = new SqlCommand(query, myConnection);

                myConnection.Open();

                command.ExecuteNonQuery();
            }

            Console.Clear();
            Console.WriteLine("Flashcard deleted successfully!");
        }

        public void PrintXAmount(int amount, int stackid)
        {
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                string query = $"SELECT TOP {amount} Question, Answer FROM Flashcard WHERE StackId='{stackid}' ORDER BY NEWID()";
                SqlCommand command = new SqlCommand(query, myConnection);

                myConnection.Open();
                SqlDataReader reader = command.ExecuteReader();

                List<FlashcardDTO> flashcards = new List<FlashcardDTO>();
                var tableData = new List<List<object>>();
                int i = 1;
                while (reader.Read())
                {
                    FlashcardDTO aux = new FlashcardDTO();
                    aux.Question = reader[0].ToString();
                    aux.Answer = reader[1].ToString();

                    // TODO: This can may not be neccesary, check later
                    flashcards.Add(aux);

                    List<object> row = new List<object> { aux.Question, aux.Answer };
                    tableData.Add(row);
                    i++;
                }

                ConsoleTableBuilder.From(tableData)
                    .WithCharMapDefinition(
                        CharMapDefinition.FramePipDefinition,
                        new Dictionary<HeaderCharMapPositions, char> {
                        {HeaderCharMapPositions.TopLeft, '╒' },
                        {HeaderCharMapPositions.TopCenter, '╤' },
                        {HeaderCharMapPositions.TopRight, '╕' },
                        {HeaderCharMapPositions.BottomLeft, '╞' },
                        {HeaderCharMapPositions.BottomCenter, '╪' },
                        {HeaderCharMapPositions.BottomRight, '╡' },
                        {HeaderCharMapPositions.BorderTop, '═' },
                        {HeaderCharMapPositions.BorderRight, '│' },
                        {HeaderCharMapPositions.BorderBottom, '═' },
                        {HeaderCharMapPositions.BorderLeft, '│' },
                        {HeaderCharMapPositions.Divider, '│' },
                        })
                    .WithTitle($"{amount} FLASHCARDS")
                    .ExportAndWriteLine(TableAligntment.Left);
            }
        }

        public void Update(int id, string question, string answer)
        {
            FlashcardDTO flashcard = GetFlashcard(id);

            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                string query = $"UPDATE Flashcard SET Question = '{question}', Answer = '{answer}' WHERE Id = {flashcard.Id}";
                SqlCommand command = new SqlCommand(query, myConnection);

                myConnection.Open();

                command.ExecuteNonQuery();
            }

            Console.Clear();
            Console.WriteLine("Flashcard updated successfully!");
        }
        /*
                private bool CheckIdExists(int id)
                {
                    bool exists;
                    using (SQLiteConnection myConnection = new SQLiteConnection(connectionString))
                    {
                        myConnection.Open();

                        var cmd = myConnection.CreateCommand();
                        cmd.CommandText = $"SELECT * FROM coding_tracker WHERE Id={id}";
                        SQLiteDataReader reader = cmd.ExecuteReader();

                        exists = reader.HasRows;

                        reader.Close();
                    }
                    return exists;
                }
         * */
    }
}
