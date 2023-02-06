using System.ComponentModel.DataAnnotations;


namespace Northwind.ViewModels.Order
{
    public class VMEdit
    {
        public int OrderId { get; set; }
        public string? CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        [Display(Name = "訂購日期")]
        public DateTime? OrderDate { get; set; }
        [Display(Name = "預計到達日期")]
        public DateTime? RequiredDate { get; set; }
        [Display(Name = "發貨日期")]
        public DateTime? ShippedDate { get; set; }
        [Display(Name = "運貨商")]
        public int? ShipVia { get; set; }
        [Display(Name = "運費")]
        public decimal? Freight { get; set; }
        [Display(Name = "貨主姓名")]
        public string? ShipName { get; set; }
        [Display(Name = "貨主地址")]
        public string? ShipAddress { get; set; }
        [Display(Name = "貨主所在城市")]
        public string? ShipCity { get; set; }
        [Display(Name = "貨主所在地區")]
        public string? ShipRegion { get; set; }
        [Display(Name = "貨主郵編")]
        public string? ShipPostalCode { get; set; }
        [Display(Name = "貨主所在國家")]
        public string? ShipCountry { get; set; }
    }
}
