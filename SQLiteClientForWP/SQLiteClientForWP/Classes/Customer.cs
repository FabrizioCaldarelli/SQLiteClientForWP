using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQLiteClientForWP.Classes
{
    public class Customer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Desc { get; set; }
        public Customer() { }
        public Customer(int Id, string name, string email, string desc)
        {
            ID = Id;
            Name = name;
            Email = email;
            Desc = desc;
        }
        //public event PropertyChangedEventHandler PropertyChanged;
        //public void RaisePropertyChanged(string propertyName)   
        //{   
        //    var handler = PropertyChanged;   
        //    if (handler != null)   
        //    {   
        //        handler(this, new PropertyChangedEventArgs(propertyName));   
        //    }   
        //}   
    }
}
