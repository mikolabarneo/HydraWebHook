using HydraWebHook.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HydraWebHook.Controllers
{
    /// <summary>
    /// Main controller for handling External DNS webhooks.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class HydraWebHook(ILogger<HydraWebHook> logger) : ControllerBase
    {
        private readonly string? _uri = Environment.GetEnvironmentVariable("HYDRA_URI");
        private readonly string? _tokenName = Environment.GetEnvironmentVariable("HYDRA_TOKEN_NAME");
        private readonly string? _tokenPass = Environment.GetEnvironmentVariable("HYDRA_TOKEN_PASS");
        private const string MediaTypeFormatAndVersion = "application/external.dns.webhook+json;version=1";

        /// <summary>
        /// Health check endpoint.
        /// </summary>
        /// <returns>An IResult indicating the health status.</returns>
        [HttpGet("/healthz", Name = "HealthCheck")]
        public IResult Check()
        {
            return Results.Ok();
        }

        /// <summary>
        /// Initialization endpoint that provides a domain filter.
        /// </summary>
        /// <returns>An IResult containing the domain filter.</returns>
        [HttpGet("/", Name = "Init")]
        public IResult Init()
        {
            var domainFilter = new DomainFilter 
            {
                Filter = ["shore.ox.ac.uk"]
            };
            return Results.Json(domainFilter, contentType: MediaTypeFormatAndVersion);
        } 
        
        /// <summary>
        /// Gets DNS records from the Hydra API.
        /// </summary>
        /// <param name="search">The search string to filter records.</param>
        /// <returns>An IResult containing the list of DNS records.</returns>
        [HttpGet("/records", Name = "GetRecords")]
        public async Task<IResult> Get(string search = "")
        {
            try
            {
                if (_uri == null || _tokenName == null || _tokenPass == null)
                {
                    logger.LogError(
                        "ENV variables missing: uri={uri}, tokenName={tokenName}, tokenPass={tokenPass}",
                        _uri, _tokenName, _tokenPass
                    );

                    return Results.Problem("Connection ENV variables not set");
                }

                var client = new HydraClient(_uri, _tokenName, _tokenPass);
                var records = await client.List(search);

                List<ExternalDnsRecord> externalDnsRecords = [];

                foreach (var record in records)
                {
                    if (record.Type != "TXT")
                    {
                        var hostname = record.Hostname.TrimEnd('.');
                        externalDnsRecords.Add(new ExternalDnsRecord
                        {
                            DnsName = hostname,
                            RecordType = record.Type,
                            Targets = [record.Content.TrimEnd('.')],
                            RecordTtl = record.Ttl != null ? (int)record.Ttl : 0
                        });
                    }
                    else 
                    {
                        var hostname = record.Hostname.TrimEnd('.');
                        externalDnsRecords.Add(new ExternalDnsRecord
                        {
                            DnsName = hostname,
                            RecordType = record.Type,
                            Targets = [record.Content.TrimEnd('.')],
                            RecordTtl = record.Ttl != null ? (int)record.Ttl : 0
                        });
                    }
                }

                return Results.Json(
                    externalDnsRecords,
                    contentType: MediaTypeFormatAndVersion
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get records");

                return Results.Problem(
                    title: ex.GetType().Name,
                    detail: ex.ToString()
                );
            }
        }
        
        /// <summary>
        /// Applies DNS changes (create, update, delete).
        /// </summary>
        /// <param name="changes">The package of DNS changes to apply.</param>
        /// <returns>An IResult indicating the outcome of the operation.</returns>
        [HttpPost("/records", Name = "ManipulateRecords")]
        public async Task<IResult> ApplyChanges([FromBody] ExternalDnsPackage changes)
        {
            logger.LogDebug(JsonConvert.SerializeObject(changes));
            try
            {
                if (_uri == null || _tokenName == null || _tokenPass == null)
                {
                    logger.LogError("ENV variables missing");
                    return Results.Problem("Connection ENV variables not set");
                }

                var client = new HydraClient(_uri, _tokenName, _tokenPass);

                // CREATE
                if (changes.Create != null)
                {
                    foreach (var record in changes.Create)
                    {
                        await client.CreateRecord(record);
                    }
                }

                // UPDATE to be implemented, have no Idea what is it for never seen UpdateOld or UpdateNew not null in ExternalDnsPackage
                
                // DELETE
                if (changes.Delete == null) return Results.NoContent();
                {
                    foreach (var record in changes.Delete)
                    {
                        await client.DeleteRecord(record);
                    }
                }

                return Results.NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to apply DNS changes");
                return Results.Problem(ex.ToString());
            }
        }

        /// <summary>
        /// Adjusts endpoints based on the provided records.
        /// </summary>
        /// <param name="recordsToAdjust">A list of records to adjust.</param>
        /// <returns>An IResult containing the adjusted records.</returns>
        [HttpPost("/adjustendpoints", Name = "UpdateEndpoints")]
        public async Task<IResult> AdjustEndpointsHandler([FromBody] List<ExternalDnsRecord> recordsToAdjust)
        {
            try
            {
                if (_uri == null || _tokenName == null || _tokenPass == null)
                {
                    logger.LogError("ENV variables missing: uri={uri}, tokenName={tokenName}, tokenPass={tokenPass}",
                        _uri, _tokenName, _tokenPass);

                    return Results.Problem("Connection ENV variables not set");
                }
                
                List<ExternalDnsRecord> externalDnsRecords = [];
                var client = new HydraClient(_uri, _tokenName, _tokenPass);
                foreach (var recordToAdjust in recordsToAdjust)
                {   
                    var records = await client.List(recordToAdjust.DnsName);
                    if (records.Count > 0)
                    {
                        recordToAdjust.Targets = [];
                        foreach (var record in records.Where(record => record.Type == recordToAdjust.RecordType))
                        {
                            recordToAdjust.Targets.Add(record.Content.TrimEnd('.'));
                            recordToAdjust.RecordTtl = record.Ttl != null ? (int)record.Ttl : 0;
                            recordToAdjust.RecordType = record.Type;
                            externalDnsRecords.Add(recordToAdjust);
                        }
                    }
                    else
                    {
                        externalDnsRecords.Add(recordToAdjust);
                    }

                }
                return Results.Json(externalDnsRecords, contentType: MediaTypeFormatAndVersion);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get records");

                return Results.Problem(
                    title: ex.GetType().Name,
                    detail: ex.ToString()
                );
            }
        }
    }
}
