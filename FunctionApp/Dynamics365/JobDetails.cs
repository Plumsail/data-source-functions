using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Plumsail.DataSource.Dynamics365;

public class JobDetails
{
    private readonly HttpClientProvider _httpClientProvider;

    public JobDetails(HttpClientProvider httpClientProvider)
    {
        _httpClientProvider = httpClientProvider;
    }

    [FunctionName("JobDetails")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
        HttpRequest req,
        ILogger log)
    {
        try
        {
            var client = _httpClientProvider.Create();

            // load _ldd_projectid_value from ldd_jobs
            var jobNumber = req.Query["jobNumber"];

            log.LogInformation("JobDetails for " + jobNumber + " requested");
            var jobsJson = await client.GetStringAsync(
                $"ldd_jobs?$select=_ldd_projectid_value&$filter=ldd_jobnumber eq '{jobNumber}'&$top=1");
            var jobs = JObject.Parse(jobsJson)["value"];

            if (!jobs.Any()) return new NotFoundResult();

            //load project information from kms_projects
            var projectId = jobs[0]["_ldd_projectid_value"];
            if (projectId == null) return new OkObjectResult(new { });

            var projectJson = await client.GetStringAsync(
                $"kms_projects({projectId})?$select=_kms_role1contactid_value,_kms_role1accountid_value,kms_name,kms_address1_line1,kms_address1_line2,kms_address1_line3,kms_address1_city,kms_address1_postalcode,kms_name");
            var project = JObject.Parse(projectJson);

            //load contact and account information from contacts and accounts
            var contactId = project["_kms_role1contactid_value"];
            var accountId = project["_kms_role1accountid_value"];

            var tasks = new List<Task<string>>();
            if (accountId != null) tasks.Add(client.GetStringAsync($"accounts({accountId})?$select=name"));
            if (contactId != null)
                tasks.Add(client.GetStringAsync($"contacts({contactId})?$select=fullname,emailaddress1"));

            var results = await Task.WhenAll(tasks);
            var contact = results.Length > 0 ? JObject.Parse(results[0]) : new JObject();
            var account = results.Length > 1 ? JObject.Parse(results[1]) : new JObject();

            return new OkObjectResult(new
            {
                project,
                contact,
                account
            });
        }
        catch (Exception ex)
        {
            log.LogError(ex, ex.StackTrace);
            throw;
        }
    }
}