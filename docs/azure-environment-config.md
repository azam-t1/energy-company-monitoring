# Azure Environment Configuration Guide

This document provides detailed instructions for configuring environment settings for your Energy Company Monitoring application in Azure.

## Required GitHub Secrets

Before deploying, add these secrets to your GitHub repository (Settings > Secrets and variables > Actions):

| Secret Name | Description | Example |
|-------------|-------------|---------|
| `AZUREAPPSERVICE_PUBLISHPROFILE` | Publish profile XML from Azure App Service | *[XML content from Azure Portal]* |
| `AZURE_STATIC_WEB_APPS_API_TOKEN` | Deployment token for Static Web Apps | *[Token from Azure Portal]* |
| `AZURE_SQL_CONNECTION_STRING` | Connection string to Azure SQL Database | `Server=tcp:energy-monitoring-sql.database.windows.net,1433;Database=EnergyMonitoringDB;User ID=dbadmin;Password=yourPassword;Encrypt=true;Connection Timeout=30;` |
| `API_URL` | URL of your deployed API | `https://energy-monitoring-api.azurewebsites.net` |
| `CLIENT_URL` | URL of your deployed client | `https://lively-sand-0123ab456cd.azurestaticapps.net` |

## Backend API Configuration

### 1. Database Connection String

1. In the Azure Portal, navigate to your SQL Database
2. Go to "Connection strings" in the left menu
3. Copy the ADO.NET connection string
4. Replace `{your_username}` and `{your_password}` with your actual credentials
5. Add this as the `AZURE_SQL_CONNECTION_STRING` GitHub secret

### 2. CORS Configuration

The workflow automatically creates an `appsettings.Production.json` with CORS settings based on your `CLIENT_URL` secret.

To manually configure CORS in the Azure Portal:
1. Navigate to your App Service
2. Go to "CORS" in the left menu
3. Add your Static Web App URL to the allowed origins
4. Save the changes

### 3. Application Settings in Azure App Service

In addition to the automatic configuration, you may want to set these in the Azure Portal:
1. Go to your App Service > Configuration > Application settings
2. Add the following settings:
   - `ASPNETCORE_ENVIRONMENT`: `Production`
   - `WEBSITE_TIME_ZONE`: Your preferred timezone (e.g., `Eastern Standard Time`)

## Frontend Client Configuration

### 1. API URL Configuration

The workflow automatically creates a `.env` file with your API URL from the `API_URL` secret.

To manually configure API URL after deployment:
1. Go to your Static Web App in Azure Portal
2. Navigate to "Configuration" > "Application settings"
3. Add a setting with key `REACT_APP_API_URL` and your API URL as the value

### 2. Environment Variables for Static Web Apps

In Azure Static Web Apps, environment variables are configured differently:

1. Go to your Static Web App in Azure Portal
2. Navigate to "Configuration" > "Application settings"
3. Add any required environment variables:
   - `REACT_APP_API_URL`: URL of your API
   - `REACT_APP_VERSION`: Optional version identifier

### 3. Static Web Apps Configuration File

For advanced configuration, create a `staticwebapp.config.json` file in your client's public folder:

```json
{
  "navigationFallback": {
    "rewrite": "/index.html",
    "exclude": ["*.{css,scss,js,png,gif,ico,jpg,svg}"]
  },
  "globalHeaders": {
    "content-security-policy": "default-src https: 'unsafe-eval' 'unsafe-inline'; object-src 'none'"
  },
  "mimeTypes": {
    ".json": "text/json"
  }
}
```

## Testing Your Configuration

After deployment, verify your environment configuration:

1. **API Connectivity Test**:
   - Open your client application
   - Attempt to upload a meter reading CSV
   - Check if communication with the API is successful
   
2. **Database Connectivity Test**:
   - Use a tool like Azure Data Studio to connect to your database
   - Verify that data is being stored correctly

3. **CORS Test**:
   - Open browser DevTools (F12)
   - Check for any CORS errors in the Console tab
   - If errors exist, verify your CORS configuration

## Troubleshooting

1. **API Connection Issues**:
   - Verify your `API_URL` is correct
   - Ensure the API is running (check Azure App Service logs)
   - Check for any CORS errors in browser console

2. **Database Connection Issues**:
   - Verify your connection string in GitHub secrets
   - Check if firewall rules allow Azure App Service access
   - Review App Service logs for connection errors

3. **Client Build Issues**:
   - Review GitHub Actions workflow logs
   - Check for any environment variable issues

## Best Practices

1. **Sensitive Data**:
   - Never store sensitive data directly in code or configuration files
   - Always use GitHub secrets for passwords and connection strings
   
2. **Monitoring**:
   - Set up Azure Application Insights for both API and client
   - Configure alerts for critical errors or performance issues

3. **Scaling**:
   - Start with a B1 App Service Plan for development
   - Scale up/out based on production needs
