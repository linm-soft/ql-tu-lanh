using System;

namespace Tulanh.Models
{
    public class Food
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Category { get; set; }

        public string Compartment { get; set; }

        public int Quantity { get; set; }

        public double Weight { get; set; }

        public DateTime ImportDate { get; set; }

        public int FreshDays { get; set; }

        public string QuantityWeight
        {
            get
            {
                return Quantity + " / " + Weight + "g";
            }
        }

        public string FreshStatus
        {
            get
            {
                DateTime expiryDate = ImportDate.AddDays(FreshDays);

                if (DateTime.Today > expiryDate)
                    return "Đã hết hạn";

                return "Còn tươi";
            }
        }
    }
}