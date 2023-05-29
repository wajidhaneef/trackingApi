namespace trackingApi.Dtos
{
    public class CurrencyDto
    {
        public string? CurrencyCodeTo { get; set; }
        public string? CurrencyCodeFrom { get; set; }
        public decimal? ConversionRate { get; set; }
        
    }
}