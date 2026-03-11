using System.Text;

namespace HydraWebHook.Models
{
    public class HydraClient
    {
        private readonly HttpClient _httpClient = new();

        public HydraClient(string uri, string username, string password)
        {
            var authString = $"{username}:{password}";
            var plainTextBytes = Encoding.UTF8.GetBytes(authString);
            var apiToken = Convert.ToBase64String(plainTextBytes);
            _httpClient.BaseAddress = new Uri(uri);
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {apiToken}");
        }
        private static async Task<string> ParseResponse(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return response.IsSuccessStatusCode ? content : throw new HttpRequestException($"Unexpected server response: {response.StatusCode}\n{content}");
        }
        public async Task<List<HydraRecord>> List(string? search)
        {
            var response = await _httpClient.GetAsync($"records?q={search}");
            var content = await ParseResponse(response);
            var records = RecordHelpers.JsonToRecordList(content)!;
            return records;
        }
        public async Task<HydraRecord> Add(HydraRecord theRecord)
        {
            var json = RecordHelpers.RecordToJson(theRecord);
            using StringContent jsonContent = new(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("records", jsonContent);
            var content = await ParseResponse(response);
            return RecordHelpers.JsonToRecord(content)!;
        }

        public async Task CreateRecord(ExternalDnsRecord record) 
        {
            var hydraRecord = new HydraRecord
            {
                Content = record.Targets.First(),
                Hostname = record.DnsName,
                Type = record.RecordType
            };
            var json = RecordHelpers.RecordToJson(hydraRecord);
            using StringContent jsonContent = new(json, Encoding.UTF8, "application/json");
            await _httpClient.PostAsync("records", jsonContent);
        }
        public async Task DeleteRecord(ExternalDnsRecord record)
        {
            var hydraRecords = await List(record.DnsName);
            string? uid = null;
            foreach (var hydraRecord in hydraRecords.Where(hydraRecord => hydraRecord.Type == record.RecordType))
            {
                uid = hydraRecord.Id;
            }
            if (uid != null) 
            {
                await _httpClient.DeleteAsync($"records/{uid}");
            }
        }
    }
}
