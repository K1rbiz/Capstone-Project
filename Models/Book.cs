using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone_Project_v0._1.Models
{
    public class Book
    {
        public int BookID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int PageCount { get; set; }
        public int PublishYear { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    }
}
