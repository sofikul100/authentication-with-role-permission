using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inventory_api.Models;
using Microsoft.EntityFrameworkCore;

public interface IECustomerRepository
{
    Task<IEnumerable<Customer>> GetAllCustomersAsync(string search, string sortOrder, int pageNumber, int pageSize);
    Task<Customer> GetByIdAsync(int id);
    Task AddAsync(Customer customer);
    Task UpdateAsync(Customer customer);
    Task DeleteAsync(int id);
}



public class CustomerRepository : IECustomerRepository
{
    private readonly AppDbContext _context;

    public CustomerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Customer>> GetAllCustomersAsync(string search, string sortOrder, int pageNumber, int pageSize)
    {
        var customers = _context.Customers.AsQueryable();

        // Search
        if (!string.IsNullOrEmpty(search))
        {
            customers = customers.Where(c => c.Name.Contains(search) || c.Email.Contains(search));
        }

        // Sorting
        switch (sortOrder)
        {
            case "name_desc":
                customers = customers.OrderByDescending(c => c.Name);
                break;
            default:
                customers = customers.OrderBy(c => c.Name);
                break;
        }

        // Pagination
        customers = customers.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return await customers.ToListAsync();
    }

    public async Task<Customer> GetByIdAsync(int id)
    {
        return await _context.Customers.FindAsync(id);
    }

    public async Task AddAsync(Customer customer)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Customer customer)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var customer = await _context.Customers.FindAsync(id);
        if (customer != null)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }
    }
}
