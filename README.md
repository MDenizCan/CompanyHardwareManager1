# Company Hardware Manager

**My fourth and last project that i developed during my Internship**

## About the Project
Company Hardware Manager (CHM) is a comprehensive enterprise management system developed to manage hardware assets (computers, phones, monitors, etc.), their categories, departments, and personnel within an organization.

The system digitalizes critical business processes such as assigning hardware to personnel (Assignment), creating hardware requests by staff (Request), and detailed user/role management.

## Key Features
- **Asset Management:** Tracking, status management, and categorization of all items in the inventory.
- **Assignment Tracking:** A system to track which hardware is assigned to which user, when it was given, and its return status.
- **Request Management:** Allowing users to create requests for necessary hardware and enabling managers to approve or reject these requests.
- **User and Role Management:** JWT-based authentication, Role-based Authorization, and department management.
- **Soft Delete Mechanism:** Marking data as deleted (IsDeleted) instead of physical removal from the database.
- **Refresh Token:** Long-term session support for secure session management.

## Technologies Used

### Backend
- **.NET 8 Web API:** Modern and high-performance API infrastructure.
- **Entity Framework Core:** Database operations and ORM management.
- **N-Tier Architecture:** Layered architecture for sustainability (API, BLL, Entities, Infrastructure, Models).
- **JWT (JSON Web Token):** Secure authentication and authorization.
- **AutoMapper:** Simple and efficient mapping between objects (Entities to DTOs).
- **FluentValidation:** Strong and flexible validation of API input data.
- **Serilog:** Detailed logging and error tracking.
- **Swagger / Swashbuckle:** API documentation and testing interface.

### Database
- **SQL Server:** Relational database management.
- **Fluent API:** Programmatic configuration of database schema and relationships.

## Project Structure
- **CHM.API:** Entry point of the application, containing Controllers and Middlewares.
- **CHM.BLL (Business Logic Layer):** Layer where business logic and services reside.
- **CHM.INFRASTRUCTURE:** Database context (DbContext), Repositories, and Migrations.
- **CHM.ENTITIES:** Database models and common entities.
- **CHM.MODELS:** DTOs (Data Transfer Objects) and Request/Response models.

---
*This project is the final study reflecting my learning and technical growth during my internship.*

