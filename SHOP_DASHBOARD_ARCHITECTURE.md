# Shop Dashboard Navigation - Architektur-Übersicht

## ??? Struktur

```
User Dashboard (/UserDashboard)
?
?? Tab 1: Profil
?   ?? ApplicationUserComponent
?
?? Tab 2: Creator werden
?   ?? ProfileEdit
?
?? Tab 3: Creator Bereich
?   ?? CreatorMainPage
?
?? Tab 4: Mein Shop ? [NEW]
    ?? ShopDashboard
        ?? Sub-Tab 1: Produkte
        ?   ?? ProductsList
        ?       ?? Produktliste (GET /api/products)
        ?       ?? Edit Button ? ProductEdit
        ?       ?? Delete Button
        ?
        ?? Sub-Tab 2: Kategorien
        ?   ?? Categories
        ?       ?? Kategorien-Grid (GET /api/shop/categories)
        ?       ?? Dialog: CategoryEditDialog
        ?       ?? CRUD Operations
        ?
        ?? Sub-Tab 3: Bestellungen
        ?   ?? Orders
        ?       ?? Orders Table (GET /api/shop/orders/creator)
        ?       ?? Status View
        ?       ?? Mark as Paid Button
        ?       ?? Details Link ? OrderDetails
        ?
        ?? Sub-Tab 4: Bewertungen
        ?   ?? Reviews Management (TODO)
        ?       ?? Reviews List
        ?       ?? Rating Stats
        ?
        ?? Sub-Tab 5: Shop Statistiken
            ?? Stats Dashboard (TODO)
                ?? Gesamtumsatz
                ?? Bestellungen Count
                ?? Produkte Count
                ?? Ø Bewertung
```

---

## ?? Navigation Flows

### Scenario 1: Direkter Shop-Zugriff via Navigation
```
User logged in + Creator = true
   ?
Nav2.razor: AuthorizeView ? MudNavGroup "Shop"
   ?
Links:
  - Meine Produkte ? /creator/shop/products ? ProductsPage ? ProductsList
  - Kategorien ? /creator/shop/categories ? CategoriesPage ? Categories
  - Bestellungen ? /creator/shop/orders ? OrdersPage ? Orders
  - Shop Dashboard ? /creator/shop/dashboard ? ShopManagementPage ? ShopDashboard
```

### Scenario 2: Dashboard-basierter Zugriff
```
User on /UserDashboard
   ?
Tab "Mein Shop" clicked
   ?
ShopDashboard Component rendered
   ?
All Tabs available:
  - Produkte
  - Kategorien
  - Bestellungen
  - Bewertungen
  - Statistiken
```

---

## ?? Datei-Übersicht

### Neue Komponenten
```
Tribe.Ui/Pages/CreatorShop/
?? ShopDashboard.razor ...................... Hub mit allen Shop-Tabs
?? ShopManagementPage.razor ................. Standalone /creator/shop/dashboard
?? ProductsPage.razor ...................... Wrapper für /creator/shop/products
?? CategoriesPage.razor .................... Wrapper für /creator/shop/categories
?? OrdersPage.razor ........................ Wrapper für /creator/shop/orders
```

### Genutzte Komponenten (bereits vorhanden)
```
Tribe.Ui/Pages/CreatorShop/
?? ProductsList.razor ...................... Produktliste + CRUD
?? ProductEdit.razor ....................... Produkt Editor (type-spezifisch)
?? Categories.razor ........................ Kategorien Management
?? CategoryEditDialog.razor ................ Dialog für Category Edit
?? Orders.razor ........................... Bestellungen Liste
?? OrderDetails.razor ..................... Order Detail View
?? ProductReviews.razor ................... Reviews Component
```

### Modifizierte Dateien
```
Tribe.Client/Layout/
?? Nav2.razor ............................. Shop NavGroup + Links hinzugefügt

Tribe.Ui/Pages/UserDashboard/
?? UserDashboardMain.razor ............... "Mein Shop" Tab hinzugefügt
```

---

## ?? Authorization

Alle Shop-Seiten haben **2-Level Authorization**:

