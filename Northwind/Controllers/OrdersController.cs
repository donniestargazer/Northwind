using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Northwind.Models;
using Northwind.ViewModels.Order;

namespace Northwind.Controllers
{
    public class OrdersController : Controller
    {
        private readonly NorthwindContext _context;

        public OrdersController(NorthwindContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            List<VMIndex> OrderList 
                = await (from o in _context.Orders
                         select new VMIndex
                         {
                             OrderId = o.OrderId,
                             OrderDate = o.OrderDate,
                             Freight = o.Freight,
                             ShipName = o.ShipName,
                             ShipCountry = o.ShipCountry
                         }).ToListAsync();
            return View(OrderList);
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.ShipViaNavigation)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId");
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId");
            ViewData["ShipVia"] = new SelectList(_context.Shippers, "ShipperId", "ShipperId");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId,CustomerId,EmployeeId,OrderDate,RequiredDate,ShippedDate,ShipVia,Freight,ShipName,ShipAddress,ShipCity,ShipRegion,ShipPostalCode,ShipCountry")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CustomerId", order.CustomerId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", order.EmployeeId);
            ViewData["ShipVia"] = new SelectList(_context.Shippers, "ShipperId", "ShipperId", order.ShipVia);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            VMEdit vmEdit = new VMEdit()
                            {
                                OrderId = order.OrderId,
                                CustomerId = order.CustomerId,
                                EmployeeId = order.EmployeeId,
                                OrderDate = order.OrderDate,
                                RequiredDate = order.RequiredDate,
                                ShippedDate = order.ShippedDate,
                                ShipVia = order.ShipVia,
                                Freight = order.Freight,
                                ShipAddress = order.ShipAddress,
                                ShipCity = order.ShipCity,
                                ShipRegion = order.ShipRegion,
                                ShipPostalCode = order.ShipPostalCode,
                                ShipCountry = order.ShipCountry
                            };

            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CompanyName", vmEdit.CustomerId); //CompanyName 對應 ShipName
            //ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", order.EmployeeId); 人名分為 名(First Name) 與 姓(Last Name)，顯示上有困難
            ViewData["ShipVia"] = new SelectList(_context.Shippers, "ShipperId", "CompanyName", vmEdit.ShipVia);
            return View(vmEdit);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,CustomerId,EmployeeId,OrderDate,RequiredDate,ShippedDate,ShipVia,Freight,ShipAddress,ShipCity,ShipRegion,ShipPostalCode,ShipCountry")] VMEdit vmEdit)
        {
            if (id != vmEdit.OrderId)
            {
                return NotFound();
            }
            
            if (ModelState.IsValid)
            {

                Customer customer = _context.Customers.Where(c => c.CustomerId == vmEdit.CustomerId).FirstOrDefault();

                Order order = new Order
                {
                    OrderId = vmEdit.OrderId,
                    CustomerId = vmEdit.CustomerId,
                    EmployeeId = vmEdit.EmployeeId,
                    OrderDate = vmEdit.OrderDate,
                    RequiredDate = vmEdit.RequiredDate,
                    ShippedDate = vmEdit.ShippedDate,
                    ShipVia = vmEdit.ShipVia,
                    Freight = vmEdit.Freight,
                    ShipName = customer.CompanyName,
                    ShipAddress = vmEdit.ShipAddress,
                    ShipCity = vmEdit.ShipCity,
                    ShipRegion = vmEdit.ShipRegion,
                    ShipPostalCode = vmEdit.ShipPostalCode,
                    ShipCountry = vmEdit.ShipCountry
                };

                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "CompanyName", vmEdit.CustomerId); //CompanyName 對應 ShipName
            //ViewData["EmployeeId"] = new SelectList(_context.Employees, "EmployeeId", "EmployeeId", order.EmployeeId); 人名分為 名(First Name) 與 姓(Last Name)，顯示上有困難
            ViewData["ShipVia"] = new SelectList(_context.Shippers, "ShipperId", "CompanyName", vmEdit.ShipVia);
            return View(vmEdit);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.ShipViaNavigation)
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'NorthwindContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
