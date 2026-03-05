# Community Issue Reporting & Local Events System with Service Request Management

## 🏙️ Overview
This web application allows community members to **report local issues** (such as sanitation, road, and utility problems), **stay informed about local events and municipal announcements**, and manage **service requests** efficiently.  
It promotes transparency, participation, and communication between residents and their municipality.

The system includes three major modules:
1. **Report Issues** – submit, view, manage community issues.
2. **Local Events & Announcements** – browse, filter, and discover upcoming events and important notices.
3. **Service Requests** – organise, prioritise, and visualise service requests with advanced data structures.

---

## 🌟 Features

### 🧾 Community Issue Reporting
- Create new issue reports with fields: **Title**, **Category**, **Location**, **Description**.  
- Optionally attach media (images/files).  
- Edit or delete existing reports.  
- View detailed report information with media preview.  
- Success toasts for create/edit/delete actions.  
- Real-time form validation.  
- Modern UI with **dark theme** and **video background**.  
- Encouraging messages for user engagement.

### 📅 Local Events & Announcements
- Displays **upcoming events** and **important municipal announcements**.  
- Search and filter by:
  - **Title**
  - **Category**
  - **Date range**
- Supports multiple filters simultaneously.  
- Highlights **Top Priority** events using a priority queue system.  
- Suggests **Recommended Events** based on recent searches using a **Stack + Queue** data structure.  
- Tracks user **Recent Searches** visually.  
- Dynamic categorisation using a **HashSet** and sorted event listing.  
- Clean, responsive card layout consistent with the dark UI.

### 🛠️ Service Request Management (Part 3)
- **Add, Edit, Delete service requests** efficiently.  
- **Filter and sort** by ID, status, category, or priority.  
- **Reset Button** for clearing filters and returning to default view.  
- **Services class** abstraction for simplifying controller logic.  
- **Visualise View**:
  - Key stats: total requests, most frequent category, highest priority, last updated.
  - Pie charts for category, status, and priority.
  - Simplified network graph showing connections between service requests.
- Data is efficiently organised and retrieved using advanced **data structures**.

---

## 🧠 Data Structures & Logic

The project demonstrates applied programming concepts in a practical MVC context:

### Community Events & Issues
- **SortedDictionary** → Groups events by date.  
- **HashSet** → Maintains unique categories.  
- **PriorityQueue** → Handles top priority announcements/events.  
- **Queue** → Stores recommended events dynamically.  
- **Stack** → Tracks last three user searches.

### Service Requests
- **AVL Tree** → Efficiently store and retrieve service requests by key.  
- **Min Heap** → Manages request priorities (1 = High, 2 = Medium, 3 = Low).  
- **Graph & MST** → Represents relationships between service requests and optimises visualisation.  
- **BFS Traversal** → Enables exploration of service request relationships.

---

## ⚙️ Technologies Used
- **.NET 6+ / ASP.NET Core MVC**
- **Entity Framework Core** (database access)
- **Bootstrap 5** (responsive design and dark theme)
- **C# Data Structures** (Stack, Queue, PriorityQueue, HashSet, SortedDictionary, AVL Tree, Min Heap, Graph)
- **Chart.js** (visualisation of statistics and network graphs)
- **HTML5 & CSS3** (Video backgrounds, modern UI)
- **JavaScript / AJAX** (dynamic data loading for visualisations)

---

## 🧩 Setup & Running

### Prerequisites
- .NET 6+ SDK
- Visual Studio 2022 or VS Code with C# extension
- Modern browser (Chrome, Edge, Firefox)
- Internet connection (for Chart.js CDN)

---

## 💻 Usage Guide

### Report Issues
1. Click **Create New Issue**.
2. Fill required fields: **Title, Category, Location, Description**.
3. Attach optional media.
4. Edit or delete existing reports.
5. Success toasts appear after actions.

### Local Events & Announcements
1. Use the **search and filter form** to find events.
2. Apply filters by **Title, Category, Date range**.
3. Explore **Top Priority Events**, **All Upcoming Events**, **Recommended Events**.
4. Recent searches appear visually under recommendations.

### Service Requests
1. **Add/Edit/Delete Requests** in table view using the **ServiceRequestController**.
2. Filter and sort requests by **ID, category, status, or priority**.
3. Use **Reset Button** to clear filters and restore default view.
4. Navigate to **Visualise View** in **ServiceRequestManager** to see:
   - Pie charts for **category, status, priority**.
   - Simplified network graph of request relationships.
   - Key stats: **total requests, most frequent category, highest priority count, last updated time**.

---

## 📁 Project Structure

```
/Prog7312PoePart1/
│
├── Controllers/
│   ├── EventsController.cs
│   ├── ReportIssuesController.cs        # Create, Edit, Delete, Details, Index for issues
│   ├── ServiceRequestController.cs     # Create, Edit, Delete, Details, Index for service requests
│   └── ServiceRequestManagerController.cs # Visualise view & management logic
│
├── Models/
│   ├── Event.cs
│   ├── ReportIssue.cs
│   └── ServiceRequest.cs
│
├── Services/
│   └── ServiceRequestService.cs        # Business logic, simplifies controllers
│
├── DataStructures/
│   ├── AVLTree.cs
│   ├── MinHeap.cs
│   └── Graph.cs
│
├── Views/
│   ├── Events/
│   │   └── Index.cshtml
│   ├── ReportIssues/
│   │   └── Index.cshtml
│   ├── ServiceRequest/
│   │   ├── Create.cshtml
│   │   ├── Edit.cshtml
│   │   ├── Delete.cshtml
│   │   ├── Details.cshtml
│   │   └── Index.cshtml
│   ├── ServiceRequestManager/
│   │   └── Visualise.cshtml
│   └── Shared/
│       └── _Layout.cshtml
│
├── wwwroot/
│   ├── videos/
│   │   └── Service Requests.mp4
│   ├── uploads/
│   └── css/
│       └── site.css
│
├── Prog7312PoePart1.sln
└── README.md
```
---

## 🪶 Notes
- Media uploads are stored in `/wwwroot/uploads`.
- Consistent **dark theme** and **video background** across all views.
- `ServiceRequestService` encapsulates business logic, reducing controller complexity.
- **Reset button** allows clearing all filters in the Service Request table.
- Demo data seeded in controllers for testing.

---

## 🚀 Future Improvements
- Implement **User Authentication & Roles** (Admin/User distinction).
- Add **Calendar Integration** for events.
- Include **Email Notifications** for new announcements or service requests.
- Enhance **Recommendation Algorithm** for events based on similarity and search history.
- Improve **network graph visualisation** for Service Requests with interactive features.
- Expand **database persistence** for service requests, events, and issues.
