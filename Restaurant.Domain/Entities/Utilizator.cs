using Restaurant.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities
{
    public class Utilizator
    {
        public int Id { get; set; }
        public string Nume { get; set; }
        public string Prenume { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string AdresaLivrare { get; set; }
        public string ParolaHash { get; set; }

        public UserRole Role { get; set; }

        public ICollection<Comanda> Comenzi { get; set; }
    }
}
