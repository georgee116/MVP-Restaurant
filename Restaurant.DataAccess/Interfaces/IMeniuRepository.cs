using Restaurant.Domain.Entities;

namespace Restaurant.DataAccess.Interfaces
{
    public interface IMeniuRepository : IRepository<Meniu>
    {
        // nu mai trebuie să adaugi nimic aici, moşteneşte toate metodele CRUD din IRepository<Meniu>
    }
}
