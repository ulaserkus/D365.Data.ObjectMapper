using D365.Data.ObjectMapper;
using Example.WithoutAttributes.DataverseModels;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.WithoutAttributes
{
    public class Example
    {
        public readonly IOrganizationService _orgService;

        public Example()
        {
            CrmServiceClient crmServiceClient = new CrmServiceClient(@"<Enter the CrmServiceClient connectionstring>");

            //ref link to connection string : https://learn.microsoft.com/en-us/power-apps/developer/data-platform/xrm-tooling/use-connection-strings-xrm-tooling-connect

            _orgService = (IOrganizationService)crmServiceClient.OrganizationWebProxyClient != null ? (IOrganizationService)crmServiceClient.OrganizationWebProxyClient : (IOrganizationService)crmServiceClient.OrganizationServiceProxy;
        }

        public List<Contact> GetContactsList()
        {
            try
            {
                QueryExpression query = new QueryExpression("contact");
                query.ColumnSet = new ColumnSet(true);
                
                EntityCollection entityCollection = _orgService.RetrieveMultiple(query);
                
                List<Contact> contacts = XrmMapper.Map<List<Contact>>(entityCollection);
                
                return contacts;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public HashSet<Contact> GetContactsHashSet()
        {
            try
            {
                QueryExpression query = new QueryExpression("contact");
                query.ColumnSet = new ColumnSet(true);

                EntityCollection entityCollection = _orgService.RetrieveMultiple(query);

                HashSet<Contact> contacts = XrmMapper.Map<HashSet<Contact>>(entityCollection);

                return contacts;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public Collection<Contact> GetContactsCollection()
        {
            try
            {
                QueryExpression query = new QueryExpression("contact");
                query.ColumnSet = new ColumnSet(true);

                EntityCollection entityCollection = _orgService.RetrieveMultiple(query);

                Collection<Contact> contacts = XrmMapper.Map<Collection<Contact>>(entityCollection);

                return contacts;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public Queue<Contact> GetContactsQueue()
        {
            try
            {
                QueryExpression query = new QueryExpression("contact");
                query.ColumnSet = new ColumnSet(true);

                EntityCollection entityCollection = _orgService.RetrieveMultiple(query);

                Queue<Contact> contacts = XrmMapper.Map<Queue<Contact>>(entityCollection);

                return contacts;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public Stack<Contact> GetContactsStack()
        {
            try
            {
                QueryExpression query = new QueryExpression("contact");
                query.ColumnSet = new ColumnSet(true);

                EntityCollection entityCollection = _orgService.RetrieveMultiple(query);

                Stack<Contact> contacts = XrmMapper.Map<Stack<Contact>>(entityCollection);

                return contacts;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public Contact GetContact()
        {
            try
            {
                Entity entity = _orgService.Retrieve("contact", Guid.Parse("{5230B95E-1BD3-EC11-A7B5-000D3A4A5AA4}"), new ColumnSet(true));

                Contact contact = XrmMapper.Map<Contact>(entity);

                return contact;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

    }
}
