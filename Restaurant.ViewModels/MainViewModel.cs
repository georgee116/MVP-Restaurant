using Restaurant.Domain.Entities;
using Restaurant.Domain.Enums;

namespace Restaurant.ViewModels
{
    public class MainViewModel
    {
        public AlergenViewModel AlergenVM { get; } = new();
        public MeniuViewModel MeniuVM { get; } = new();
        public ComandaViewModel ComandaVM { get; } = new();

        // setate de Login step
        public Utilizator? CurrentUser { get; set; }
        public UserRole CurrentRole { get; set; } = UserRole.Guest;
    }
}
