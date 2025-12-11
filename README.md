# Stackbuld Inventory & Order Management System

A production-grade RESTful Web API built with .NET 8 for managing product catalog and order processing with robust concurrency control to prevent overselling.

## 🎯 Features

### Product Catalog Management
-  Complete CRUD operations for products
-  Product search and filtering
-  Pagination support
-  Stock quantity tracking
-  Soft delete functionality
-  SKU uniqueness validation

### Order Processing
-  Place orders with multiple products
-  Real-time stock validation
-  Transaction-based order creation (prevents overselling)
-  Order cancellation with stock restoration
-  Order history tracking
-  Role-based access control

### Security & Authentication
-  JWT-based authentication
-  Role-based authorization (Admin, Customer)
-  User registration and login
-  Password validation with ASP.NET Core Identity

### Data Integrity & Concurrency
-  **Database transactions** for atomic operations
-  **Pessimistic locking** to prevent race conditions
-  **Concurrency conflict handling** with proper error responses
-  Stock quantity updates within transactions

## 🏗 Architecture

This project follows **Clean Architecture** principles with clear separation of concerns:

```
StackbuldInventoryOrderManagement/
│
├── Api/                          # Presentation Layer
│   ├── Controllers/              # API endpoints
│   └── Extensions/               # Service registration
│
├── Application/                  # Application Layer
│   ├── Interfaces/               # Service & repository contracts
│   ├── Services/                 # Business logic implementation
│   ├── DTOs/                     # Data Transfer Objects
│   └── Validators/               # FluentValidation rules
│
├── Domain/                       # Domain Layer
│   ├── Users/                    # User entities
│   ├── Products/                 # Product entities
│   ├── Orders/                   # Order entities
│   ├── Common/                   # Base entities
│   └── Audit/                    # Audit trail
│
├── Persistence/                  # Infrastructure Layer
│   ├── Context/                  # DbContext
│   ├── Repositories/             # Data access implementation
│   └── Extensions/               # Database configuration
│
└── Common/                       # Shared Layer
    ├── Responses/                # Response wrappers
    └── Utilities/                # Helper classes
```

## 🛠 Technology Stack

- **Framework**: .NET 8
- **ORM**: Entity Framework Core 8
- **Database**: PostgreSQL
- **Authentication**: ASP.NET Core Identity + JWT
- **Validation**: FluentValidation
- **Logging**: Microsoft.Extensions.Logging
- **API Documentation**: Swagger/OpenAPI

##  Prerequisites

- .NET 8 SDK
- PostgreSQL 12 or higher
- Visual Studio 2022 / VS Code / JetBrains Rider

##  Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/stackbuld-inventory-management.git
cd stackbuld-inventory-management
```

### 2. Configure Database Connection

Set the PostgreSQL connection string as an environment variable:

**Windows (PowerShell):**
```powershell
$env:DATABASE_CONNECTION="Host=localhost;Database=StackbuldInventoryDb;Username=postgres;Password=yourpassword"
```

**Linux/Mac:**
```bash
export DATABASE_CONNECTION="Host=localhost;Database=StackbuldInventoryDb;Username=postgres;Password=yourpassword"
```

### 3. Apply Database Migrations

```bash
cd StackbuldInventoryOrderManagement.Persistence
dotnet ef migrations add InitialCreate --startup-project ../StackbuldInventoryOrderManagement.Api
dotnet ef database update --startup-project ../StackbuldInventoryOrderManagement.Api
```

### 4. Run the Application

```bash
cd ../StackbuldInventoryOrderManagement.Api
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7001`
- HTTP: `http://localhost:5001`
- Swagger UI: `https://localhost:7001/swagger`

##  API Endpoints

### Authentication
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/auth/onboard` | Customer registration | No |
| POST | `/api/auth/login` | User login | No |

### Products
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/products` | Create product | Admin |
| GET | `/api/products` | Get all products | No |
| GET | `/api/products/{id}` | Get product by ID | No |
| PUT | `/api/products/{id}` | Update product | Admin |
| DELETE | `/api/products/{id}` | Delete product | Admin |
| GET | `/api/products/availability` | Check stock availability | Yes |

### Orders
| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|---------------|
| POST | `/api/orders` | Place order | Yes |
| GET | `/api/orders/{id}` | Get order by ID | Yes |
| GET | `/api/orders` | Get all orders | Admin |
| GET | `/api/orders/my-orders` | Get user's orders | Yes |
| POST | `/api/orders/{id}/cancel` | Cancel order | Yes |

##  Authentication Flow

1. **Register as Customer**
```json
POST /api/auth/onboard
{
  "fullName": "Kingsley Okafor",
  "email": "kingsleychiboy22@gmail.com.com",
  "phoneNumber": "1234567890",
  "password": "Password123!",
  "userType": 1
}
```

2. **Login**
```json
POST /api/auth/login
{
  "email": "john@example.com",
  "password": "Password123!",
  "userType": 1
}
```

3. **Use the returned JWT token** in subsequent requests:
```
Authorization: Bearer {your-jwt-token}
```

## 🛡 Preventing Overselling

The system implements multiple layers of protection against overselling:

### 1. Database Transactions
All order creation operations are wrapped in database transactions to ensure atomicity.

### 2. Row-Level Locking
Products are locked during order processing using Entity Framework's change tracking, preventing concurrent modifications.

### 3. Stock Validation
- Pre-transaction validation checks
- In-transaction stock verification
- Atomic stock updates

