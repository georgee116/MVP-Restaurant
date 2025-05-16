namespace Restaurant.Domain.Entities
{
    public class ImaginePreparat
    {
        public int Id { get; set; }
        public string PathImagine { get; set; }

        public int PreparatId { get; set; }
        public Preparat Preparat { get; set; }
    }
}
