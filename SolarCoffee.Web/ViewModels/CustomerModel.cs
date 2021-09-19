using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarCoffee.Web.ViewModels
{
    public class CustomerModel
    {
        public int Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public CustomerAddressModel PrimaryAddress { get; set; }
    }
}
