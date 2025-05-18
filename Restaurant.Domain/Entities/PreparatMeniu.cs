using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities
{
    public class PreparatMeniu
    {
        public int MeniuId { get; set; }
        public Meniu Meniu { get; set; }

        public int PreparatId { get; set; }
        public Preparat Preparat { get; set; }

        /// <summary>
        /// Gramajul per porţie pentru preparatul din meniu (poate fi diferit de gramajul standard).
        /// </summary>
        public float CantitatePortieMeniu { get; set; }
    }
}
