namespace VarlikYönetimi.API.DTOs.AdvanceRequestDTO
{
    public class UpdateAdvanceRequestDTO
    {
        public int Id { get; set; } 
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime DesiredDate { get; set; }
    }
}
