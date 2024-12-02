## GLOBANT DATA ENGINEER CODING CHALLENGE: CSV Loader API  


**Description**  
This API, built with **ASP.NET**, processes three CSV files: `Jobs.csv`, `Employees.csv`, and `Departments.csv`. The data from these files is read, validated, and inserted into a **SQL Server** database via connection string.  

## Quick start
- "Open terminal"
- "Excecute" git clone https://github.com/Pliperkiller/GDECC.git
- "Excecute" cd GDECC
- "Insert your SQLserver connection string inside appsettings.json or request acces to the default configured cloud server."
- "Excecute" dotnet run

---
## Features  

1. **Data Processing**:  
   - Reads and validates three CSV files: `Jobs.csv`, `Employees.csv`, and `Departments.csv`.  
   - Loads each file into a corresponding table in the database.  

2. **Handling Special Cases**:  
   Includes mechanisms to manage records with empty values or invalid data fields see Mappings folder.  
   - Empty names: Set names into "NULL".
   - Empty date: Set date to 1/1/0001 00:00:00.
   - Empty id: Set to 0.
  When reading data, all data is set to string, and latter formatted into their respective format by mapping the columns into their respective variable
    
3. **Endpoints for Querying Data**:
  Two API endpoints were created to query and extract specific information from the loaded datasets (See Controllers/QueriesController.cs).  
   - Queries are stores inside Queries folder.
   - QueriesController.cs has the endpoint controllers and also tool for reading the data and excecute the SQL queries.
  Another endpoint is added to remove all the data inside the database by excecuting SQL_ResetDB.sql query (for testing/debugging purposes).

5. **Containerization**:  
   - A Docker image has been created to streamline the deployment of this API.  
   - Access the Docker image in the link https://hub.docker.com/repository/docker/gruposl1e/gdecc/general
  
4. **ONLINE Server**:  
An Azure SQL server was implemented with limited access by IP address. Since it is not a public server, the IP address access must be requested manually. This can be managed by asking me personally, and in this way, I will assign access to the server so it can be used by the API (see connection string in appsettings.json). 

5. **MigrationAPI interface via Swagger UI**:  
The project was integrated with a user interface designed for executing the configured endpoints. It has the option to upload files for requests that have this feature enabled. To access it, go to the URL http://localhost:5000/swagger/index.html after running the application.

---

## API Endpoints  

### 1. **Get API status**  
   **Endpoint**: `/api/status`  
   **Method**: `GET`  
   **Description**: Returns a message if the API is running.  

### 2. **Test server connection**  
   **Endpoint**: `/api/database/test-connection`  
   **Method**: `GET`  
   **Description**: Returns the status connection between the api and the server (Testing and debugging purposes).

### 3. **Migration**  
   **Endpoints**: `/api/upload/employees , /api/upload/departments , /api/upload/jobs`  
   **Method**: `GET`  
   **Description**: Try to load the information from the CSV files. If nothing is loaded or the file is empty, return error 400. If the files are incompatible with the established business rules, return error 500. Additionally, return the number of batches loaded (with 1000 rows per batch). The batch loading is implemented such that for every 1000 rows, an insert is executed to the server. If more than 2001 rows are inserted, three data inserts will be performed for the respective file into the server, meaning 3 batches will be inserted. 

### 4. **Queries**  
   **Endpoints**: `/api/extract/hired-by-department , /api/extract/hired-by-quarters`  
   **Method**: `GET`  
   **Description**: Returns the tables grouped with the business information requested for the company's decision-making.

### 5. **Queries Reset DB**  
   **Endpoints**: `/api/extract/delete/reset-db`  
   **Method**: `DELETE`  
   **Description**: DELETE all data inside database (Testing and debugging purposes) .

---

## Handling Missing Data  

In the work scenario presented for this project, it was assumed that the FACTS table may have empty foreign keys. In this case, these empty values were mapped to an ID with a value of 0 and named 'Unknown' in the DIMENSIONS tables. When the application is initialized for the first time, these records are created in the tables within the server (see Data/MigrationDbContext.cs). In this way, the empty values can be mapped to add value through: analysis of the state of the software tools used, applications in data science, data analysis and administrative decision-making.
