using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace Capstone_Project_v0._1.Models;

//  Status enum representing the reading status of a book
public enum Status { WishList = 0, Owned = 1, Reading = 2, Finished = 3 }

//  UserBook model class representing the relationship between a user and a book
public class UserBook
{
    public int OwnedBookID { get; set; } // Primary Key
    public int UserID { get; set; }
    public int BookID { get; set; } // Foreign Key to Book
    public Book? Book { get; set; } // Navigation property to Book
    public Status Status { get; set; } = Status.Owned; // Reading status
    public int CurrentPage { get; set; } = 0; // Current page number
    public DateTime? StartDate { get; set; } = DateTime.UtcNow; // Start date of reading
    public DateTime? EndDate { get; set; } // End date of reading
    public DateTime DateAdded { get; set; } = DateTime.UtcNow; // Date added to the library
}