### 4. Concurrency Conflict Handling
```csharp
try {
    // Order processing with transaction
    await transaction.CommitAsync();
}
catch (DbUpdateConcurrencyException ex) {
    await transaction.RollbackAsync();
    return "Unable to complete order due to concurrent updates. Please try again.";
}
```

##  Sample Request: Place Order

```json
POST /api/orders
Authorization: Bearer {your-jwt-token}

{
  "shippingAddress": "123 Main St, Lagos, Nigeria",
  "notes": "Please deliver after 2 PM",
  "orderItems": [
    {
      "productId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "quantity": 2
    },
    {
      "productId": "7bc42f3d-8932-4521-a4e9-9f8e7d6c5b4a",
      "quantity": 1
    }
  ]
}
```

##  Testing Concurrency

To test the system's ability to prevent overselling:

1. Create a product with limited stock (e.g., 5 units)
2. Simulate multiple concurrent orders that exceed total stock
3. Observe that only valid orders succeed while others fail gracefully

**Test Script Example:**
```bash
# Create 10 concurrent requests for 5 units each (50 total) when only 20 are available
for i in {1..10}; do
  curl -X POST https://localhost:7001/api/orders \
    -H "Authorization: Bearer {token}" \
    -H "Content-Type: application/json" \
    -d '{"orderItems":[{"productId":"...","quantity":5}]}' &
done
```

Expected Result: Only 4 orders succeed (20 units), 6 orders fail with insufficient stock error.

##  Design Patterns & Best Practices

### Implemented Patterns
-  **Repository Pattern**: Abstracts data access
-  **Unit of Work Pattern**: Transaction management via DbContext
-  **Dependency Injection**: Loose coupling throughout
-  **CQRS-lite**: Separation of read/write operations
-  **DTO Pattern**: Clear API contracts
-  **Response Wrapper Pattern**: Consistent API responses

### Best Practices
-  Clean Architecture with clear layer separation
-  SOLID principles
-  Comprehensive error handling
-  Structured logging
-  Input validation with FluentValidation
-  Async/await throughout
-  Proper use of HTTP status codes
-  API documentation with Swagger
-  Soft deletes for data retention
-  Audit trail support

##  Database Indexes

Optimized queries with strategic indexes:

```sql
-- Products
CREATE INDEX idx_product_name ON Products(Name);
CREATE UNIQUE INDEX idx_product_sku ON Products(Sku);

-- Orders
CREATE UNIQUE INDEX idx_order_number ON Orders(OrderNumber);
CREATE INDEX idx_order_customer ON Orders(CustomerId);
CREATE INDEX idx_order_status ON Orders(Status);
CREATE INDEX idx_order_date ON Orders(DateCreated);
```

##  Error Handling

The API returns consistent error responses:

```json
{
  "statusCode": 400,
  "message": "Insufficient stock: Laptop: Requested 10, Available 5",
  "data": null
}
```

Common status codes:
- `200` - Success
- `201` - Created
- `400` - Bad Request (validation errors, insufficient stock)
- `401` - Unauthorized
- `403` - Forbidden
- `404` - Not Found
- `409` - Conflict (duplicate SKU, concurrency)
- `500` - Internal Server Error

##  Pagination

All list endpoints support pagination:

```
GET /api/products?page=1&pageSize=10&searchTerm=laptop&minPrice=100&maxPrice=1000
```

Response:
```json
{
  "statusCode": 200,
  "message": "Successful",
  "data": {
    "currentPage": 1,
    "pageSize": 10,
    "totalCount": 45,
    "totalPages": 5,
    "items": [...]
  }
}
```

##  Configuration

### JWT Settings (appsettings.json)
```json
{
  "AuthSettings": {
    "SecretKey": "your-super-secret-key-min-32-characters",
    "Issuer": "StackbuldInventoryAPI",
    "Audience": "StackbuldInventoryClients",
    "ExpirationInMinutes": 60
  }
}
```

### Database Settings
Configure via environment variable for security:
```bash
DATABASE_CONNECTION="Host=localhost;Database=StackbuldInventoryDb;Username=postgres;Password=yourpassword"
```

##  Assumptions

1. **User Registration**: Only customers can self-register; admins are created manually
2. **Stock Management**: Negative stock is not allowed
3. **Order Cancellation**: Only pending/processing orders can be cancelled
4. **Product Deletion**: Soft delete to maintain referential integrity
5. **Currency**: All prices are in a single currency (configurable)
6. **Time Zones**: All timestamps stored in UTC
7. **Authentication**: JWT tokens expire after 60 minutes (configurable)
]
##  Deployment Considerations

### Production Readiness Checklist
-  Environment-based configuration
-  Connection string security (environment variables)
-  Structured logging with appropriate levels
-  Database retry policies for transient failures
-  Comprehensive error handling
-  API versioning ready
-  CORS configuration
-  Rate limiting ready
-  Health check endpoints ready

### Recommended Improvements for Scale
- Add Redis caching for product catalog
- Implement message queue for order processing
- Add distributed locking (Redis) for multi-instance deployments
- Implement API rate limiting
- Implement circuit breaker pattern
- Add health check endpoints
- Configure auto-scaling policies


##  Author

**Kingsley Okafor C**

## 🤝 Contributing

Contributions, issues, and feature requests are welcome!

## ⭐️ Show your support

Give a ⭐️ if this project helped you!
