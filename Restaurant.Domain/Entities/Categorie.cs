using System.Collections.Generic;

namespace Restaurant.Domain.Entities
{
    public class Categorie
    {
        public int Id { get; set; }
        public string Nume { get; set; }

        public ICollection<Preparat> Preparate { get; set; }
    }
}
