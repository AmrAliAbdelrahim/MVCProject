using AutoMapper;
using Demo.DAL.Models;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.VisualStudio.Web.CodeGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
	[Authorize(Roles ="Admin")]
	public class UserController : Controller
	{
		private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserController(UserManager<ApplicationUser> userManager,IMapper mapper,SignInManager<ApplicationUser> signInManager)
        {
			_userManager = userManager;
            _mapper = mapper;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Index(string SearchValue)
		{
		 
			if(string.IsNullOrEmpty(SearchValue))
			{
				var users = await _userManager.Users.Select(
					U => new UserViwModel()
					{
						Id = U.Id,
						FName = U.FName,
						LName = U.LName,
						Email = U.Email,
						PhoneNumber = U.PhoneNumber,
						Roles = _userManager.GetRolesAsync(U).Result
					}).ToListAsync();
				return View(users);
			}
			else
			{
				var user = await _userManager.FindByEmailAsync(SearchValue);
				var MappedUser=new UserViwModel()
				{
					Id = user.Id,
					FName = user.FName,
					LName = user.LName,
					Email = user.Email,
					PhoneNumber = user.PhoneNumber,
					Roles = _userManager.GetRolesAsync(user).Result

				};
			return View(new List<UserViwModel>() {MappedUser});
			}
		}
		public async Task<IActionResult> Details(string id,string ViewName= "Details")
		{
			if (id is null)
				return BadRequest();
			var user = await _userManager.FindByIdAsync(id);
			if (user is null)
				return NotFound();
			var MappedUser = _mapper.Map<ApplicationUser, UserViwModel>(user);
			return View(ViewName, MappedUser);
		}
		public async Task<IActionResult> Edit(string Id)
		{
			return await Details(Id, "Edit");

		}
		[HttpPost]
		public async Task<IActionResult> Edit([FromRoute] string Id, UserViwModel model)
		{
			if (Id != model.Id)
				return BadRequest();
			if (ModelState.IsValid)
			{
				try
				{
					var user = await _userManager.FindByIdAsync(Id);
					user.PhoneNumber = model.PhoneNumber;
					user.FName=model.FName;
					user.LName=model.LName;
                    //var MappedUser = _mapper.Map<UserViwModel, ApplicationUser>(model);
                    await _userManager.UpdateAsync(user);
                    return RedirectToAction(nameof(Index));
                }
				catch(Exception ex)
				{
					ModelState.AddModelError(string.Empty, ex.Message);
				}
			}
			return View(model);

		}

        public async Task<IActionResult> Delete(string Id)
        {
            return await Details(Id, "Delete");

        }
		[HttpPost]
		public async Task<IActionResult> Delete([FromRoute]string Id,UserViwModel model)
		{
			if(ModelState.IsValid)
			{
				try
				{

                    var user = await _userManager.FindByIdAsync(Id);
                    user.FName = model.FName;
                    user.LName = model.LName;
                    user.PhoneNumber = model.PhoneNumber;
                    await _userManager.DeleteAsync(user);
					return RedirectToAction(nameof(Index));
                }
				catch(Exception ex)
				{
					ModelState.AddModelError(string.Empty, ex.Message);
				}


            }
				return View(model);
		}
		
    }
}
