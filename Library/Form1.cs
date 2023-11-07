﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Library
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private SQLiteConnection conn;        
        private DataTable dtAutors;
        private DataTable dtBooks;

        //private DataSet dtAutors;
        //private DataSet dtBooks;

        private SQLiteDataAdapter adAutors;
        private SQLiteDataAdapter adBooks;
        private void selectDatabaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filename = openFileDialog1.FileName;
                conn = new SQLiteConnection("DataSource=" + filename);
                conn.Open(); // создаст файл 
                string sqltext = "select name from sqlite_master where type='table';";

                using (SQLiteCommand cmd = new SQLiteCommand(sqltext, conn))
                {
                    SQLiteDataReader reader = cmd.ExecuteReader();
                    createTablesToolStripMenuItem.Enabled = !reader.HasRows; // отключаем кнопку создания таблиц
                    if (reader.HasRows)
                    {
                        dataGridViewFillAutors();
                        dataGridViewFillBooks();
                    }
                    reader.Close();
                    conn.Close();
                }                    
                //dataGridViewFillAutors();
                //dataGridViewFillBooks();               
            }
            else
            {
                //указать что будет если база не откроится Library.db
            }
        }

        private void createTablesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sqltext = "create table Autors(id int PRIMARY KEY, name varchar(20), description varchar(100));" +
                "create table Books(id int PRIMARY KEY, id_autors int, name varchar(20), description varchar(100));";
            using (SQLiteCommand cmd = new SQLiteCommand(sqltext, conn))
            {
                //cmd.Connection = conn; // эту строку можно убрать conn на строке выше
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            createTablesToolStripMenuItem.Enabled = false; // отключаем кнопку создания таблиц
        }

        //private void dataGridViewFill()
        //{
        //    string sqltext = "select * from Autors;";
        //    adAutors = new SQLiteDataAdapter(sqltext, conn);
        //    SQLiteCommandBuilder cbAutors = new SQLiteCommandBuilder(adAutors);
        //    dtAutors = new DataTable();
        //    //dtAutors = new DataSet();
        //    adAutors.Fill(dtAutors);
        //    dGVAutors.DataSource = dtAutors;

        //    sqltext = "select * from Books;";
        //    adBooks = new SQLiteDataAdapter(sqltext, conn);
        //    SQLiteCommandBuilder cbBooks = new SQLiteCommandBuilder(adBooks);
        //    dtBooks = new DataTable();
        //    //dtBooks = new DataSet();
        //    adBooks.Fill(dtBooks);
        //    dGVBooks.DataSource = dtBooks;
        //    // dGVBooks..DataSource = dtBooks.Tables[0];

        //}

        private void dataGridViewFillAutors()
        {
            string sqltext = "select * from Autors;";
            adAutors = new SQLiteDataAdapter(sqltext, conn);

            SQLiteCommandBuilder cbAutors = new SQLiteCommandBuilder(adAutors);
            dtAutors = new DataTable();            
            adAutors.Fill(dtAutors);
            dGVAutors.DataSource = dtAutors;            

        }
        private void dataGridViewFillBooks(string autor_id="")
        {
            //string sqltext = "select * from Books" +
            //    " join Autors on Autors.id = Books.id_autors";

            string sqltext = "select Books.id, Books.id_autors, Autors.name as autor, Books.name as book, Books.description as book_description from Books join Autors on Autors.id = Books.id_autors";
            if (!string.IsNullOrEmpty(autor_id))
            {
                sqltext = sqltext + " where id_autors=" + autor_id;
            }                
            dtBooks = new DataTable();
            adBooks = new SQLiteDataAdapter(sqltext, conn);

            //Вариант 1
            SQLiteCommandBuilder cbBooks = new SQLiteCommandBuilder(adBooks);
            adBooks.Fill(dtBooks);
            dGVBooks.DataSource = dtBooks;

            /* 
            Вариант 2
            adBooks.SelectCommand = new SQLiteCommand(sqltext, conn);
            sqltext = @"insert into Books values (@id, @id_autors, @name, @description);";
            adBooks.InsertCommand = new SQLiteCommand(sqltext, conn);
            adBooks.InsertCommand.Parameters.Add("id", DbType.Int32,5, "id");
            adBooks.InsertCommand.Parameters.Add("id_autors", DbType.Int32, 5, "id_autors");
            adBooks.InsertCommand.Parameters.Add("name", DbType.String, 20, "name");
            adBooks.InsertCommand.Parameters.Add("description", DbType.String, 100, "description");

            sqltext = @"update Books set id_autors=@id_autors, name=@name, description=@description;";
            adBooks.UpdateCommand = new SQLiteCommand(sqltext, conn);
            adBooks.UpdateCommand.Parameters.Add("id", DbType.Int32, 5, "id");
            adBooks.UpdateCommand.Parameters.Add("id_autors", DbType.Int32, 5, "id_autors");
            adBooks.UpdateCommand.Parameters.Add("name", DbType.String, 20, "name");
            adBooks.UpdateCommand.Parameters.Add("description", DbType.String, 100, "description");

            sqltext = @"delete from Books where id=@id;";
            adBooks.DeleteCommand = new SQLiteCommand(sqltext, conn);
            adBooks.DeleteCommand.Parameters.Add("id", DbType.Int32, 5, "id");  

            adBooks.Fill(dtBooks);
            dGVBooks.DataSource = dtBooks;
            */                   

        }
        private void saveChangesToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            adAutors.Update(dtAutors);            
            adBooks.Update(dtBooks);
        }

        private void dGVAutors_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //MessageBox.Show(dGVAutors[0, dGVAutors.CurrentRow.Index].Value.ToString());
            dataGridViewFillBooks(dGVAutors[0, dGVAutors.CurrentRow.Index].Value.ToString());


        }
    }
}
