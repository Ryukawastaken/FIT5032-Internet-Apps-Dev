using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace FIT5032Assignment.Models
{
    public class Client
    {
        //[Key]
        public int ClientID { get; set; }
        public string clientFirstName { get; set; }
        public string clientLastName { get; set; }
        [DataType(DataType.Date)]
        public DateTime clientDOB { get; set; }
        public string clientMedicareNo { get; set; }
        public string clientMedicareUniqueID { get; set; }
        public string clientAddress { get; set; }
        public string clientSuburb { get; set; }
        public string clientPostcode { get; set; }
        public string clientPhoneNumber { get; set; }
        public string clientEmail { get; set; }
        public List<Appointment> appointments { get; set; }

        public Client()
        {

        }

        public Client(string newFirstName, string newLastname, DateTime newDOB, string newMedicareNo,
            string newMedicareUniqueID, string newAddress, string newPostcode, string newPhoneNumber, string newEmail)
        {
            clientFirstName = newFirstName;
            clientLastName = newLastname;
            clientDOB = newDOB;
            clientMedicareNo = newMedicareNo;
            clientMedicareUniqueID = newMedicareUniqueID;
            clientAddress = newAddress;
            clientSuburb = newPostcode;
            clientPostcode = newPostcode;
            clientEmail = newEmail;
        }

       /* public int GetID()
        {
            return ClientID;
        }

        public bool SetClientID(int newID)
        {
            ClientID = newID;
            return true;
        }*/

        public string GetClientFirstName()
        {
            return clientFirstName;
        }

        public bool SetClientFirstName(string newFirstName)
        {
            clientFirstName = newFirstName;
            return true;
        }

        public string GetClientLastName()
        {
            return clientLastName;
        }

        public bool SetClientLastName(string newLastName)
        {
            clientLastName = newLastName;
            return true;
        }

        public DateTime GetClientDOB()
        {
            return clientDOB;
        }

        public bool SetClientDOB(DateTime newDOB)
        {
            clientDOB = newDOB;
            return true;
        }

        public string GetClientMedicareNo()
        {
            return clientMedicareNo;
        }

        public bool SetClientMedicareNo(string newMedicareNo)
        {
            clientMedicareNo = newMedicareNo;
            return true;
        }

        public string GetClientMedicareUniqueID()
        {
            return clientMedicareUniqueID;
        }

        public bool SetClientMedicareUniqueID(string newMedicareUniqueID)
        {
            clientMedicareUniqueID = newMedicareUniqueID;
            return true;
        }

        public string GetClientAddress()
        {
            return clientAddress;
        }

        public bool SetClientAddress(string newAddress)
        {
            clientAddress = newAddress;
            return true;
        }

        public string GetClientSuburb()
        {
            return clientSuburb;
        }

        public bool SetClientSuburb(string newSuburb)
        {
            clientSuburb = newSuburb;
            return true;
        }

        public string GetClientPostcode()
        {
            return clientPostcode;
        }

        public bool SetClientPostcode(string newPostcode)
        {
            clientPostcode = newPostcode;
            return true;
        }

        public string GetClientPhoneNumber()
        {
            return clientPhoneNumber;
        }

        public bool SetClientPhoneNumber(string newPhoneNumber)
        {
            clientPhoneNumber = newPhoneNumber;
            return true;
        }

        public string GetClientEmail()
        {
            return clientEmail;
        }

        public bool SetClientEmail(string newEmail)
        {
            clientEmail = newEmail;
            return true;
        }

        public List<Appointment> GetClientAppointments()
        {
            return appointments;
        }

        public bool SetClientAppointments(List<Appointment> newAppointments)
        {
            appointments = newAppointments;
            return true;
        }



    }
}