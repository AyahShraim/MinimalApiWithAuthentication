# Secure API with JWT Authentication

## Description

This project is a secure API with JWT authentication implemented using ASP.NET Minimal APIs and Jwt token with the use of Carter framework. It provides a minimalistic structure for building a secure and authenticated API.

## Features

- JWT token-based authentication
- Jwt Token validation
- Minimal API structure
- Endpoint grouping using Carter
- API versioning

### Usage
The API provides the following endpoints:

- /Login: User login endpoint to generate JWT tokens.
- /welcome: Get a welcome message for the authenticated user.
- /specialResource: Get a special resource for the first member in the system.
Ensure you have valid JWT tokens for authenticated endpoints.

### Documentation

**Postman Documentation**
### [Postman API Documentation](https://documenter.getpostman.com/view/26374109/2s9YyweKSV)

Visit the [Postman API Documentation](https://documenter.getpostman.com/view/26374109/2s9YyweKSV) for additional details and usage instructions.
Samle of documentation:
![image](https://github.com/AyahShraim/MinimalApiWithAuthentication/assets/73714621/6ae93e2a-bea6-4590-89fb-d8a4c3d538c4)
![image](https://github.com/AyahShraim/MinimalApiWithAuthentication/assets/73714621/859779e0-a527-4bc2-b114-47252edc073c)


**Postman Collection**

Explore and test the API using the Postman Collection.

- Open Postman.
- Click on "Import" in the top-left corner.
- Upload the JSON file: SecureApiWithJWTAuthentication/MinimalApiWithJWT.postman_collection.json from the repository

**API Documentation**
- The API is documented using XML comments. Detailed information about each endpoint, including parameters and expected responses, can be found in the XML comments in the source code files.

**Swagger**

- Swagger documentation is available for visualizing and interacting with the API. When running the API in development mode, access the Swagger UI at https://localhost:7210/swagger/index.html.

**Contributing**

- Contributions are welcome! 

