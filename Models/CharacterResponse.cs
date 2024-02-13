namespace Yoyo.Models
{
    public class CharacterResponse
    {
        public Info? info { get; set; }
        public List<Character>? results { get; set; }
    }

    public class Info
    {
        public int? count { get; set; }
        public int? pages { get; set; }
        public string? next { get; set; }
        public string? prev { get; set; }
    }


    public class Character
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? status { get; set; }
        public string? species { get; set; }
        public string? type { get; set; }
        public string? gender { get; set; }
        public Link? origin { get; set; }
        public Link? location { get; set; }
        public string? image { get; set; }
        public List<string>? episode { get; set; }
        public string? url { get; set; }
        public string? created { get; set; }
    }

    public class Link
    {
        public string? name { get; set; }
        public string? url { get; set; }
    }

}
