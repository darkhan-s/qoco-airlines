namespace Qoco_Airlines.Models
{
    public class Inconsistency
    {
        public Flight Flight { get; set; } = default!;
        public string ExpectedNextDeparture { get; set; } = string.Empty;
    }
}
