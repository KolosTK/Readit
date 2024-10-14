using Readit.Models.Enums;
namespace Readit.Models;

public class UserBook
{
    public int BookId { get; set; }
    public Book Book { get; set; }
    public Status Status { get; set; }
    public int Rate { get; set; }
    public DateTime DateAdded { get; set; }
    public DateTime DateModified { get; set; }
}