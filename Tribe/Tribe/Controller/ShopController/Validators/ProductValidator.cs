using System.ComponentModel.DataAnnotations;
using static Tribe.Bib.ShopRelated.ShopStruckture;

namespace Tribe.Controller.ShopController.Validators
{
    public static class ProductValidator
    {
        public static List<ValidationResult> ValidateProduct(ShopProduct product)
        {
            var results = new List<ValidationResult>();

            // Basic validation
            if (string.IsNullOrWhiteSpace(product.Title))
                results.Add(new ValidationResult("Title ist erforderlich"));

            if (product.Title?.Length > 200)
                results.Add(new ValidationResult("Title darf max. 200 Zeichen lang sein"));

            if (product.Price < 0)
                results.Add(new ValidationResult("Preis kann nicht negativ sein"));

            if (product.Price > 999999)
                results.Add(new ValidationResult("Preis ist zu hoch"));

            if (product.OriginalPrice.HasValue && product.OriginalPrice < product.Price)
                results.Add(new ValidationResult("Originalpreis sollte größer als aktueller Preis sein"));

            // Type-specific validation
            if (product is PhysicalProduct phys)
            {
                if (phys.StockQuantity < 0)
                    results.Add(new ValidationResult("Lagermenge kann nicht negativ sein"));

                if (phys.ShippingCost < 0)
                    results.Add(new ValidationResult("Versandkosten können nicht negativ sein"));

                if (!string.IsNullOrEmpty(phys.SKU) && phys.SKU.Length > 50)
                    results.Add(new ValidationResult("SKU ist zu lang"));
            }
            else if (product is VideoProduct vid)
            {
                if (vid.DurationSeconds <= 0)
                    results.Add(new ValidationResult("Videodauer muss > 0 sein"));

                if (vid.FileSizeBytes < 0)
                    results.Add(new ValidationResult("Dateigröße kann nicht negativ sein"));
            }
            else if (product is ImageProduct img)
            {
                if (img.FileSizeBytes < 0)
                    results.Add(new ValidationResult("Dateigröße kann nicht negativ sein"));

                if (img.PhotoCount <= 0)
                    results.Add(new ValidationResult("Fotoanzahl muss > 0 sein"));
            }
            else if (product is ServiceProduct svc)
            {
                if (svc.DurationMinutes <= 0)
                    results.Add(new ValidationResult("Servicedauer muss > 0 sein"));

                if (svc.MaxBookingsPerDay <= 0)
                    results.Add(new ValidationResult("Max. Buchungen pro Tag muss > 0 sein"));
            }
            else if (product is EventTicketProduct evt)
            {
                if (evt.EventDate <= DateTime.UtcNow)
                    results.Add(new ValidationResult("Event-Datum muss in der Zukunft liegen"));

                if (evt.MaxTickets <= 0)
                    results.Add(new ValidationResult("Max. Tickets muss > 0 sein"));

                if (string.IsNullOrWhiteSpace(evt.EventLocation))
                    results.Add(new ValidationResult("Event-Ort ist erforderlich"));
            }

            return results;
        }

        public static List<ValidationResult> ValidateOrder(ShopOrder order, List<ShopOrderItem> items)
        {
            var results = new List<ValidationResult>();

            if (order == null)
                results.Add(new ValidationResult("Order ist erforderlich"));

            if (items == null || items.Count == 0)
                results.Add(new ValidationResult("Mindestens 1 Artikel erforderlich"));

            if (!string.IsNullOrWhiteSpace(order?.CustomerEmail))
            {
                var emailValidator = new EmailAddressAttribute();
                if (!emailValidator.IsValid(order.CustomerEmail))
                    results.Add(new ValidationResult("E-Mail-Format ungültig"));
            }

            if (order?.TotalAmount < 0)
                results.Add(new ValidationResult("Gesamtbetrag kann nicht negativ sein"));

            return results;
        }
    }
}
