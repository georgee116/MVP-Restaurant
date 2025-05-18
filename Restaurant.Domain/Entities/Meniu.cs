using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Domain.Entities
{
    public class Meniu
    {
        public int Id { get; set; }
        public string Denumire { get; set; }
        public int CategorieId { get; set; }
        public Categorie Categorie { get; set; }

        /// <summary>
        /// Discount-ul efectiv se va calcula la runtime după regula din config, 
        /// deci nu-l stocăm aici.
        /// </summary>
        public ICollection<PreparatMeniu> PreparatMeniuri { get; set; }
    }
}
