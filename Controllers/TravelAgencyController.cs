﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Travista.Data;
using Travista.Models;
using Travista.Models.Domain;
using Microsoft.AspNetCore.Hosting;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Identity;
using System.Drawing;
using Travista.Areas.Identity.Data;

namespace Travista.Controllers
{
    public class TravelAgencyController : Controller
    {
        private readonly TravistaContext _dBContext;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<TravistaUser> _userManager;

        public TravelAgencyController(TravistaContext _dBContext, IWebHostEnvironment env, UserManager<TravistaUser> userManager)
        {
            this._dBContext = _dBContext;
            this._env = env;
            this._userManager = userManager;
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult AddTravelAgency()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Index()
        {
            var travelage = await _dBContext.TravelAgency.ToListAsync();
            return View(travelage);
        }

        [Authorize]
        private async Task<string> SaveImage(IFormFile image)
        {
            if (image != null)
            {
                var allowedExtensions = new[] { ".jpg", ".png" };
                var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("image", "Invalid file type. Only JPG and PNG files are allowed.");
                    return "noimage";
                }
                var fileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }
                return "~/uploads/" + fileName;
            }
            return "noimage";
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> AddTravelAgency(TravelAgency addUserRequest, IFormFile image)
        {
            var imagePath = await SaveImage(image);
            if (imagePath == null)
            {
                return View("AddTravelAgency");
            }
            var TravelAge = new TravelAgency()
            {
                ID_TravelAgency = 0,
                emri = addUserRequest.emri,
                description = addUserRequest.description,
                price = addUserRequest.price,
                image = imagePath,
                ID_Country = addUserRequest.ID_Country,
                ID_City = addUserRequest.ID_City,
                streetAddress = addUserRequest.streetAddress,
                additionalAddressInfo = addUserRequest.additionalAddressInfo,
                postalCode = addUserRequest.postalCode,
                phoneNumber = addUserRequest.phoneNumber,
                email = addUserRequest.email,
            };
            await _dBContext.TravelAgency.AddAsync(TravelAge);
            await _dBContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult AddUserTravelAgency()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddUserTravelAgency(TravelAgency addUserRequest, IFormFile image)
        {
            var imagePath = await SaveImage(image);
            if (imagePath == null)
            {
                return View("AddUserTravelAgency");
            }

            var currentUser = await _userManager.GetUserAsync(User);

            var TravelAge = new TravelAgency()
            {
                ID_TravelAgency = 0,
                emri = addUserRequest.emri,
                description = addUserRequest.description,
                price = addUserRequest.price,
                image = imagePath,
                ID_Country = 6,
                ID_City = addUserRequest.ID_City,
                streetAddress = addUserRequest.streetAddress,
                additionalAddressInfo = addUserRequest.additionalAddressInfo,
                postalCode = addUserRequest.postalCode,
                phoneNumber = addUserRequest.phoneNumber,
                email = currentUser.Email,
            };
            await _dBContext.TravelAgency.AddAsync(TravelAge);
            await _dBContext.SaveChangesAsync();
            return RedirectToAction("Success", "Home");
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> ViewTravelAgency(int ID_TravelAgency)
        {
            var travelage = await _dBContext.TravelAgency.FirstOrDefaultAsync(x => x.ID_TravelAgency == ID_TravelAgency);

            if (travelage != null)
            {
                var viewModel = new TravelAgency()
                {
                    ID_TravelAgency = travelage.ID_TravelAgency,
                    emri = travelage.emri,
                    description = travelage.description,
                    price = travelage.price,
                    image = travelage.image,
                    ID_Country = travelage.ID_Country,
                    ID_City = travelage.ID_City,
                    streetAddress = travelage.streetAddress,
                    additionalAddressInfo = travelage.additionalAddressInfo,
                    postalCode = travelage.postalCode,
                    phoneNumber = travelage.phoneNumber,
                    email = travelage.email
                };

                return View("ViewTravelAgency", viewModel);
            }
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> ViewTravelAgencyPost(TravelAgency model, IFormFile image)
        {
            var travelage = await _dBContext.TravelAgency.FindAsync(model.ID_TravelAgency);

            var imagePath = await SaveImage(image);
            if (imagePath == null)
            {
                return View(model);
            }

            if (travelage != null)
            {
                travelage.ID_TravelAgency = model.ID_TravelAgency;
                travelage.emri = model.emri;
                travelage.description = model.description;
                travelage.price = model.price;
                travelage.image = imagePath;
                travelage.ID_Country = model.ID_Country;
                travelage.ID_City = model.ID_City;
                travelage.streetAddress = model.streetAddress;
                travelage.additionalAddressInfo = model.additionalAddressInfo;
                travelage.postalCode = model.postalCode;
                travelage.phoneNumber = model.phoneNumber;
                travelage.email = model.email;

                await _dBContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteTravelAgency(TravelAgency model)
        {
            var travelage = await _dBContext.TravelAgency.FindAsync(model.ID_TravelAgency);

            if (travelage != null)
            {
                _dBContext.TravelAgency.Remove(travelage);
                await _dBContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}
