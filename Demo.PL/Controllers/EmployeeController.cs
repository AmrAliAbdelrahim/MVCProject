using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.DAL.Models;
using Demo.PL.Helpers;
using Demo.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Demo.PL.Controllers
{
	[Authorize]
	public class EmployeeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EmployeeController(/*IEmployeeRepository employeeRepo*/ IUnitOfWork unitOfWork ,IMapper mapper)
        {
            //_employeeRepo = employeeRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            //_departmentRepo = departmentRepo;
        }

        public IActionResult Index( string SearchName)
        {
            var employees = Enumerable.Empty<Employee>();
            if (string.IsNullOrEmpty(SearchName))
                employees = _unitOfWork.EmployeeRepository.GetAll();

            else
               // var MappedEmps = _mapper.Map<IEnumerable<Employee>, IEnumerable<EmployeeViewModel>>(employees);
                employees = _unitOfWork.EmployeeRepository.SearchByName(SearchName);
               var MappedEmps=_mapper.Map<IEnumerable<Employee>,IEnumerable<EmployeeViewModel>>(employees);
            return View(MappedEmps);
        }
    
        public IActionResult Create()
        {
            //ViewData["Departments"] = _departmentRepo.GetAll();
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EmployeeViewModel employeeVM)
        {
            if (ModelState.IsValid)
            {
                ///var MappedEmp = new Employee()
                ///{
                ///Name= employeeVM.Name,
                ///Age= employeeVM.Age,
                ///Salary= employeeVM.Salary,
                ///Email= employeeVM.Email,
                ///PhoneNumber=employeeVM.PhoneNumber,
                ///IsActive= employeeVM.IsActive,
                ///HireDate= employeeVM.HireDate
                ///};


             employeeVM.ImageName=DocumentSettings.UploadFile(employeeVM.Image, "Images");

                var MappedEmp=_mapper.Map<EmployeeViewModel,Employee>(employeeVM);
              
                _unitOfWork.EmployeeRepository.Add(MappedEmp);
            
             var count=_unitOfWork.Complete();
 
                if (count > 0)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(employeeVM);
        }

        public IActionResult Details(int? id, string ViewName = "Details")
        {
            if (id is null)
                return BadRequest();//400
            var employee = _unitOfWork.EmployeeRepository.GetById(id.Value);
            var MappedEmp = _mapper.Map<Employee, EmployeeViewModel>(employee);
            if (employee is null)
                return NotFound();//404
            return View(ViewName, MappedEmp);
        }
        public IActionResult Edit(int? id)
        {
            //ViewData["Departments"] = _departmentRepo.GetAll();

            return Details(id, "Edit");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([FromRoute] int id, EmployeeViewModel employeeVM)
        {
            if (id != employeeVM.Id)
                return BadRequest();
            if (ModelState.IsValid)
            {
                try
                {
                    if(employeeVM.Image is not null)
                    {

                    employeeVM.ImageName = DocumentSettings.UploadFile(employeeVM.Image, "Images");
                    }
                    var MappedEmp = _mapper.Map<EmployeeViewModel, Employee>(employeeVM);

                    _unitOfWork.EmployeeRepository.Update(MappedEmp);
                     
                    _unitOfWork.Complete();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);

                }
            }
            return View(employeeVM);
        }
        public IActionResult Delete(int? id)
        {
            return Details(id, "Delete");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete([FromRoute] int id, EmployeeViewModel employeeVM)
        {
            if (id != employeeVM.Id)
                return BadRequest();
            try
            {
                var MappedEmp = _mapper.Map<EmployeeViewModel, Employee>(employeeVM);

                _unitOfWork.EmployeeRepository.Delete(MappedEmp);

               var Result= _unitOfWork.Complete();
                //Check To Delete Image
                if(Result>0&&employeeVM.ImageName is not null)
                {
                    DocumentSettings.DeleteFile(employeeVM.ImageName, "Images");
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, ex.Message);
                return View(employeeVM);
            }
        }
    }
}
