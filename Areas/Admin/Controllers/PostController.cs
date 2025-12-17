using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TopSpeed.Application.ApplicationConstants;
using TopSpeed.Application.Contracts.Presistence;
using TopSpeed.Application.Services.Interface;
using TopSpeed.Domain.ApplicationEnum;
using TopSpeed.Domain.Models;
using TopSpeed.Domain.ViewModel;
using TopSpeed.Infrastructure.Common;



namespace TopSpeed.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =CustomRole.MasterAdmin+","+CustomRole.Admin)]
    public class PostController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IUserNameService _userName;

        public Post Post { get; private set; }

        public PostController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, IUserNameService userName)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _userName = userName;   
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            List<Post> posts = await _unitOfWork.Post.GetAllPost();
            return View(posts);
        }  

        [HttpGet]
        public IActionResult Create() 
        { 
            IEnumerable<SelectListItem> brandList = _unitOfWork.Brand.Query().Select(x => new SelectListItem
            { 
                Text = x.Name.ToUpper(),
                Value= x.Id.ToString()  
            });

            IEnumerable<SelectListItem> vehicleTypeList = _unitOfWork.VehicleType.Query().Select(x => new SelectListItem {
            
              Text= x.Name.ToUpper(),   
              Value= x.Id.ToString()    
            
            });

            IEnumerable<SelectListItem> engineAndFuelList = Enum.GetValues(typeof(EngineAndFuelType))
                .Cast<EngineAndFuelType>()
                .Select(x => new SelectListItem
                {
                    Text = x.ToString().ToUpper(),
                    Value = ((int)(x)).ToString()
                });
            IEnumerable<SelectListItem> transmissionList = Enum.GetValues(typeof(Transmission))
                  .Cast<Transmission>()
                  .Select(x => new SelectListItem
                  {
                      Text = x.ToString().ToUpper(),    
                      Value = ((int)(x)).ToString() 
                  });

            PostVM postVM = new PostVM
            {
                Post = new Post(),
                BrandList = brandList,
                VehicleTypeList = vehicleTypeList,
                EngineAndFuelList= engineAndFuelList,
                TransmissionList = transmissionList 
            };
            return View(postVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PostVM postVM)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;

            var file = HttpContext.Request.Form.Files;

            if (file.Count > 0)
            {
                string newFileName = Guid.NewGuid().ToString();

                var upload = Path.Combine(webRootPath, @"images\post");

                var extension = Path.GetExtension(file[0].FileName);

                using (var fileStream = new FileStream(Path.Combine(upload, newFileName + extension), FileMode.Create))
                {
                    file[0].CopyTo(fileStream);
                }
               postVM.Post.vehicleImage = @"\images\post\" + newFileName + extension;
            }
            if (ModelState.IsValid)
            {
                await _unitOfWork.Post.Create(postVM.Post);
                await _unitOfWork.SaveAsync();

                TempData["Success"] = CommonMessage.RecordCreate;

                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            Post post = await _unitOfWork.Post.GetPostById(id);

            //if (post == null)
            //{
            //    // Handle case where the post does not exist
            //    return NotFound();  // Or redirect to an error page or another view
            //}
            post.CreatedBy = await _userName.GetUserName(post.CreatedBy);

            post.ModifiedBy = await _userName.GetUserName(post.ModifiedBy);


            return View(post);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            Post post = await _unitOfWork.Post.GetPostById(id);

            IEnumerable<SelectListItem> brandList = _unitOfWork.Brand.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });

            IEnumerable<SelectListItem> vehicleTypeList = _unitOfWork.VehicleType.Query().Select(x => new SelectListItem
            {

                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()

            });

            IEnumerable<SelectListItem> engineAndFuelList = Enum.GetValues(typeof(EngineAndFuelType))
                .Cast<EngineAndFuelType>()
                .Select(x => new SelectListItem
                {
                    Text = x.ToString().ToUpper(),
                    Value = ((int)(x)).ToString()
                });
            IEnumerable<SelectListItem> transmissionList = Enum.GetValues(typeof(Transmission))
                  .Cast<Transmission>()
                  .Select(x => new SelectListItem
                  {
                      Text = x.ToString().ToUpper(),
                      Value = ((int)(x)).ToString()
                  });

            PostVM postVM = new PostVM
            {
                Post = post,
                BrandList = brandList,
                VehicleTypeList = vehicleTypeList,
                EngineAndFuelList = engineAndFuelList,
                TransmissionList = transmissionList
            };
            return View(postVM);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PostVM postVM)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;

            var file = HttpContext.Request.Form.Files;

            if (file.Count > 0)
            {
                string newFileName = Guid.NewGuid().ToString();

                var upload = Path.Combine(webRootPath, @"images\post");

                var extension = Path.GetExtension(file[0].FileName);

                //delete old images
                var objFromDB = await _unitOfWork.Post.GetByIdAsync(postVM.Post.Id);

                if (objFromDB.vehicleImage != null)
                {
                    var oldimagepath = Path.Combine(webRootPath, objFromDB.vehicleImage.Trim('\\'));

                    if (System.IO.File.Exists(oldimagepath))
                    {
                        System.IO.File.Delete(oldimagepath);
                    }
                }
                using (var fileStream = new FileStream(Path.Combine(upload, newFileName + extension), FileMode.Create))
                {
                    file[0].CopyTo(fileStream);
                }
                postVM.Post.vehicleImage = @"\images\post\" + newFileName + extension;
            }

            if (ModelState.IsValid)
            {
                var objFromDB = await _unitOfWork.Post.GetByIdAsync(postVM.Post.Id);

                await _unitOfWork.Post.Update(postVM.Post);
                await _unitOfWork.SaveAsync();

                TempData["Warning"] = CommonMessage.RecordUpdate;
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            Post post = await _unitOfWork.Post.GetByIdAsync(id);

            IEnumerable<SelectListItem> brandList = _unitOfWork.Brand.Query().Select(x => new SelectListItem
            {
                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()
            });

            IEnumerable<SelectListItem> vehicleTypeList = _unitOfWork.VehicleType.Query().Select(x => new SelectListItem
            {

                Text = x.Name.ToUpper(),
                Value = x.Id.ToString()

            });

            IEnumerable<SelectListItem> engineAndFuelList = Enum.GetValues(typeof(EngineAndFuelType))
                .Cast<EngineAndFuelType>()
                .Select(x => new SelectListItem
                {
                    Text = x.ToString().ToUpper(),
                    Value = ((int)(x)).ToString()
                });
            IEnumerable<SelectListItem> transmissionList = Enum.GetValues(typeof(Transmission))
                  .Cast<Transmission>()
                  .Select(x => new SelectListItem
                  {
                      Text = x.ToString().ToUpper(),
                      Value = ((int)(x)).ToString()
                  });

            PostVM postVM = new PostVM
            {
                Post = post,
                BrandList = brandList,
                VehicleTypeList = vehicleTypeList,
                EngineAndFuelList = engineAndFuelList,
                TransmissionList = transmissionList
            };
            return View(postVM);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(PostVM postVM)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;

            if (!string.IsNullOrEmpty(postVM.Post.vehicleImage))
            {
                var objFromDB = await _unitOfWork.Post.GetByIdAsync(postVM.Post.Id);

                if (objFromDB.vehicleImage != null)
                {
                    var oldimagepath = Path.Combine(webRootPath, objFromDB.vehicleImage.Trim('\\'));

                    if (System.IO.File.Exists(oldimagepath))
                    {
                        System.IO.File.Delete(oldimagepath);

                    }
                }
            }
            await _unitOfWork.Post.Delete(postVM.Post);
            await _unitOfWork.SaveAsync();

            TempData["error"] = CommonMessage.RecordDelete;
            return RedirectToAction("Index");
        }
    }
}
