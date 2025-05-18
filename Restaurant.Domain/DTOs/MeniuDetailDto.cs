// Restaurant.Domain/DTOs/MeniuDetailDto.cs
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

// Restaurant.Domain/DTOs/MeniuTotalsDto.cs
public class MeniuTotalsDto
{
    public float TotalGramaj { get; set; }
    public decimal TotalPret { get; set; }
}

// Restaurant.Domain/DTOs/NewOrderResultDto.cs
public class NewOrderResultDto
{
    public int ComandaId { get; set; }
    public string CodUnic { get; set; }
}
