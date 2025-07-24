# ⚡️ Energy Company Monitoring

Welcome! 🚀 This project empowers energy companies to efficiently monitor customer meter readings and deliver accurate billing. Built with modern technologies and a focus on reliability, it's designed to make your workflow smoother and more enjoyable! 😊

## 📝 User Story
As an Energy Company Account Manager, I want to be able to load a CSV file of Customer Meter Readings so that we can monitor their energy consumption and charge them accordingly.

## 🎯 Task
Ensuring the Acceptance Criteria are met, build a C# Web API that connects to an instance of a database and persists the contents of the Meter Reading CSV file.

We have provided you with a list of test customers along with their respective Account IDs (please refer to `Test_Accounts.csv`). Please seed the `Test_Accounts.csv` data into your chosen data storage technology and validate the Meter Read data against the accounts.

## ✅ Acceptance Criteria

### 🚩 MUST HAVE

1) Create the following endpoint:
	
	POST => /meter-reading-uploads

2) The endpoint should be able to process a CSV of meter readings. An example CSV file has been provided (`Meter_reading.csv`).

3) Each entry in the CSV should be validated and if valid, stored in a DB.

4) After processing, the number of successful/failed readings should be returned.

**Validation:** 
- You should not be able to load the same entry twice
- A meter reading must be associated with an Account ID to be deemed valid
- Reading values should be in the format NNNNN

### 🌟 NICE TO HAVE
1) Create a client in the technology of your choosing to consume the API. You can use Angular/React/whatever you like.

2) When an account has an existing read, ensure the new read isn't older than the existing read.

3) Tests

4) CI/CD pipeline in GitHub

## 🗂️ Project Structure

### 🛠️ Backend (.NET Core Web API)
- **EnergyCompanyMonitoring**: Main Web API project
  - `Controllers`: Contains API endpoints including MeterReadingUploadsController
  - `Models`: Domain models (Account, MeterReading)
  - `Data`: Database context and configuration
  - `Services`: Business logic implementation
  - `DTOs`: Data Transfer Objects for API communication
  - `SeedData`: Sample data for testing and development

### 💻 Frontend (React)
- **client**: React application for consuming the API
  - Modern JavaScript UI for uploading meter readings
  - Displays validation results and error messages
  - Built with React hooks and custom components

### 🧪 Tests
- **EnergyCompanyMonitoring.Tests**: NUnit test project
  - Unit tests for services and business logic
  - Integration tests for API endpoints

## 🚀 Implementation Details

### Backend Features
- 🗄️ Entity Framework Core for ORM
- 🛢️ SQL Server database for data persistence
- 🛡️ Validation rules for meter readings
- 📄 CSV parsing and processing
- 🌐 RESTful API design

### Frontend Features
- 📤 CSV file upload functionality
- ✅ Display of successful/failed readings
- ⚠️ Error message visualization
- 📱 Responsive design

### 📅 Date Formats Supported
The system supports multiple date formats for meter readings:
- Standard format (dd/MM/yyyy HH:mm)
- Alternative format (M/d/yyyy H:mm)

### 🔄 CI/CD Pipeline

A GitHub Actions workflow has been set up for continuous integration and deployment of the application. The pipeline:

- 🏗️ Builds and tests both the .NET backend and React frontend
- 🤖 Runs automatically on pushes to main/master branches and pull requests
- 🕵️ Validates code quality before deployment

The workflow file is located at `.github/workflows/dotnet.yml`.

To view pipeline runs:
1. Go to the repository on GitHub
2. Click on the "Actions" tab
3. View the status of current and past workflow runs

## 🚦 Getting Started

### Prerequisites
- 🟣 .NET 6.0+ SDK
- 🟢 Node.js 16+ and npm
- 🟠 SQL Server (or Docker for containerized database)

### Setup and Installation

1. **Clone the repository** 📥
   ```
   git clone <repository-url>
   cd EnergyCompanyMonitoring
   ```

2. **Set up the backend** ⚙️
   ```
   cd EnergyCompanyMonitoring
   dotnet restore
   dotnet ef database update
   dotnet run
   ```
   The API will be available at http://localhost:5233

3. **Set up the frontend** 🖥️
   ```
   cd client
   npm install
   npm start
   ```
   The client application will be available at http://localhost:3000

### 🐳 Using Docker
You can also run the application using Docker:
```
docker-compose up -d
```

## 📡 API Usage

### Meter Reading Upload Endpoint

