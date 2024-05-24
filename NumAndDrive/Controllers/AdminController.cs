﻿using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NumAndDrive.Database;
using NumAndDrive.Models;
using NumAndDrive.Models.Repositories;
using NumAndDrive.ViewModels;

namespace NumAndDrive.Controllers
{
    public class AdminController : Controller
    {

        public IFormFile? File { get; set; }
        private readonly UserManager<User> _userManager;
        private readonly NumAndDriveDbContext Db;
        private readonly IAdminRepository _adminRepository;
 
        public AdminController(NumAndDriveDbContext db, UserManager<User> userManager, IAdminRepository adminRepository)
        {
            Db = db;
            _userManager = userManager;
            _adminRepository = adminRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetUsers()
        {
            IEnumerable<User> users = Db.Users.Where(x => x.ArchiveDate == null).ToList();
            return View(users);
        }

        public IActionResult Details(string id)
        {
            User user = Db.Users.Find(id);
            return View(user);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            User user = Db.Users.Find(id);
            return View(user);
        }

        [HttpPost]
        public IActionResult Edit(User user, string id)
        {
            User ToFindUser = Db.Users.Find(id);

            if (ToFindUser != null)
            {
                ToFindUser.LastName = user.LastName;
                ToFindUser.FirstName = user.FirstName;
                ToFindUser.Email = user.Email;

                Db.SaveChanges();
            }

            return RedirectToAction(nameof(GetUsers));
        }

        public IActionResult Archive(string id)
        {
            User user = Db.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            user.ArchiveDate = DateOnly.FromDateTime(DateTime.UtcNow);
            Db.SaveChanges();

            return RedirectToAction(nameof(GetUsers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadUsersFromCSVFile(Admin admin)
        {
            await _adminRepository.UploadUsersFromCSVFile(admin, _userManager);
            return RedirectToAction(nameof(Index));
        }

            public async Task<IActionResult> CreateSingleUser()
        {
            CreateUserViewModel createSingleUserViewModel = new CreateUserViewModel
            {
                Statuses = Db.Statuses,
                Departments = Db.Departments
            };
            return View(createSingleUserViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSingleUser(User user)
        {
            user.UserName = user.Email;
            user.ProfilePicturePath = "";
            await _userManager.CreateAsync(user, _adminRepository.PasswordGenerator());
            return RedirectToAction(nameof(Index));
        }
    }
}

