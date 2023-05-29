using trackingApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.OpenApi;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace trackingApi.Models
{
    public class Currency
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? CurrencyCodeFrom { get; set; }
        public string? CurrencyCodeTo { get; set; }
        public decimal? ConversionRate { get; set;}
    }

}
