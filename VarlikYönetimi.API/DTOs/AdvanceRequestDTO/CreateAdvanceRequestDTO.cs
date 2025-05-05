namespace VarlikYönetimi.API.DTOs.AdvanceRequestDTO
{
    public class CreateAdvanceRequestDTO
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public DateTime DesiredDate { get; set; }
    }
}
