# 💻 Phase 3: User Interface & Real-Time Client

## 🎯 Objective
Implement the MudBlazor UI components, pages, and the Client-side SignalR integration using the Cascading Parameter pattern.

## 📡 Step 1: SignalR Client Infrastructure
**Location:** `Tribe.Ui/Services` or `Tribe.Ui/SignalR`

1.  **CommunityConnectionProvider.razor:**
    *   Create a component that wraps the community content.
    *   **Logic:**
        *   Initialize `HubConnection`.
        *   Handle Reconnection.
        *   Implement `JoinCommunityChannel` logic.
        *   Expose C# `Events` (e.g., `OnMemberJoined`, `OnGroupCreated`) that child components can subscribe to.
    *   **Render:** `<CascadingValue Value="this">@ChildContent</CascadingValue>`

2.  **Registration:**
    *   In `Program.cs` (Client), ensure `MudServices` are added.
    *   Ensure `HttpClient` is configured to point to your API.

## 🧱 Step 2: Shared UI Components
**Location:** `Tribe.Ui/Components/Community`

Create reusable cards and lists to avoid code duplication:
*   `CommunityCard.razor` (Dashboard view)
*   `GroupCard.razor`
*   `MemberCard.razor`
*   `EventTimelineItem.razor` (for the Event timeline)
*   `CraftRequestCard.razor`

> **Note:** Use `MudCard`, `MudButton`, `MudChip` as per the styling conventions.

## 📄 Step 3: Main Pages
**Location:** `Tribe.Ui/Pages/Community`

Implement the routing and layout:

1.  **Index/Overview (`/communities`):**
    *   List user's communities.
    *   "Create Community" Modal.

2.  **Community Layout/Dashboard (`/community/{CommunityId}`):**
    *   Use `CommunityConnectionProvider` here to wrap the dashboard.
    *   Use `MudTabs` to switch between features:
        *   **Overview:** Stats, MotD.
        *   **Members:** List, Kick/Ban actions (Permission guarded).
        *   **Groups:** Filterable list of Groups.
        *   **Events:** Timeline view.
        *   **Crafts:** Craft browser and Request board.

3.  **Group Management:**
    *   Create/Edit Group Dialogs.
    *   Group Detail View (Members, Goals).

4.  **Craft Browser:**
    *   Search/Filter by `CraftCategory`.
    *   Dialog to show "Who can craft this?".

5.  **Event Calendar:**
    *   Create Event Dialog.
    *   "Sign Up" logic (Role selection).

## 🧩 Step 4: Component Integration
**Pattern:**
For every functional component (e.g., `CommunityGroups.razor`):
1.  Inject `[CascadingParameter] CommunityConnectionProvider Provider`.
2.  `OnInitialized`: Subscribe to relevant events (e.g., `Provider.OnGroupCreated += RefreshList;`).
3.  `Dispose`: Unsubscribe events to prevent memory leaks (`Provider.OnGroupCreated -= RefreshList;`).
4.  Use `InvokeAsync(StateHasChanged)` inside event handlers to update UI on SignalR messages.

## ✅ Checklist
- [ ] MudBlazor installed and configured?
- [ ] SignalR Client package (`Microsoft.AspNetCore.SignalR.Client`) installed?
- [ ] `CommunityConnectionProvider` manages the single Hub connection correctly?
- [ ] Pages update automatically when valid SignalR messages are received?
