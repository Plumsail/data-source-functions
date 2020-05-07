# Azure Functions for retriving data from SharePoint Online and Dynamics 365 using Azue AD App and Microsoft Graph API

This repository contains Azure Functions for retrieving data from a SharePoint list and Dynamics 365 (coming soon). These functions can be used anonymously without additional authentication or requesting permissions from the end-user. For instance, they can be utilized in public web forms as data sources of choice fields, e.g. drop-downs, checkboxes, or radios. [Read the article](https://medium.com/plumsail/how-to-provide-public-access-to-sharepoint-online-list-data-via-azure-function-using-azure-ad-app-f25f881d7328) for more details.

## Installation

### SharePoint

1. Sign into your Office 365 tenant as tenant administator and navigate to Azure Active Directory admin center.
2. [Register an Azure AD app](https://docs.microsoft.com/en-us/azure/active-directory/develop/quickstart-register-app) and add application permissions to read data from SharePoint sites via **Microsoft Graph API** (Sites.Read.All).
3. Grant admin consent for your tenant.
4. Sign into Azure Portal, [create function app](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-function-app-portal) and deploy **SharePoint-ListData** function
5. Set up [application settings](https://docs.microsoft.com/en-us/azure/azure-functions/functions-how-to-use-azure-function-app-settings) for your function app:

Name | Description
--- | --- 
SharePoint:AzureApp:ClientId | Id of the Azure AD app
SharePoint:AzureApp:ClientSecret | Client Secret of the Azure AD app
SharePoint:AzureApp:Tenant | Office 365 tenant, ex.: contoso.onmicrosoft.com
SharePoint:ListData:SiteUrl | The absolute URL of the source site, ex: https://contoso.sharepoint.com/sites/mysite
SharePoint:ListData:ListName | The name of the source list

### Dynamics 365 Business Central

## License ##

The MIT License (MIT)

Copyright (c) Plumsail
