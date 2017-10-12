using System;
using System.Collections.Generic;

namespace SearchUploader.Model
{
    public class BlogPost
    {
        public string id { get; set; }
        public string text { get; set; }
        public DateTime datePublished { get; set; }
        public List<string> tags { get; set; }
        public string filePath { get; set; }
    }
}
