using System;
using System.Collections.Generic;
using System.Text;

namespace SolarCoffee.Services.Product
{
    public interface IProductService
    {
        //Our service layer will return a uniform response type: ServiceResponse. But what about the first two methods?
        List<Data.Models.Product> GetAllProducts();
        Data.Models.Product GetProductById(int id);
        ServiceResponse<Data.Models.Product> CreateProduct(Data.Models.Product product);
        ServiceResponse<Data.Models.Product> ArchiveProduct(int id);
    }
}
