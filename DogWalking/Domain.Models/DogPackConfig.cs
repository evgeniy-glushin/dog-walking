namespace Domain.Models
{
    public class DogPackConfig
    {
        public DogSize DogSize { get; set; }
        public bool IsAggressive { get; set; }
        public byte MaxPackSize { get; set; }
    }
}
