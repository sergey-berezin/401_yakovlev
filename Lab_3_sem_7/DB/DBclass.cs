using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using System.Xml;
using Microsoft.EntityFrameworkCore;

namespace DB
{
    public class TextID
    {
        public int ID { get; set; }
        public string Text { get; set; }
    }
    public class TextIDQuestion
    {
        public int TextNum { get; set; }
        public int ID { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public int openCount { get; set; }
    }
    public class TextsAnswers : DbContext
    {
        public DbSet<TextID> IDtoText { get; set; }
        public DbSet<TextIDQuestion> textIDQuestions { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder o)
        {
            string dataPath = "C:\\Users\\Yakov\\source\\repos\\401_yakovlev\\Lab_3_sem_7\\DB\\library.db";
            o.UseLazyLoadingProxies().UseSqlite($"Data Source={dataPath}");
        }
    }
}