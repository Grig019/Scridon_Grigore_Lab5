using Scridon_Grigore_Lab2.Models;
using System.ComponentModel.DataAnnotations;


public class Publisher
{
    public int ID { get; set; }
    [Required]
    [Display(Name = "Publisher Name")]
    [StringLength(50)]
    public string PublisherName { get; set; }

    [StringLength(70)]
    public string Adress { get; set; }
    public ICollection<PublishedBook> PublishedBooks { get; set; }

}

public class PublishedBook
{
    public int PublisherID { get; set; }
    public int BookID { get; set; }
    public Publisher Publisher { get; set; }
    public Book Book { get; set; }
}


