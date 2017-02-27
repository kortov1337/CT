using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSQMConsole
{
    public class Book
    {
        public Book(string title, string author, string branchOfScience, int numberOfPage, string publishingHouse, string binding)
        {
            this.title = title;
            this.author = author;
            this.branchOfScience = branchOfScience;
            this.numberOfPage = numberOfPage;
            this.publishingHouse = publishingHouse;
            this.binding = binding;
        }
        public string title { get; set; }
        public string author { get; set; }
        public string branchOfScience { get; set; }
        public int numberOfPage { get; set; }
        public string publishingHouse { get; set; }
        public string binding { get; set; }

        public override string ToString()
        {
            return Environment.NewLine + "Title: " + title + Environment.NewLine + "Author: " + author + Environment.NewLine + "Branch of science: "
                + branchOfScience + Environment.NewLine + "Number of pages: " + numberOfPage + Environment.NewLine +
                "Publishing house: " + publishingHouse + Environment.NewLine + "Binding: " + binding + Environment.NewLine;

        }
    }
}
