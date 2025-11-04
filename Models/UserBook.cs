using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace Capstone_Project_v0._1.Models;

public enum Status { WishList = 0, Owned = 1, Reading = 2, Finished = 3 }

public class UserBook
{
    public int OwnedBookID { get; set; }
    public int UserID { get; set; } = 1; 
    public int BookID { get; set; }
    public Book? Book { get; set; }
    public Status Status { get; set; } = Status.Owned;
    public int CurrentPage { get; set; } = 0;
    public DateTime? StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; }
    public DateTime DateAdded { get; set; } = DateTime.UtcNow;
}