namespace ServerApplication.Models
{
    public class PresetCommentsData
    {
        public List<PresetComment>? PresetComments { get; set; }
    }

    public class PresetComment
    {
        public string? Comment { get; set; }
        public int Score { get; set; }
    }
}
