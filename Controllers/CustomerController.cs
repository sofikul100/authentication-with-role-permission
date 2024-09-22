using Microsoft.AspNetCore.Mvc;
using inventory_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using inventory_api.Data;
using Microsoft.AspNetCore.Authorization;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly IECustomerService _customerService;

    public CustomerController(IECustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    [Authorize(Roles = "Manager, Admin")]
    [Authorize(Policy = "view.customer")]
    public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers([FromQuery] string search = null, [FromQuery] string sortOrder = null, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1)
    {
        var customers = await _customerService.GetAllCustomersAsync(search, sortOrder, pageNumber, pageSize);
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        if (customer == null)
        {
            return NotFound("Customer not found.");
        }
        return Ok(customer);
    }

    [HttpPost]
     [Authorize(Roles = "Manager, Admin")]
    [Authorize(Policy = "add.customer")]
    public async Task<IActionResult> Create([FromBody] CustomerCreateDTO customerCreateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _customerService.CreateAsync(customerCreateDto);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Manager,Admin")]
    [Authorize(Policy = "update.customer")]
    public async Task<IActionResult> Update(int id, [FromBody] CustomerUpdateDTO customerUpdateDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _customerService.UpdateAsync(id, customerUpdateDto);
        if (result == "Customer not found.")
        {
            return NotFound(result);
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]

    [Authorize(Roles = "Manager, Admin")] 
    [Authorize(Policy = "delete.customer")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _customerService.DeleteAsync(id);
        if (result == "Customer not found.")
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
