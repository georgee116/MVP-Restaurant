using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities
{
    public class Alergen
    {
        public int Id { get; set; }
        public string Nume { get; set; }

        public ICollection<PreparatAlergen> PreparatAlergeni { get; set; }
    }
}