**POST /meter-reading-uploads**

Uploads meter readings from a CSV file.

**Request:**
- Form data with a CSV file

**Response:**
```json
{
  "successfulReadings": 25,
  "failedReadings": 10,
  "errors": [
    "Invalid reading for account 2346: Meter reading value must be in NNNNN format (up to 5 digits)",
    "Account 2354 does not exist",
    "Invalid reading for account 4534: Meter reading value is required"
  ]
}
```

## 🧪 Testing

Run the tests using:
```
dotnet test
```

## 🚀 Azure Deployment

This project is configured for deployment to Microsoft Azure. Follow these steps to deploy your application:

### Prerequisites
- An active Azure subscription
- Azure CLI installed (optional, for manual deployments)
- GitHub repository with the code

### Backend API Deployment (Azure App Service)

1. **Create an Azure App Service:**
   ```
   az group create --name energy-monitoring-rg --location eastus
   az appservice plan create --name energy-monitoring-plan --resource-group energy-monitoring-rg --sku B1
   az webapp create --name energy-monitoring-api --resource-group energy-monitoring-rg --plan energy-monitoring-plan --runtime "DOTNET|9.0"
   ```

2. **Configure Connection String:**
   - In the Azure portal, go to your App Service > Settings > Configuration
   - Add a new connection string named `DefaultConnection` with your Azure SQL Database connection string
   - Set the type to `SQLAzure`

3. **Set up GitHub Actions Deployment:**
   - In the Azure portal, go to your App Service > Deployment Center
   - Select GitHub as the source
   - Follow the prompts to connect to your GitHub account and select your repository
   - Download the publish profile and add it as a GitHub secret named `AZUREAPPSERVICE_PUBLISHPROFILE`

### Frontend Deployment (Azure Static Web Apps)

1. **Create an Azure Static Web App:**
   ```
   az staticwebapp create --name energy-monitoring-client --resource-group energy-monitoring-rg --location eastus2 --source https://github.com/YOUR_USERNAME/YOUR_REPO --branch main --app-location "/client" --output-location "build"
   ```

2. **Configure API URL:**
   - After deployment, get your API URL from the App Service overview
   - Update the client's environment configuration to point to this URL

3. **Set up GitHub Actions Deployment:**
   - During creation, Static Web Apps automatically sets up GitHub Actions
   - Copy the deployment token and add it as a GitHub secret named `AZURE_STATIC_WEB_APPS_API_TOKEN`

### Database Deployment (Azure SQL Database)

1. **Create an Azure SQL Database:**
   ```
   az sql server create --name energy-monitoring-sql --resource-group energy-monitoring-rg --location eastus --admin-user dbadmin --admin-password <password>
   az sql db create --resource-group energy-monitoring-rg --server energy-monitoring-sql --name EnergyMonitoringDB --service-objective S0
   ```

2. **Configure Firewall Rules:**
   ```
   az sql server firewall-rule create --resource-group energy-monitoring-rg --server energy-monitoring-sql --name AllowAzureServices --start-ip-address 0.0.0.0 --end-ip-address 0.0.0.0
   ```

3. **Run Migrations:**
   - Use Entity Framework Core tools to apply migrations to your Azure SQL database:
   ```
   dotnet ef database update --connection "Your_Azure_Connection_String" --project EnergyCompanyMonitoring
   ```

### Automated Deployment with GitHub Actions

This project includes a GitHub Actions workflow file (`.github/workflows/azure-deploy.yml`) that automates the deployment process:

1. **Set up required secrets in your GitHub repository:**
   - `AZUREAPPSERVICE_PUBLISHPROFILE`: The publish profile from your Azure App Service
   - `AZURE_STATIC_WEB_APPS_API_TOKEN`: The deployment token from your Static Web App

2. **Push to the main branch:**
   - Any push to the main branch will trigger the workflow
   - The workflow builds and deploys both the API and client application

3. **Monitor deployments:**
   - Go to the "Actions" tab in your GitHub repository
   - Check the logs for any deployment issues

### Post-Deployment Configuration

1. **CORS Configuration:**
   - In your Azure App Service, ensure CORS is properly configured to allow requests from your Static Web App

2. **Environment Variables:**
   - Set any required environment variables in the Azure App Service Configuration section

3. **Database Migration:**
   - Ensure your database is properly seeded with initial test accounts

## 📄 License

MIT

---

Made with ❤️ by the Energy Company Monitoring team. Happy coding!
