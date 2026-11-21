# UPB Erasmus+ Hub

A web application built with **Orchard Core CMS** to help students of the National University of Science and Technology POLITEHNICA Bucharest explore Erasmus+ mobility opportunities and access useful information about the application process.

## Overview

**UPB Erasmus+ Hub** is a university-focused information platform designed to bring important Erasmus+ mobility resources together in one place.

Students can browse partner universities, explore application steps, and read experiences shared by other exchange students. The platform also provides an administration interface through Orchard Core, allowing content such as university information, application steps, and student reviews to be managed through the CMS.

The project was developed using ASP.NET Core and Orchard Core, with a custom module for application functionality and a custom theme for the website's presentation.

## Features

### 🎓 Partner University Catalogue
- Browse Erasmus+ partner universities.
- Search universities by name or related information.
- Filter universities by country and required language level.
- Navigate through results using pagination.
- View detailed university information, including Erasmus code, location, language requirements, available places, and website.

### 📋 Erasmus+ Application Steps
- Display the different stages of the Erasmus+ application process.
- Present required documents and useful guidance for each stage.
- Manage application-step content through Orchard Core.

### 💬 Student Reviews
- Browse reviews submitted by exchange students.
- View information such as host university, mobility semester, overall rating, and estimated monthly living costs.
- Submit a review through a dedicated form.
- Store submitted reviews as drafts for administrator moderation before publication.

### 📢 Announcements and Information
- Homepage with quick access to the main sections.
- Dynamic announcement content managed through Orchard Core.
- Dedicated page with general Erasmus+ mobility information.

### ⚙️ Content Management
- Custom Orchard Core content types and content parts.
- Admin interface for managing partner university information.
- Content lifecycle handling and default values.
- Custom database indexing to support university catalogue queries.

## Technology Stack

| Technology | Purpose |
|---|---|
| C# | Application development |
| ASP.NET Core | Web application framework |
| Orchard Core CMS | Content management and extensibility |
| YesSQL | Document database and querying |
| SQLite | Database provider |
| Razor Views | Server-side page rendering |
| Liquid | Orchard Core layout templating |
| Bootstrap 5 | Responsive UI styling |
| HTML / CSS | Frontend structure and styling |
| Dependency Injection | Component registration and management |

## Project Architecture

The solution is organized into three main projects:

```text
UpbErasmusHub/
│
└── src/
    ├── Modules/
    │   └── UpbErasmusHub.Core/
    │       ├── Controllers/
    │       ├── Models/
    │       ├── ViewModels/
    │       ├── Drivers/
    │       ├── Handlers/
    │       ├── Indexes/
    │       ├── Migrations.cs
    │       ├── Startup.cs
    │       └── Manifest.cs
    │
    ├── Themes/
    │   └── UpbErasmusHub.Theme/
    │       ├── Views/
    │       └── Manifest.cs
    │
    └── Web/
        └── UpbErasmusHub.Web/
```

*The structure above illustrates the main project organization; individual files and folders may vary.*

### 1. UpbErasmusHub.Web
The application host. It starts the ASP.NET Core application and loads the Orchard Core environment, modules, and themes.

### 2. UpbErasmusHub.Core
The custom Orchard Core module containing the main application functionality.

It includes:
- Controllers responsible for handling requests.
- Content parts and models defining application data.
- ViewModels for organizing data passed to views and validating input.
- Display drivers for custom content-part editing interfaces.
- Content handlers for lifecycle operations and data normalization.
- Custom YesSQL indexes for more efficient university catalogue queries.
- Versioned migrations for defining and updating content types and database structures.
- Startup configuration for registering module components and routes.

### 3. UpbErasmusHub.Theme
The custom Orchard Core theme responsible for the website's appearance.

It includes:
- Custom page templates.
- Homepage presentation.
- Partner university detail presentation.
- Blog post and announcement presentation.
- Shared layout and frontend styling.

The theme uses **TheBlogTheme** as its base theme and extends it with project-specific templates and design.

## Database and Content Structure

The project uses Orchard Core's content management system to organize information into content types.

| Content type | Purpose |
|---|---|
| `PartnerUniversity` | Stores Erasmus+ partner university information. |
| `StudentReview` | Stores student experiences and ratings. |
| `ApplicationStep` | Stores information about application stages. |

The `PartnerUniversityPart` contains structured university information, such as the Erasmus code, country, city, language requirement, website, and available places.

A custom `PartnerUniversityIndex` maps selected university fields into a queryable index. This allows the application to filter, sort, count, and paginate university results without loading every complete content item first.

Database and content structure changes are managed through versioned Orchard Core migrations.

## Getting Started

### Prerequisites

Before running the project, make sure you have:

- .NET SDK compatible with the solution.
- A compatible development environment (Visual Studio, Visual Studio Code, or JetBrains Rider).
- Git.

### Installation

1. Clone the repository:

```bash
git clone https://github.com/Nicu-C/UpbErasmusHub.git
```

2. Navigate to the project directory:

```bash
cd UpbErasmusHub
```

3. Restore the NuGet dependencies:

```bash
dotnet restore
```

4. Build the solution:

```bash
dotnet build
```

5. Run the ASP.NET Core web host:

```bash
dotnet run --project <path-to-UpbErasmusHub.Web.csproj>
```

Replace the placeholder with the actual path to the Web project's `.csproj` file.

6. Open the local URL displayed in the terminal.

**Note:** On the first launch, complete any required Orchard Core setup and database configuration. Make sure the custom module and theme are available and that the custom theme is selected in the Orchard Core administration interface.

## Main Application Routes

| Route | Description |
|---|---|
| `/` | Homepage |
| `/universities` | Partner university catalogue |
| `/about` | Erasmus+ information |
| `/application-steps` | Application process and required documents |
| `/reviews` | Published student reviews |
| `/reviews/submit` | Student review submission form |

Individual university detail pages use the configured university autoroute pattern.

## Development Highlights

Some of the main technical aspects implemented in this project include:

- Extending Orchard Core through a custom module and theme.
- Creating custom content types and content parts.
- Implementing versioned data migrations.
- Using dependency injection to register custom services and components.
- Implementing content lifecycle handlers for default values and data normalization.
- Building a custom YesSQL index for university catalogue queries.
- Implementing server-side search, filtering, sorting, and pagination.
- Validating student review submissions and saving them as drafts for moderation.
- Separating application functionality from frontend presentation.
