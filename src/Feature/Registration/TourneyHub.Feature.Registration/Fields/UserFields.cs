using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TourneyHub.Feature.Registration.Fields
{
    public class UserFields
    {
        public static readonly ID Id = new ID("{B33E6BC7-2A85-4E64-898C-EB2125866EEC}");
        public static readonly ID UsersFolderId = new ID("{BFA445E6-B096-4ECA-95D5-830F9C28B1C0}");
        public static class Fields
        {
            public static readonly ID UsernameFieldId = new ID("{04614F1F-C3FC-4B91-8034-358E892EF10D}");
            public static readonly ID NameFieldId = new ID("{8D29D842-470C-4E38-8B73-19DCD9282859}");
            public static readonly ID SurnameFieldId = new ID("{96318DE5-D99B-4250-9C3B-5A3768553884}");
            public static readonly ID EmailFieldId = new ID("{3AFA44CD-8F32-467B-B402-440F2A4E3E74}");
            public static readonly ID AgeFieldId = new ID("{94AE3167-DE97-463F-895A-5C12CD1579C0}");

        }
    }
}