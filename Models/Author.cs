using System.ComponentModel.DataAnnotations.Schema;

namespace Scridon_Grigore_Lab2.Models
{
    public class Author
    {
        public int ID { get; set; }

       
        public string FirstName { get; set; }

        public string LastName { get; set; }

     
       
        public string FullName => $"{FirstName} {LastName}";

      
    }
}

