using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using inventory_api.Models;
using inventory_api.Data;

public interface IECustomerService
{
    Task<IEnumerable<Customer>> GetAllCustomersAsync(string search, string sortOrder, int pageNumber, int pageSize);
    Task<CustomerDTO> GetByIdAsync(int id);
    Task<string> CreateAsync(CustomerCreateDTO customerDto);
    Task<string> UpdateAsync(int id, CustomerUpdateDTO customerDto);
    Task<string> DeleteAsync(int id);
}






public class CustomerService : IECustomerService
{
    private readonly IECustomerRepository _repository;
    private readonly IMapper _mapper;

    public CustomerService(IECustomerRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Customer>> GetAllCustomersAsync(string search, string sortOrder, int pageNumber, int pageSize)
    {
        return await _repository.GetAllCustomersAsync(search, sortOrder, pageNumber, pageSize);
    }

    public async Task<CustomerDTO> GetByIdAsync(int id)
    {
        var customer = await _repository.GetByIdAsync(id);
        return _mapper.Map<CustomerDTO>(customer);
    }

    public async Task<string> CreateAsync(CustomerCreateDTO customerDto)
    {
        var customer = _mapper.Map<Customer>(customerDto);
        customer.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(customer);
        return "Customer added successfully.";
    }

    public async Task<string> UpdateAsync(int id, CustomerUpdateDTO customerDto)
    {
        var existingCustomer = await _repository.GetByIdAsync(id);
        if (existingCustomer == null)
        {
            return "Customer not found.";
        }

        _mapper.Map(customerDto, existingCustomer);
        existingCustomer.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(existingCustomer);
        return "Customer updated successfully.";
    }

    public async Task<string> DeleteAsync(int id)
    {
        var existingCustomer = await _repository.GetByIdAsync(id);
        if (existingCustomer == null)
        {
            return "Customer not found.";
        }

        await _repository.DeleteAsync(id);
        return "Customer deleted successfully.";
    }
}
