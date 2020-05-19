# Azure Functions for retriving data from SharePoint Online, Dynamics CRM, and Dynamics 365 Business Central using Azue AD App and Microsoft Graph API

This repository contains Azure Functions for retrieving data from a SharePoint list, Dynamics CRM, and Dynamics 365 Business Central. These functions can be used anonymously without additional authentication and requesting permissions from the end-user. For instance, they can be utilized in public web forms as data sources for choice fields, e.g. drop-downs, checkboxes, or radios. For more inforamtion on how to install and use them, read articles on retrieving data from [SharePoint Online](https://medium.com/plumsail/how-to-provide-public-access-to-sharepoint-online-list-data-via-azure-function-using-azure-ad-app-f25f881d7328) and [Dynamics 365 Business Central](https://medium.com/plumsail/how-to-retrieve-data-from-dynamics-365-business-central-via-azure-function-and-use-it-in-public-aa219b0443cc).

## Installation

### SharePoint

1. Sign in to your Office 365 tenant as tenant administator and navigate to Azure Active Directory admin center.
2. [Register an Azure AD app](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app) and add application permissions to read data from SharePoint sites via **Microsoft Graph API** (Sites.Read.All).
3. Grant admin consent for your tenant.
4. Sign in to Azure Portal, [create function app](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-function-app-portal) and deploy FunctionApp project.
5. Set up [application settings](https://docs.microsoft.com/en-us/azure/azure-functions/functions-how-to-use-azure-function-app-settings) for your function app:

Name | Description
--- | --- 
SharePoint:AzureApp:ClientId | Id of the Azure AD app
SharePoint:AzureApp:ClientSecret | Client Secret of the Azure AD app
SharePoint:AzureApp:Tenant | Office 365 tenant, ex.: contoso.onmicrosoft.com
SharePoint:ListData:SiteUrl | The absolute URL of the source site, ex: https://contoso.sharepoint.com/sites/mysite
SharePoint:ListData:ListName | The name of the source list

### Dynamics 365 CRM

1. Sign in to your Office 365 tenant as tenant administator and navigate to Azure Active Directory admin center.
2. [Register an Azure AD app](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app) and add delegated permissions to Dynamics CRM: **Dynamics CRM** → **user_impersanation**.
3. Sign in to Azure Portal, [create function app](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-function-app-portal) and deploy FunctionApp project
4. Copy URL of **D365-CRM-Authorize** function and add it to **Authentication** → **Redirect URIs** of the Azure AD app
5. Set up [application settings](https://docs.microsoft.com/en-us/azure/azure-functions/functions-how-to-use-azure-function-app-settings) for your function app:

Name | Description
--- | --- 
Dynamics365.CRM:AzureApp:ClientId | Id of the Azure AD app
Dynamics365.CRM:AzureApp:ClientSecret | Client Secret of the Azure AD app
Dynamics365.CRM:AzureApp:Tenant | Office 365 tenant, ex.: contoso.onmicrosoft.com
Dynamics365.CRM:AzureApp:DynamicsUrl | URL of the Dynamics CRM, ex.: https://contoso.crm.dynamics.com

6. Copy URL of **D365-CRM-Authorize**, open it in your browser, and provide the app with the requested permissions. After that, all functions for Dynamics 365 CRM will perform on behalf of your account. The end-user will not be asked for permissions.

### Dynamics 365 Business Central

1. Sign in to your Office 365 tenant as tenant administator and navigate to Azure Active Directory admin center.
2. [Register an Azure AD app](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app) and add delegated  permissions to read/write financials data via **Microsoft Graph API** (Financials.ReadWrite.All).
3. Sign in to Azure Portal, [create function app](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-function-app-portal) and deploy FunctionApp project
4. Copy URL of **D365-BC-Authorize** function and add it to **Authentication** → **Redirect URIs** of the Azure AD app
5. Set up [application settings](https://docs.microsoft.com/en-us/azure/azure-functions/functions-how-to-use-azure-function-app-settings) for your function app:

Name | Description
--- | --- 
Dynamics365.BusinessCentral:AzureApp:ClientId | Id of the Azure AD app
Dynamics365.BusinessCentral:AzureApp:ClientSecret | Client Secret of the Azure AD app
Dynamics365.BusinessCentral:AzureApp:Tenant | Office 365 tenant, ex.: contoso.onmicrosoft.com
Dynamics365.BusinessCentral:Customers:Company | The name of the source company of customers
Dynamics365.BusinessCentral:Vendors:Company | The name of the source company of vendors
Dynamics365.BusinessCentral:Items:Company | The name of the source company of items

6. Copy URL of **D365-BC-Authorize**, open it in your browser, and provide the app with the requested permissions. After that, all functions for Business Central will perform on behalf of your account. The end-user will not be asked for permissions.

## License ##

The MIT License (MIT)

Copyright (c) Plumsail
