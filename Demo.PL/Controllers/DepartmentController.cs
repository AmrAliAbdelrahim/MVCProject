﻿using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.DAL.Models;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Demo.PL.Controllers
{
	[Authorize]

	public class DepartmentController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentRepository _departmentRepo;

        public DepartmentController(IMapper mapper,/*IDepartmentRepository departmentRepo*/ IUnitOfWork unitOfWork)
        {
             _mapper = mapper;
            _unitOfWork = unitOfWork;
           // _departmentRepo = departmentRepo;
        }
        public IActionResult Index()
        {
            var departments = _unitOfWork.DepartmentRepository.GetAll();
            var MappedDM=_mapper.Map<IEnumerable<Department>, IEnumerable<DepartmentViewModel>>(departments);
            return View(MappedDM);
        }
        
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DepartmentViewModel departmentVM)
        {
            if(ModelState.IsValid)
            {
                var MappedDM = _mapper.Map<DepartmentViewModel, Department>(departmentVM);
                _unitOfWork.DepartmentRepository.Add(MappedDM);
                var count = _unitOfWork.Complete();
                if (count > 0)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(departmentVM);
        }
        
        public IActionResult Details(int? id, string ViewName="Details")
        {
            if (id is null)
                return BadRequest();//400

            var department = _unitOfWork.DepartmentRepository.GetById(id.Value);
            var MappedDM = _mapper.Map<Department ,DepartmentViewModel>(department);


            if (department is null)
                return NotFound();//404
            //var MappedDM = _mapper.Map<DepartmentViewModel, Department>(departmentVM);


            return View (ViewName, MappedDM);
        }

        public IActionResult Edit(int? id)
        {
            return Details(id , "Edit");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute]int id, DepartmentViewModel departmentVM)
        {
            if (id != departmentVM.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    var MappedDM = _mapper.Map<DepartmentViewModel, Department>(departmentVM);

                    _unitOfWork.DepartmentRepository.Update(MappedDM);
					_unitOfWork.Complete();
					return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    
                }
            }
            return View(departmentVM);
        }
        public IActionResult Delete(int? id)
        {
            return Details(id,"Delete");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete([FromRoute]int id , DepartmentViewModel departmentVM)
        {
            if(id != departmentVM.Id)
                return BadRequest();
            try
            {
                var MappedDM = _mapper.Map<DepartmentViewModel, Department>(departmentVM);

                _unitOfWork.DepartmentRepository.Delete(MappedDM);
				_unitOfWork.Complete();
				return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty,ex.Message);
                return View(departmentVM);
            }
        }
        
    }
}