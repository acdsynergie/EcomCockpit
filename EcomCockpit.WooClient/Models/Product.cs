using System;
using Serilog;

namespace EcomCockpit.WooClient.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class Product
    {
        public int ID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        internal void update(Product other)
        {
            try
            {
                var typeOfOther = other.GetType();
                Array.ForEach(GetType().GetProperties(), prop => prop.SetValue(this, typeOfOther.GetProperty(prop.Name).GetValue(typeOfOther)));
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }
    }
}

