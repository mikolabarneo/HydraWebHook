using System.Text.Json;

namespace HydraWebHook.Models
{
    /// <summary>
    /// Provides helper methods for serializing and deserializing Hydra records.
    /// </summary>
    public static class RecordHelpers
    {

        public static string RecordToJson(HydraRecord r) =>
            JsonSerializer.Serialize(r);

        public static string RecordListToJson(List<HydraRecord> r) =>
            JsonSerializer.Serialize(r);

        public static HydraRecord? JsonToRecord(string j) =>
            JsonSerializer.Deserialize(j, typeof(HydraRecord))
                as HydraRecord;

        public static List<HydraRecord>? JsonToRecordList(string s) =>
            JsonSerializer.Deserialize(s, typeof(List<HydraRecord>))
                as List<HydraRecord>;

    }
}
