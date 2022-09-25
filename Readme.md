Description

This library helps about Microsoft.Xrm.Sdk.Entity or Microsoft.Xrm.Sdk.EntityCollection objects convert to your custom class objects and generic collections like list,hashset,queue,stack or collection and parse your custom class objects to Microsoft.Xrm.Sdk.Entity.Library designed for only Microsoft.Xrm.Sdk.Entity or Microsoft.Xrm.Sdk.EntityCollection mapping and parsing

Attributes

Basic Usages :
Attributes helps to design our custom classes more flexible

    [Schema("contact")]// specifies entity schema name
    public class Contact
    {
        [PrimaryKey] // maps entity , id prop name not important and property type must be guid
        public Guid ContactId { get; set; }

        [MapFrom("emailaddress1")]// maps entity attribute's emailaddress1 , property name not important
        public string Email { get; set; }

        [MapFrom("mobilephone")]// maps entity attribute's mobilephone , property name not important
        public string PhoneNumber { get; set; }

        [MapFrom("fullname")]// maps entity attribute's fullname , property name not important
        public string FullName { get; set; }

        [MapFrom("firstname")]// maps entity attribute's firstname , property name not important
        public string FirstName { get; set; }

        [MapFrom("lastname")]// maps entity attribute's lastname , property name not important
        public string LastName { get; set; }

        [MapFrom("birthdate")]// maps entity attribute's birthdate , property name not important
        public DateTime BirtDate { get; set; }

        [MapFrom("parentcustomerid")]// maps entity attribute's parentcustomerid , property name not important 
        [Reference("account")]// specifies entityreference schema name and gets reference id
        public Guid ParentAccountId { get; set; }

        [MapFrom("gendercode")]// maps entity attribute's gendercode , property name not important 
        [OptionSetAsInt]// if your entity attribute is optionsetvalue use to get value as int
        public int Gender { get; set; }

        [MapFrom("statuscode")]// maps entity attribute's statuscode , property name not important 
        [OptionSetAsInt]// if your entity attribute is optionsetvalue use to get value as int
        public int Status { get; set; }

        [MapFrom("statecode")]// maps entity attribute's statecode , property name not important
        [OptionSetAsInt]// if your entity attribute is optionsetvalue use to get value as int
        public int State { get; set; }
    }

XrmMapper Usage
Please check examples in github repository to detailed description
Basic Usages :

Mapping

EntityCollection entityCollection = _orgService.RetrieveMultiple(query);
List<Contact> contacts = XrmMapper.Map<List<Contact>>(entityCollection);

Entity entity = _orgService.Retrieve("contact", Guid.Parse("{5230B95E-1BD3-EC11-A7B5-000D3A4A5AA4}"), new ColumnSet(true));
Contact contact = XrmMapper.Map<Contact>(entity);

Parsing
    
Entity contactEntity = contact.ParseToEntity() as Entity;
List<Entity> contactEntityList=contactsList.Select(ct => ct.ParseToEntity() as Entity).ToList();


