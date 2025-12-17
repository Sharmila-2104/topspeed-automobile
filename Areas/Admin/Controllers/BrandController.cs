using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TopSpeed.Application.ApplicationConstants;
using TopSpeed.Application.Contracts.Presistence;
using TopSpeed.Domain.Models;
using TopSpeed.Infrastructure.Common;

namespace TopSpeed.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =CustomRole.MasterAdmin+","+CustomRole.Admin)]
    public class BrandController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<BrandController> _logger;
        public BrandController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, ILogger<BrandController> logger)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                List<Brand> brand = await _unitOfWork.Brand.GetAllAsync();

                _logger.LogInformation("Brand List Fetched from Database Successfully");

                return View(brand);
            }
            catch(Exception ex)
            {
                _logger.LogError("Something went wrong");
                return View();
            }
            //List<Brand> brand = await _unitOfWork.Brand.GetAllAsync();
            //return View(brand);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Brand brand)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;

            var file = HttpContext.Request.Form.Files;

            if (file.Count > 0)
            {
                string newFileName = Guid.NewGuid().ToString();

                var upload = Path.Combine(webRootPath, @"images\brand");

                var extension = Path.GetExtension(file[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(upload, newFileName + extension), FileMode.Create))
                {
                    file[0].CopyTo(fileStream);
                }
                brand.BrandLogo = @"\images\brand\" + newFileName + extension;
            }
            if (ModelState.IsValid)
            {
                await _unitOfWork.Brand.Create(brand);
                await _unitOfWork.SaveAsync();

                TempData["Success"] = CommonMessage.RecordCreate;

                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            Brand brand = await _unitOfWork.Brand.GetByIdAsync(id);

            return View(brand);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            Brand brand = await _unitOfWork.Brand.GetByIdAsync(id);

            return View(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Brand brand)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;

            var file = HttpContext.Request.Form.Files;

            if (file.Count > 0)
            {
                string newFileName = Guid.NewGuid().ToString();

                var upload = Path.Combine(webRootPath, @"images\brand");

                var extension = Path.GetExtension(file[0].FileName);

                //delete old images
                var objFromDB = await _unitOfWork.Brand.GetByIdAsync(brand.Id);

                if (objFromDB.BrandLogo != null)
                {
                    var oldimagepath = Path.Combine(webRootPath, objFromDB.BrandLogo.Trim('\\'));

                    if (System.IO.File.Exists(oldimagepath))
                    {
                        System.IO.File.Delete(oldimagepath);
                    }
                }
                using (var fileStream = new FileStream(Path.Combine(upload, newFileName + extension), FileMode.Create))
                {
                    file[0].CopyTo(fileStream);
                }
                brand.BrandLogo = @"\images\brand\" + newFileName + extension;
            }

            if (ModelState.IsValid)
            {
                var objFromDB = await _unitOfWork.Brand.GetByIdAsync(brand.Id);

                await _unitOfWork.Brand.Update(brand);
                await _unitOfWork.SaveAsync();

                TempData["Warning"] = CommonMessage.RecordUpdate;
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            Brand brand = await _unitOfWork.Brand.GetByIdAsync(id);

            return View(brand);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Brand brand)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;

            if (!string.IsNullOrEmpty(brand.BrandLogo))
            {
                var objFromDB = await _unitOfWork.Brand.GetByIdAsync(brand.Id);

                if (objFromDB.BrandLogo != null)
                {
                    var oldimagepath = Path.Combine(webRootPath, objFromDB.BrandLogo.Trim('\\'));

                    if (System.IO.File.Exists(oldimagepath))
                    {
                        System.IO.File.Delete(oldimagepath);

                    }
                }
            }
            await _unitOfWork.Brand.Delete(brand);
            await _unitOfWork.SaveAsync();

            TempData["error"] = CommonMessage.RecordDelete;
            return RedirectToAction("Index");
        }
    }
}
