# Verbesserungsvorschläge – MyTribe Lösung

## 🔴 KRITISCH (sofort beheben)

### ✅ 1. RaffleController: Debug-Logging in Produktion
**Datei:** `Tribe/Tribe/Controller/ShopController/RaffleController.cs`
**ERLEDIGT:** Debug-Logging entfernt, `GetProfileId()` liest direkt aus JWT-Claim `profileId`.

### ✅ 2. RaffleController: GetCreatorProfileIdAsync() – Unnötige DB-Abfrage
**Datei:** `RaffleController.cs`
**ERLEDIGT:** DB-Abfrage entfernt, `ApplicationDbContext` nicht mehr injiziert.

### ✅ 3. Authentication: Doppelte AddAuthentication()-Aufrufe
**Datei:** `Tribe/Tribe/Program.cs`
**ERLEDIGT:** Zu einem einzigen `AddAuthentication()`-Aufruf zusammengeführt. JWT-Cookie-Fallback hinzugefügt.

### ✅ 4. Login.razor: Server ruft eigene API auf (Self-Call)
**Datei:** `Tribe/Tribe/Components/Account/Pages/Login.razor`
**ERLEDIGT:** HttpClient-Self-Call entfernt. Verwendet jetzt direkt `SignInManager`, `JwtTokenService` und `ApplicationDbContext`.

### ✅ 5. ClientApiService: Reflection für HttpClient-Zugriff
**Datei:** `Tribe.Services/GeneralServices/ClientApiService.cs`
**ERLEDIGT:** Reflection entfernt, `HttpClient` wird per DI injiziert. Auch in `UserApiService` behoben.

---

## 🟡 WICHTIG (zeitnah beheben)

### ✅ 6. ShopCreatorService: Raffle-Methoden verwenden `dynamic`
**Datei:** `Tribe.Services/ClientServices/ShopServices/ShopCreatorService.cs`
**ERLEDIGT:** `dynamic`/`object` durch typisierte `Raffle`-Klasse ersetzt.

### ✅ 7. Doppelte Raffle-Client-Services
**Dateien:** `IRaffleClientService` + `IShopCreatorService`
**ERLEDIGT:** `ShopCreatorService` delegiert Raffle-Methoden an `IRaffleClientService`.

### ✅ 8. Doppelte LoginResponse-Klasse
**Dateien:** `ComModels.cs` + `ClientModels.cs`
**ERLEDIGT:** Duplikate aus `ClientModels.cs` entfernt, referenziert jetzt `ComModels`.

### ⏭️ 9. Doppelte RaffleEditDialog.razor
**Dateien:**
- `Tribe.Ui/Pages/Shop/CreatorShop/RaffleEditDialog.razor`
- `Tribe.Ui/Pages/CreatorShop/RaffleEditDialog.razor`
- `Tribe.Ui/Pages/UserDashboard/CreatorRaffle/RaffleEditDialog.razor`
**ÜBERSPRUNGEN:** 3 duplizierte Ordner gefunden — erfordert Klärung welche Routen aktiv genutzt werden.

### ✅ 10. RaffleController: [Authorize] ohne Authentication-Scheme
**Datei:** `RaffleController.cs`
**ERLEDIGT:** `[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]` gesetzt.

---

## 🟢 EMPFEHLUNG (bei Gelegenheit)

### 11. ApplicationUser.cs: Alle Modelle in einer Datei
**Datei:** `Tribe.Bib/TribeRelated/ApplicationUser.cs`
**Problem:** 15+ Klassen in einer Datei (~400 Zeilen). Schwer zu navigieren und zu warten.

### ✅ 12. UserApiService: Falscher Logger-Typ
**Datei:** `Tribe.Services/GeneralServices/UserApiService.cs`
**ERLEDIGT:** `ILogger<ClientApiService>` durch `ILogger<UserApiService>` ersetzt.

### 13. API-Controller: Inkonsistente Endpunkt-Benennung
**Problem:** Mischung aus PascalCase (`api/Products`), lowercase (`api/shop/raffles`), generic (`api/genericapi/`).

### 14. ShopService als Singleton im WebAssembly-Client
**Datei:** `Tribe.Client/Program.cs` (Zeile 41)
**Problem:** `AddSingleton<ShopService>()` — bei SSR teilen sich alle Requests denselben State.

### 15. GenericApiController: Sicherheitsrisiko
**Problem:** Generischer CRUD-Controller erlaubt Zugriff auf beliebige Entity-Typen.

### ✅ 16. JWT Secret-Key als Fallback-String
**Datei:** `Program.cs`
**ERLEDIGT:** Fallback-String durch `throw new InvalidOperationException()` ersetzt.

---

## 📊 Ergebnis

| # | Status | Beschreibung |
|---|--------|-------------|
| 1 | ✅ | Debug-Logging entfernt |
| 2 | ✅ | DB-Abfrage durch Claim ersetzt |
| 3 | ✅ | Doppelte Authentication zusammengeführt |
| 4 | ✅ | Login.razor Self-Call entfernt |
| 5 | ✅ | Reflection entfernt |
| 6 | ✅ | dynamic → Raffle typisiert |
| 7 | ✅ | Doppelte Services konsolidiert |
| 8 | ✅ | Doppelte LoginResponse entfernt |
| 9 | ⏭️ | Doppelte Razor-Dateien (übersprungen) |
| 10 | ✅ | Auth-Scheme gesetzt |
| 11 | — | Modelle aufteilen (bei Gelegenheit) |
| 12 | ✅ | Logger-Typ korrigiert |
| 13 | — | Endpunkt-Benennung (bei Gelegenheit) |
| 14 | — | Singleton → Scoped (bei Gelegenheit) |
| 15 | — | GenericApiController (bei Gelegenheit) |
| 16 | ✅ | Secret-Key Fallback entfernt |
