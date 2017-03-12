using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace Client1.Models
{
    public class BooksModels
    {
        public int Id { get; set; }
        [DisplayName("Название")]
        public string title { get; set; }
        [DisplayName("Автор")]
        public string author { get; set; }
        [DisplayName("Область науки")]
        public string branchOfScience { get; set; }
        [DisplayName("Количество страниц")]
        public int numberOfPage { get; set; }
        [DisplayName("Издательство")]
        public string publishingHouse { get; set; }
        [DisplayName("Переплет")]
        public string binding { get; set; }
    }
}