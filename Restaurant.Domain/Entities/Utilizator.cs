using Restaurant.Domain.Enums;

namespace Restaurant.Domain.Entities
{
    public class Utilizator
    {
        public int Id { get; set; }
        public string Nume { get; set; } = "";
        public string Prenume { get; set; } = "";
        public string Email { get; set; } = "";
        public string Telefon { get; set; } = "";
        public string AdresaLivrare { get; set; } = "";
        public string Parola { get; set; } = ""; // store hashed in prod
        public UserRole Role { get; set; }
        public string NumeComplet => $"{Nume} {Prenume}";
    }
}
