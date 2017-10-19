namespace Domain.Models
{
    public class PriceRate
    {
        public DogSize DogSize { get; set; }
        public bool IsAggressive { get; set; }
        public decimal PricePerWalk { get; set; }
        public byte MaxPackSize { get; set; }
    }
}
