using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities
{
    public class ComandaItem
    {
        public int Id { get; set; }

        public int ComandaId { get; set; }
        public Comanda Comanda { get; set; }

        // doar unul dintre cele două FK‐uri va fi nenul
        public int? PreparatId { get; set; }
        public Preparat Preparat { get; set; }

        public int? MeniuId { get; set; }
        public Meniu Meniu { get; set; }

        public int Cantitate { get; set; }
        public int CantitatePortie { get; set; } // salvează gramajul folosit la comandă
        public decimal PretUnitate { get; set; }   // prețul la momentul efectuării comenzii
    }
}
