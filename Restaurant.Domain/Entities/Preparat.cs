using System.Collections.Generic;

namespace Restaurant.Domain.Entities
{
    public class Preparat
    {
        public int Id { get; set; }
        public string Denumire { get; set; }
        public decimal Pret { get; set; }
        public float CantitatePortie { get; set; }
        public float CantitateTotala { get; set; }

        public int CategorieId { get; set; }
        public Categorie Categorie { get; set; }

        public ICollection<ImaginePreparat> Imagini { get; set; }
        public ICollection<PreparatAlergen> Alergeni { get; set; }
        public ICollection<PreparatMeniu> Meniuri { get; set; }
    }
}
