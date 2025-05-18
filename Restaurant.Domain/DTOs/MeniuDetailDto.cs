namespace Restaurant.Domain.DTOs
{
    public class MeniuDetailDto
    {
        public int MeniuId { get; set; }
        public string Denumire { get; set; }
        public string Categorie { get; set; }
        public int PreparatId { get; set; }
        public string Preparat { get; set; }
        public float GramajPortie { get; set; }
        public decimal PretStandard { get; set; }
        public decimal Subtotal { get; set; }
    }
}


