using System.ComponentModel.DataAnnotations;


namespace Northwind.ViewModels.Order
{
    public class VMIndex
    {
        public int OrderId { get; set; }        
        [Display(Name = "訂購日期")]
        public DateTime? OrderDate { get; set; }        
        [Display(Name = "運費")]
        public decimal? Freight { get; set; }
        [Display(Name = "貨主名稱")]
        public string? ShipName { get; set; }
        [Display(Name = "貨主所在國家")]
        public string? ShipCountry { get; set; }
    }
}
