using AutoMapper;
using Demo.DAL.Models;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
    [Authorize(Roles = "Admin")]

    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleController(RoleManager<IdentityRole> roleManager,IMapper mapper,UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string  SearchValue)
        {
            if (string.IsNullOrEmpty(SearchValue))
            {
               var Roles= await _roleManager.Roles.ToListAsync();
                var MappedRole=_mapper.Map<IEnumerable<IdentityRole>,IEnumerable< RoleViewModel >>(Roles);
            return View(MappedRole);
            }
            else
            {
                var Role =await _roleManager.FindByNameAsync(SearchValue);
                var MappedRole =  _mapper.Map< IdentityRole, RoleViewModel>(Role);

                return View(new List<RoleViewModel>(){ MappedRole});
            }
        }
        public  IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var MappedRole=_mapper.Map<RoleViewModel,IdentityRole>(model);
                var Role = await _roleManager.CreateAsync(MappedRole);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid Role");
            }
            return View(model);
        }
        public async Task<IActionResult> Details(string id, string ViewName = "Details")
        {
            if (id is null)
                return BadRequest();
            var Role = await _roleManager.FindByIdAsync(id);
            if (Role is null)
                return NotFound();
            var MappedRole = _mapper.Map<IdentityRole, RoleViewModel>(Role);
            return View(ViewName, MappedRole);
        }
        public async Task<IActionResult> Edit(string Id)
        {
            return await Details(Id, "Edit");

        }
        [HttpPost]
        public async Task<IActionResult> Edit([FromRoute] string Id, RoleViewModel model)
        {
            if (Id != model.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    var Role = await _roleManager .FindByIdAsync(Id);

                    Role.Name = model.RoleName;
                    //var MappedUser = _mapper.Map<UserViwModel, ApplicationUser>(model);
                    await _roleManager.UpdateAsync(Role);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
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
        public async Task<IActionResult> Delete([FromRoute] string Id, RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var Role = await _roleManager.FindByIdAsync(Id);
                    Role.Name = model.RoleName;
                    
                    await _roleManager.DeleteAsync(Role);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }


            }
            return View(model);
        }
        public async Task<IActionResult> AddOrRemoveUsers(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound();
            ViewBag.RoleId = roleId;
            var usersInRole = new List<UserInRoleViewModel>();
            var users = await _userManager.Users.ToListAsync();
            foreach(var user in users)
            {
                var UserInRole = new UserInRoleViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                    UserInRole.IsSelected=true;
                else UserInRole.IsSelected=false;
                usersInRole.Add(UserInRole);

            }
            return View(usersInRole);
        }
        [HttpPost]
        public async Task<IActionResult> AddOrRemoveUsers(string roleId, List<UserInRoleViewModel> users)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
                return NotFound();
            if(ModelState.IsValid)
            {
                foreach(var  user in users)
                {
                    var appuser = await _userManager.FindByIdAsync(user.UserId);
                    if (appuser != null)
                    {
                        if(user.IsSelected & !await _userManager.IsInRoleAsync(appuser, role.Name))
                        
                            await _userManager.AddToRoleAsync(appuser, role.Name);
                       else if(!user.IsSelected & await _userManager.IsInRoleAsync(appuser, role.Name))
                            await _userManager.RemoveFromRoleAsync(appuser, role.Name);
                         
                    }

                }
            return RedirectToAction("Edit", new { Id = roleId });
            }
            return View(users);
        }

    }
}