### Level 1: AuthorizeView
```razor
<AuthorizeView>
    <Authorized>
        <!-- Content visible only if authenticated -->
    </Authorized>
    <NotAuthorized>
        <MudAlert>Please log in</MudAlert>
    </NotAuthorized>
</AuthorizeView>
```

### Level 2: Creator Check
```razor
@if (UserState?.TribeProfile?.IsCreator == true)
{
    <!-- Shop content visible only for creators -->
}
else
{
    <MudAlert>You must be a creator</MudAlert>
}
```

---

## ?? API Endpoints Verwendet

| Endpoint | Method | Route | Seite |
|----------|--------|-------|-------|
| Produkte | GET | `/api/products` | ProductsList |
| Produkt erstellen | POST | `/api/products` | ProductEdit |
| Produkt aktualisieren | PUT | `/api/products/{id}` | ProductEdit |
| Produkt löschen | DELETE | `/api/products/{id}` | ProductsList |
| Kategorien | GET | `/api/shop/categories` | Categories |
| Kategorie erstellen | POST | `/api/shop/categories` | CategoryEditDialog |
| Kategorie aktualisieren | PUT | `/api/shop/categories/{id}` | CategoryEditDialog |
| Kategorie löschen | DELETE | `/api/shop/categories/{id}` | Categories |
| Bestellungen (Creator) | GET | `/api/shop/orders/creator` | Orders |
| Bestellung Details | GET | `/api/shop/orders/{id}` | OrderDetails |
| Bestellung als bezahlt | POST | `/api/shop/orders/{id}/mark-paid` | Orders |
| Bestellung Status | PUT | `/api/shop/orders/{id}/status` | OrderDetails |
| Reviews (Produkt) | GET | `/api/shop/reviews/product/{id}` | ProductReviews |

---

## ?? Features pro Seite

### Produkte (ProductsList)
- ? Liste aller Creator-Produkte
- ? Edit Button pro Produkt
- ? Delete Button pro Produkt
- ? "Neues Produkt" Button
- ? Filter nach Status (Draft/Active/Inactive)
- ? Suche & Filter (TODO)

### Kategorien (Categories)
- ? Grid View mit Kategorien
- ? Farbliche Kennzeichnung
- ? Produkt-Count per Kategorie
- ? Edit/Delete Buttons
- ? "Neue Kategorie" Button
- ? Dialog-basierter Editor

### Bestellungen (Orders)
- ? Tabellen-View aller Bestellungen
- ? Status Farbkodierung
- ? Bestelldatum & Betrag
- ? "Mark as Paid" Button
- ? Details Link
- ? Status ändern (OrderDetails)

### Shop Statistiken
- ? Gesamtumsatz (TODO: Backend)
- ? Bestellungen Count (TODO: Backend)
- ? Produkte Count (TODO: Backend)
- ? Durchschnittliche Bewertung (TODO: Backend)

---

## ? Implementierungs-Checkliste

- ? Navigation Links in Nav2.razor
- ? Shop Tab in UserDashboardMain
- ? ShopDashboard Komponente
- ? ShopManagementPage Wrapper
- ? ProductsPage Wrapper
- ? CategoriesPage Wrapper
- ? OrdersPage Wrapper
- ? Authorization Checks
- ? Build erfolgreich
- ? Backend Stats Endpoints (TODO)
- ? Reviews Dashboard (TODO)

---

## ?? Verwendung für Entwickler

### Als Creator
1. Anmelden
2. Zu `/UserDashboard` navigieren
3. Tab "Mein Shop" öffnen
4. Oder über Navigation direkt zu `/creator/shop/products` gehen

### Als Developer
1. Neue Features in ShopDashboard.razor hinzufügen (neue Tabs)
2. Neue Pages als Wrapper erstellen (für direkte Navigation)
3. NavGroup in Nav2.razor aktualisieren (neue Links)

---

## ?? Notes

- **Creator Check**: Alle Seiten prüfen `UserState?.TribeProfile?.IsCreator`
- **Icons**: Alle Navigation Links nutzen Material Design Icons
- **Responsive**: MudGrid auf allen Seiten für Mobile/Desktop
- **Fehlerbehandlung**: Alerts für Authentication/Authorization Fehler

