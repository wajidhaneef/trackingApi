using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace trackingApi.Models
{
    public class News
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? NumOfArticles { get; set; }
        public string? Articles { get; set; }
        public string? Title { get; set; }
    }
}
