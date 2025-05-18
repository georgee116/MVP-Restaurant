using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Restaurant.Domain.Enums;

namespace Restaurant.Domain.Entities
{
    public class Comanda
    {
        public int Id { get; set; }
        public string CodUnic { get; set; }
        public DateTime DataComenzii { get; set; }
        public OrderStatus Status { get; set; }

        public decimal DiscountAplicat { get; set; }
        public decimal CostTransport { get; set; }
        public DateTime? OraEstimativaLivrare { get; set; }

        public int UtilizatorId { get; set; }
        public Utilizator Utilizator { get; set; }

        public ICollection<ComandaItem> ComandaItems { get; set; }
    }
}
