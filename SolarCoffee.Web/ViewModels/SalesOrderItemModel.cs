using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SolarCoffee.Web.ViewModels
{
    public class SalesOrderItemModel
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public ProductModel Product { get; set; }
    }
}
