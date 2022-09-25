using D365.Data.ObjectMapper;
using Example.WithoutAttributes.DataverseModels;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.WithoutAttributes
{
    class Program
    {
        static void Main(string[] args)
        {
            //Please check contact class descriptions in DataverseModels directory

            Example example = new Example();

            #region Map EntityCollection To Generic Collection

            List<Contact> contactsList = example.GetContactsList();
            HashSet<Contact> contactsHashSet = example.GetContactsHashSet();
            Collection<Contact> contactsCollection = example.GetContactsCollection();
            Queue<Contact> contactsQueue = example.GetContactsQueue();
            Stack<Contact> contactsStack = example.GetContactsStack();

            #endregion

            #region Map Entity To Custom Class

            Contact contact = example.GetContact();

            #endregion


            #region Map Generic Collection To Entity Collection

            List<Entity> contactEntityList = contactsList.Select(ct => ct.ParseToEntity() as Entity).ToList();
            HashSet<Entity> contactEntityHashSet = contactsList.Select(ct => ct.ParseToEntity() as Entity).ToHashSet();
            Entity[] contactEntityArray = contactsList.Select(ct => ct.ParseToEntity() as Entity).ToArray();

            #endregion

            #region Map Custom Class To Entity

            Entity contactEntity = contact.ParseToEntity() as Entity;

            #endregion

            Console.ReadLine();
        }
    }
}
