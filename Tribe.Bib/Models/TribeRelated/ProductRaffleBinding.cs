using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tribe.Bib.Models.TribeRelated
{
    /// <summary>
    /// Bindet Raffles an Shop-Produkte
    /// 1 Produkt kann maximal 1 Raffle haben
    /// 1 Raffle kann an mehrere Produkte gebunden sein
    /// </summary>
    public class ProductRaffleBinding
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(450)]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        [MaxLength(450)]
        public string RaffleId { get; set; } = string.Empty;

        /// <summary>
        /// Wie viele Los/Tokens pro Kauf erhält der Kunde
        /// Standard: 1 Los pro Produkt-Kauf
        /// </summary>
        public int TokensPerPurchase { get; set; } = 1;

        /// <summary>
        /// Ist die Raffle noch aktiv für dieses Produkt?
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Wann wurde die Bindung erstellt
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Wann wurde die Bindung zuletzt aktualisiert
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
