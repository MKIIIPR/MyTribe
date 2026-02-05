# Shop Dashboard Integration - Dokumentation

## ?? Übersicht

Das Shop-Management ist vollständig in das Creator Dashboard integriert worden. Creators haben nun mehrere Wege, auf ihre Shop-Verwaltung zuzugreifen.

---

## ??? Zugriffsoptionen

### Option 1: Über UserDashboard (Empfohlen)
- **URL**: `/UserDashboard`
- **Tab**: "Mein Shop" (letzter Tab)
- **Vorteile**: Zentrale Verwaltung mit Profil, Creator-Einstellungen
- **Inhalt**: Alle Shop-Funktionen in einem Dashboard mit Tabs

### Option 2: Über Navigation (Schnellzugriff)
Die Navigation (`Nav2.razor`) enthält eine neue "Shop" Gruppe mit:
- **Meine Produkte** ? `/creator/shop/products`
- **Kategorien** ? `/creator/shop/categories`
- **Bestellungen** ? `/creator/shop/orders`

### Option 3: Master Shop Management Seite
- **URL**: `/creator/shop/dashboard`
- **Beschreibung**: Standalone Shop Management mit allen Tabs
- **Features**: "Neues Produkt" Button im Header

---

## ?? Neue Komponenten

### ShopDashboard.razor
**Pfad**: `Tribe.Ui/Pages/CreatorShop/ShopDashboard.razor`
- **Funktion**: Hauptkomponente mit Tabs
- **Tabs**:
  1. **Produkte** - ProductsList Komponente
  2. **Kategorien** - Categories Komponente
  3. **Bestellungen** - Orders Komponente
  4. **Bewertungen** - Reviews (TODO: Implementation pending)
  5. **Shop Statistiken** - Statistics Dashboard (placeholder)

### ShopManagementPage.razor
**Pfad**: `Tribe.Ui/Pages/CreatorShop/ShopManagementPage.razor`
- **Route**: `/creator/shop/dashboard`
- **Wraps**: ShopDashboard mit Creator-Check
- **Features**: 
  - Authorization Check (nur Creators)
  - "Neues Produkt" Button
  - UserState Integration

### ProductsPage.razor / CategoriesPage.razor / OrdersPage.razor
**Pfad**: `Tribe.Ui/Pages/CreatorShop/[Feature]Page.razor`
- **Routes**: 
  - `/creator/shop/products`
  - `/creator/shop/categories`
  - `/creator/shop/orders`
- **Funktion**: Navigation-Links mit Authorization
- **Reuses**: Bestehende Komponenten (ProductsList, Categories, Orders)

---

## ?? Navigation Integration

### Nav2.razor Update
```razor
<MudNavGroup Title="Shop" Expanded="false" Icon="@Icons.Material.Filled.Store">
    <MudNavLink Href="creator/shop/products" Icon="@Icons.Material.Filled.ShoppingCart">Meine Produkte</MudNavLink>
    <MudNavLink Href="creator/shop/categories" Icon="@Icons.Material.Filled.Category">Kategorien</MudNavLink>
    <MudNavLink Href="creator/shop/orders" Icon="@Icons.Material.Filled.Receipt">Bestellungen</MudNavLink>
</MudNavGroup>
```

- **Gruppe**: "Shop" (klappbar)
- **Icon**: Store Icon
- **Links**: 3 schnelle Zugriffspunkte
- **Status**: Default collapsed

---

## ?? Authorization & Sicherheit

### Creator-Check auf allen Seiten
```csharp
@if (UserState?.TribeProfile?.IsCreator == true)
{
    // Show content
}
```

### Fallbacks:
- **Kein Creator**: Warning Alert mit Link zu Dashboard
- **Nicht angemeldet**: Error Alert

---

## ?? Shop Statistiken (Placeholder)

Die Statistik-Tab zeigt:
- **Gesamtumsatz**: €0,00 (TODO: Berechnung)
- **Bestellungen**: 0 (TODO: Count)
- **Produkte**: 0 (TODO: Count)
- **Ø Bewertung**: 0? (TODO: Calculation)

**Nächste Schritte**: Backend-Endpoints für Stats implementieren

---

## ?? Verwendung

### Für Creators
1. Anmelden
2. Zu `/UserDashboard` navigieren
3. Tab "Mein Shop" öffnen
4. Oder über Navigation ("Shop" Gruppe) direkten Link klicken

### Für Administratoren
- Alle Routen sind mit `@authorize` geschützt
- UserState + CreatorProfile Check mandatory
- Logging in allen Services

---

## ?? Dateien hinzugefügt

| Datei | Pfad | Beschreibung |
|-------|------|-------------|
| ShopDashboard.razor | Tribe.Ui/Pages/CreatorShop/ | Hauptkomponente mit Tabs |
| ShopManagementPage.razor | Tribe.Ui/Pages/CreatorShop/ | Standalone Shop Page |
| ProductsPage.razor | Tribe.Ui/Pages/CreatorShop/ | Products Navigation Wrapper |
| CategoriesPage.razor | Tribe.Ui/Pages/CreatorShop/ | Categories Navigation Wrapper |
| OrdersPage.razor | Tribe.Ui/Pages/CreatorShop/ | Orders Navigation Wrapper |

### Modifizierte Dateien
| Datei | Änderung |
|-------|----------|
| Nav2.razor | Shop NavGroup hinzugefügt |
| UserDashboardMain.razor | "Mein Shop" Tab hinzugefügt |

---

## ? Status: READY

- ? Navigation integriert
- ? Shop Dashboard erstellt
- ? Alle Routes funktional
- ? Authorization implementiert
- ? Build erfolgreich

---

## ?? UI/UX Highlights

- **Icons**: Material Design Icons für visuelle Klarheit
- **Tabs**: Intuitive Navigation innerhalb des Shops
- **NavGroup**: Klappbar für bessere Navigation Übersicht
- **Alerts**: Klare Fehlermeldungen bei fehlender Authorization
- **Button**: "Neues Produkt" schnell erreichbar

