using System.Collections.Generic;

namespace Domain.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public List<Dog> Dogs { get; set; }
    }
}
