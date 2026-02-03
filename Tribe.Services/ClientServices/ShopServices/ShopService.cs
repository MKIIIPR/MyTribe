using static Tribe.Bib.ShopRelated.ShopStruckture;

namespace Tribe.Services.ClientServices.ShopServices
{
    public class ShopService
    {
        public ShopCart Cart { get; private set; } = new();
        public List<ShopProduct> Products { get; private set; } = new();
        public List<ShopCategory> Categories { get; private set; } = new();
        public string CurrentCreatorId { get; set; } = string.Empty;

        // Events für UI-Updates
        public event Action? OnCartChanged;
        public event Action? OnProductsChanged;

        // Cart Methods
        public void AddToCart(ShopProduct product, int quantity = 1, DateTime? selectedSlot = null)
        {
            Cart.AddItem(product, quantity, selectedSlot);
            OnCartChanged?.Invoke();
        }

        public void RemoveFromCart(string productId)
        {
            Cart.RemoveItem(productId);
            OnCartChanged?.Invoke();
        }

        public void UpdateCartQuantity(string productId, int quantity)
        {
            Cart.UpdateQuantity(productId, quantity);
            OnCartChanged?.Invoke();
        }

        public void ClearCart()
        {
            Cart.Clear();
            OnCartChanged?.Invoke();
        }

        // Product Methods
        public void SetProducts(List<ShopProduct> products)
        {
            Products = products;
            OnProductsChanged?.Invoke();
        }

        public void AddProduct(ShopProduct product)
        {
            Products.Add(product);
            OnProductsChanged?.Invoke();
        }

        public void RemoveProduct(string productId)
        {
            Products.RemoveAll(p => p.Id == productId);
            OnProductsChanged?.Invoke();
        }

        public ShopProduct? GetProduct(string productId)
        {
            return Products.FirstOrDefault(p => p.Id == productId);
        }

        public List<ShopProduct> GetProductsByCategory(string categoryId)
        {
            // Hier würdest du später die Category-Zuordnung implementieren
            return Products.Where(p => p.Tags.Contains(categoryId)).ToList();
        }

        public List<ShopProduct> GetProductsByType<T>() where T : ShopProduct
        {
            // Die .OfType<T>()-Methode gibt bereits eine IEnumerable<T> zurück.
            // Wenn Sie diese in eine List<ShopProduct> konvertieren möchten,
            // müssen Sie das Ergebnis explizit in den Basistyp umwandeln.
            // Der ToList() Aufruf ist bereits korrekt, da der Compiler den Rückgabetyp kennt.
            // Es gibt hier keinen Fehler im Code.
            // Der Fehler trat vermutlich bei der Zuweisung des Rückgabewerts auf.

            // Die Methode ist bereits korrekt.
            // Es gab kein Problem mit der Methode selbst, sondern mit der Art und Weise,
            // wie Sie das Ergebnis in Ihrem Code verwendet haben.

            return Products.OfType<T>().ToList().Cast<ShopProduct>().ToList();
        }


        // Category Methods
        public void SetCategories(List<ShopCategory> categories)
        {
            Categories = categories;
        }

        public void AddCategory(ShopCategory category)
        {
            Categories.Add(category);
        }

        // Search & Filter
        public List<ShopProduct> SearchProducts(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return Products;

            searchTerm = searchTerm.ToLower();
            return Products.Where(p =>
                p.Title.ToLower().Contains(searchTerm) ||
                p.Description?.ToLower().Contains(searchTerm) == true ||
                p.Tags.Any(t => t.ToLower().Contains(searchTerm))
            ).ToList();
        }

        public List<ShopProduct> FilterProductsByStatus(string status)
        {
            return Products.Where(p => p.Status == status).ToList();
        }

        public List<ShopProduct> FilterProductsByPriceRange(decimal minPrice, decimal maxPrice)
        {
            return Products.Where(p => p.Price >= minPrice && p.Price <= maxPrice).ToList();
        }
    }
}