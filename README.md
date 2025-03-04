# Azure Functions for retriving data from SharePoint Online, Dynamics CRM, and Dynamics 365 Business Central using Entra ID app, Microsoft Graph API and Dynamics 365 Business Central API

This repository contains Azure Functions for retrieving data from a SharePoint list, Dynamics CRM, and Dynamics 365 Business Central. These functions can be used anonymously without additional authentication and requesting permissions from the end-user. For instance, they can be utilized in public web forms as data sources for choice fields—dropdowns, checkboxes, or radios. For more inforamtion on how to install and use them, read articles on retrieving data from [SharePoint Online](https://plumsail.com/blog/how-to-provide-public-access-to-sharepoint-online-list-data/), [Dynamics 365](https://plumsail.com/blog/how-to-retrieve-data-from-dynamics-365-crm/) and [Dynamics 365 Business Central](https://plumsail.com/blog/how-to-retrieve-data-from-dynamics-365-business-central/).

## Installation

### SharePoint

1. Open [Microsoft Entra admin center](https://entra.microsoft.com/)
2. [Register an app](https://learn.microsoft.com/en-us/entra/identity-platform/quickstart-register-app?tabs=client-secret#register-an-application) and add application permissions to read data from SharePoint sites via **Microsoft Graph API** (Sites.Read.All).
3. Grant admin consent for your tenant.
4. Fork this repository.
5. Sign in to the [Azure portal](https://portal.azure.com/), [create a function app](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-function-app-portal), and deploy your forked repository.
6. Set up [Environment variables](https://learn.microsoft.com/en-us/azure/azure-functions/functions-how-to-use-azure-function-app-settings) for your function app:

Name | Description
--- | --- 
SharePoint__AzureApp__ClientId | The Application (client) ID of the Entra ID app
SharePoint__AzureApp__ClientSecret | The Client secret of the Entra ID app
SharePoint__AzureApp__Tenant | Your Microsoft 365 tenant, ex.: contoso.onmicrosoft.com
SharePoint__ListData__SiteUrl | The absolute URL of the source site, ex: https://contoso.sharepoint.com/sites/mysite
SharePoint__ListData__ListName | The name of the source list

### Dynamics 365 CRM

1. Open [Microsoft Entra admin center](https://entra.microsoft.com/)
2. [Register an app](https://learn.microsoft.com/en-us/entra/identity-platform/quickstart-register-app?tabs=client-secret#register-an-application) and add delegated permissions to Dynamics CRM: **Dynamics CRM** → **user_impersanation**.
3. Fork this repository.
4. Sign in to the [Azure portal](https://portal.azure.com/), [create a function app](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-function-app-portal), and deploy your forked repository.
5. Copy the URL of the **D365-CRM-Authorize** function and paste it to **Authentication** → **Web** → **Redirect URIs** of the Entra ID app
6. Set up [Environment variables](https://learn.microsoft.com/en-us/azure/azure-functions/functions-how-to-use-azure-function-app-settings) for your function app:

Name | Description
--- | --- 
Dynamics365.CRM__AzureApp__ClientId | The Application (client) ID of the Entra ID app
Dynamics365.CRM__AzureApp__ClientSecret | The Client secret of the Entra ID app
Dynamics365.CRM__AzureApp__Tenant | Your Microsoft 365 tenant, ex.: contoso.onmicrosoft.com
Dynamics365.CRM__AzureApp__DynamicsUrl | The URL of your Dynamics CRM, ex.: https://mycompany.crm4.dynamics.com

7. Copy the URL of the **D365-CRM-Authorize**, open it in your browser, and approve the permission request. After that, all requests to Dynamics 365 CRM will perform on behalf of your account. The end-user will not be asked for permissions.

### Dynamics 365 Business Central

1. Open [Microsoft Entra admin center](https://entra.microsoft.com/)
2. [Register an app](https://learn.microsoft.com/en-us/entra/identity-platform/quickstart-register-app?tabs=client-secret#register-an-application) and add delegated permissions to Dynamics 365 Business Central: **Dynamics 365 Business Central** → **Financials.ReadWrite.All**.
3. Fork this repository.
4. Sign in to the [Azure portal](https://portal.azure.com/), [create a function app](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-function-app-portal), and deploy your forked repository.
5. Copy the URL of **D365-BC-Authorize** function and add it to **Authentication** → **Web** → **Redirect URIs** of the Entra ID app
6. Set up [Environment variables](https://learn.microsoft.com/en-us/azure/azure-functions/functions-how-to-use-azure-function-app-settings) for your function app:

Name | Description
--- | --- 
Dynamics365.BusinessCentral__AzureApp__ClientId | The Application (client) ID of the Entra ID app
Dynamics365.BusinessCentral__AzureApp__ClientSecret | The Client secret of the Entra ID app
Dynamics365.BusinessCentral__AzureApp__Tenant | Your Microsoft 365 tenant, ex.: contoso.onmicrosoft.com
Dynamics365.BusinessCentral__AzureApp__InstanceId | Your Business Central ID: https://businesscentral.dynamics.com/{instanceId}/?referrer=office
Dynamics365.BusinessCentral__Customers__Company | The name of the source company for D365-BC-Customers function
Dynamics365.BusinessCentral__Vendors__Company | The name of the source company for D365-BC-Vendors function
Dynamics365.BusinessCentral__Items__Company | The name of the source company for D365-BC-Items function

7. Copy the URL of the **D365-BC-Authorize**, open it in your browser, and approve the permission request. After that, all requests to Business Central will perform on behalf of your account. The end-user will not be asked for permissions.

## License ##

The MIT License (MIT)

Copyright (c) Plumsail
