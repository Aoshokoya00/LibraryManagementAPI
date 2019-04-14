﻿using AutoMapper;
using LMSRepository.Data;
using LMSRepository.Dto;
using LMSRepository.Interfaces;
using LMSRepository.Interfaces.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ILibraryRepository _libraryRepo;
        private readonly IMapper _mapper;

        public LibraryController(DataContext context, UserManager<User> userManager, ILibraryRepository libraryRepo, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _libraryRepo = libraryRepo;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetCheckouts()
        {
            //var test = await _context.Users.Where(u => u.UserRoles.Any(r => r.RoleId == 2)).ToListAsync();

            //var test = await _userManager.GetUsersInRoleAsync("librarian");
            var test = await _libraryRepo.GetAdmins();
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(test);

            //var usertoreturn = _mapper.Map<>

            return Ok(usersToReturn);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Checkout>> GetCheckout(int id)
        {
            var checkout = await _context.Checkouts.FindAsync(id);

            if (checkout == null)
            {
                return NotFound();
            }

            return checkout;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCheckout(int id, Checkout checkout)
        {
            if (id != checkout.Id)
            {
                return BadRequest();
            }

            _context.Entry(checkout).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CheckoutExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Checkout>> PostCheckout(Checkout checkout)
        {
            _context.Checkouts.Add(checkout);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCheckout", new { id = checkout.Id }, checkout);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Checkout>> DeleteCheckout(int id)
        {
            var checkout = await _context.Checkouts.FindAsync(id);
            if (checkout == null)
            {
                return NotFound();
            }

            _context.Checkouts.Remove(checkout);
            await _context.SaveChangesAsync();

            return checkout;
        }

        private bool CheckoutExists(int id)
        {
            return _context.Checkouts.Any(e => e.Id == id);
        }
    }
}