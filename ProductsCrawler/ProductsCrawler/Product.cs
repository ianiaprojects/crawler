using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductsCrawler
{
    class Product
    {
        public string Name;
        public double Price;

        public bool Equals(Product product)
        {
            if (Object.ReferenceEquals(product, null)) return false;
            if (Object.ReferenceEquals(this, product)) return true;
            return Name.Equals(product.Name) && Price.Equals(product.Price);
        }

        public override int GetHashCode()
        {
            int hashName = Name == null ? 0 : Name.GetHashCode();
            int hashPrice = Price.GetHashCode();
            return hashName ^ hashPrice;
        }
    }
}
