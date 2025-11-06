using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Project_v0._1.Models
{
    //  Book model class
    public class Book
    {
        public int BookID { get; set; } // Primary Key
        public string Title { get; set; } = string.Empty; // Title of the book
        public string AuthorName { get; set; } = string.Empty; // Author's name
        public string ISBN { get; set; } = string.Empty; // ISBN number
        public int PageCount { get; set; } // Number of pages
        public int PublishYear { get; set; } // Year of publication
        public DateTime DateAdded { get; set; } = DateTime.UtcNow; // Date added to the library
    }
}
