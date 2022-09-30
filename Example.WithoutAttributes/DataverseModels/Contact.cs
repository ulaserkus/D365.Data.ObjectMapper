using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.WithoutAttributes.DataverseModels
{
    public class Contact // specifies Entity schema name 
    {
        public Guid ContactId { get; set; } // specifies Entity attributes's  contactid  , PropertyName an Type important

        public string EmailAddress1 { get; set; } // specifies Entity attributes's  emailaddress1  , PropertyName an Type important,  have to same Entity Attribute type and name

        public string MobilePhone { get; set; } // specifies Entity attributes's  mobilephone  ,  PropertyName an Type important,  have to same Entity Attribute type and name

        public string FullName { get; set; } // specifies Entity attributes's  fullName  , PropertyName important , have to same Entity Attribute type and name

        public string FirstName { get; set; } // specifies Entity attributes's  firstname  , PropertyName important , have to same Entity Attribute type and name

        public string LastName { get; set; } // specifies Entity attributes's lastname , PropertyName important , have to same Entity Attribute type and name

        public DateTime BirthDate { get; set; } // specifies Entity attributes's  birthdate name , PropertyName important , have to same Entity Attribute type and name

        public EntityReference ParentCustomerId { get; set; } // specifies Entity attributes's  parentcustomerid name , PropertyName important ,have to same Entity Attribute type and name

        public OptionSetValue GenderCode { get; set; } // specifies Entity attributes's  gendercode name , PropertyName important , have to same Entity Attribute type and name

        public OptionSetValue StatusCode { get; set; } // specifies Entity attributes's  statuscode name , PropertyName important , have to same Entity Attribute type and name

        public OptionSetValue StateCode { get; set; } // specifies Entity attributes's  statecode name , PropertyName important , have to same Entity Attribute type and name
    }
}
