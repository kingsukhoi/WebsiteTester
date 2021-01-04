using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WebSiteTester.CLI
{
    [DataContract]
    public class Config
    {
        [DataMember(Name = "wait", IsRequired = false)]
        public int Wait { get; set; } = 0;
        
        [DataMember(Name = "timeout", IsRequired = false)]
        public int Timeout { get; set; }

        [DataMember(Name = "pages", IsRequired = true)]
        public List<Page> Pages { get; set; }

        [DataMember(Name = "LogPath", IsRequired = true)]
        public string LogPath { get; set; }
    }

    [DataContract]
    public class Page
    {
        [DataMember(Name = "url", IsRequired = true)]
        public string Url { get; set; }

        [DataMember(Name = "waitSelector", IsRequired = false)]
        public string WaitSelector { get; set; } = string.Empty;

        [DataMember(Name = "takeScreenShot", IsRequired = false)]
        public bool TakeScreenShot { get; set; } = false;
        
        [DataMember(Name = "title", IsRequired = false)]
        public string Title { get; set; }
        
    }
}