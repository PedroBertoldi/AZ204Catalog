using System;

namespace fnPostDatabase
{
    internal class MovieRequest {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public int Year { get; set; }
        public string VideoUrl { get; set; }
        public string ThumbUrl { get; set; }
    }
}