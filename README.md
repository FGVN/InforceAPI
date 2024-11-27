# README for InforceAPI

## Project Overview
InforceAPI is a .NET 8-based web API project that provides endpoints for managing products and related comments. The application uses SQLite as its database and is designed to offer robust functionalities such as CRUD operations for products and comment management. The API is structured to handle file uploads, data retrieval with pagination, and image handling.

## Prerequisites
To run this project locally, ensure the following prerequisites are installed:
- .NET 8

## Project Structure
### Controllers
1. **ProductsController**
   - Manages products and provides endpoints for CRUD operations.
   - Endpoints include:
     - `GET /api/products?pageNumber={pageNumber}&pageSize={pageSize}&sortBy={sortBy}`: Retrieves a paginated list of products with optional sorting.
     - `GET /api/products/{id}`: Retrieves a specific product by its ID.
     - `POST /api/products`: Creates a new product, accepting an image file and product details.
     - `PUT /api/products/{id}`: Updates an existing product, with optional new image upload.
     - `DELETE /api/products/{id}`: Deletes a product by its ID.

2. **CommentsController**
   - Manages comments related to products.
   - Endpoints include:
     - `POST /api/comments`: Adds a new comment to a product.
     - `DELETE /api/comments/{id}`: Deletes a comment by its ID.

## Database Configuration
The project uses SQLite as the database. Ensure that you have configured the `AppDbContext` to use SQLite. Sample configuration in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=inforceapi.db;"
  }
}
```

## Running the Project
1. Clone the repository:
   ```bash
   git clone https://github.com/FGVN/inforceapi.git
   cd inforceapi
   ```

2. Restore the NuGet packages:
   ```bash
   dotnet restore
   ```

3. Apply database migrations (if needed):
   ```bash
   dotnet ef database update
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

5. The API will be available at `https://localhost:7034` (or the configured port).

## Endpoints Overview

### Products Endpoints
- **GET /api/products**
  - **Description**: Retrieves a paginated list of products.
  - **Query Parameters**: `pageNumber`, `pageSize`, `sortBy`

- **GET /api/products/{id}**
  - **Description**: Retrieves a specific product by its ID.

- **POST /api/products**
  - **Description**: Creates a new product with an image and associated data.
  - **Request Body**: `CreateProductDto`

- **PUT /api/products/{id}**
  - **Description**: Updates an existing product, with the option to upload a new image.

- **DELETE /api/products/{id}**
  - **Description**: Deletes a product by its ID.

### Comments Endpoints
- **POST /api/comments**
  - **Description**: Adds a new comment to a product.
  - **Request Parameters**: `productId`, `description`

- **DELETE /api/comments/{id}**
  - **Description**: Deletes a comment by its ID.

## Project Features
- **Image Handling**: Products can have associated images stored in the `wwwroot/uploads` directory.
- **CRUD Operations**: Full support for Create, Read, Update, and Delete operations for products and comments.
- **Data Pagination**: The `GET /api/products` endpoint supports pagination for efficient data retrieval.
- **Error Handling**: The API provides clear error messages for common issues like missing data or non-existent records.

## Sample Requests
### Create a Product
**POST** `/api/products`

```http
POST /api/products
Content-Type: multipart/form-data
```

**Body** (Example in `CreateProductDto`):
```json
{
  "name": "Sample Product",
  "image": "[file]",
  "count": 100,
  "width": 10,
  "height": 20,
  "weight": 1.5
}
```

### Add a Comment
**POST** `/api/comments`

```http
POST /api/comments?productId=1&description=This is a comment.
```

