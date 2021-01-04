using System;
using System.Runtime.Serialization;

namespace WebSiteTester.CLI
{
    [DataContract]
    public class Timing 
    {
        [DataMember(EmitDefaultValue = false)]
        public string Title { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Page { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string PageLoadTime { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string CssLoadTime { get; set; }
    }
}