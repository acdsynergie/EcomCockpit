using System;
using System.Collections.Generic;
using Serilog;

namespace EcomCockpit.Utils.EFUtils
{
    public class BasicEntity
    {
        public int ID { get; }

        public void update(BasicEntity other)
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
