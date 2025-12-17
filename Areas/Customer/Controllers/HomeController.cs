using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using TopSpeed.Application.Contracts.Presistence;
using TopSpeed.Domain.Models;
using TopSpeed.Domain.ViewModel;
using TopSpeed.Application.ExtensionsMethods;
using Microsoft.AspNetCore.WebUtilities;

namespace TopSpeed.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;   

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? page,bool resetFilter = false)
        {
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

            // List<Post> posts = await  _unitOfWork.Post.GetAllPost();
            List<Post> posts;

            if (resetFilter)
            {
                TempData.Remove("FilteredPosts");
                TempData.Remove("SelectedBrandId");
                TempData.Remove("SelectedVehicleTypeId");
            }

            if (TempData.ContainsKey("FilteredPosts"))
            {
                posts = TempData.Get<List<Post>>("FilteredPosts");
                TempData.Keep("FilteredPosts");
            }
            else
            {
                posts = await _unitOfWork.Post.GetAllPost();
            }

            int pageSize = 2;

            int pageNumber = page ?? 1;

            int totalItem = posts.Count;    

            int totalPage = (int)Math.Ceiling((double)totalItem / pageSize);

            ViewBag.TotalPages = totalPage;
            ViewBag.Currentpage = pageNumber;
            

            var pagedPosts = posts.Skip((pageNumber-1) * pageSize).Take(pageSize).ToList();

            HttpContext.Session.SetString("PreviousUrl", HttpContext.Request.Path);

            HomePostVM homePostVM = new HomePostVM
            {
                Posts = pagedPosts,
                BrandList = brandList,
                VehicleTypeList = vehicleTypeList,
                BrandID = (Guid?)TempData["SelectedBrandId"],
                VehicleTypeID = (Guid?)TempData["SelectedVehicleTypeId"]
            };
            return View(homePostVM);
        }

        [HttpPost]
        public async Task<IActionResult> Index(HomePostVM homePostVM)
        {
            var posts = await _unitOfWork.Post.GetAllPost(homePostVM.searchBox, homePostVM.BrandID, homePostVM.VehicleTypeID);

            TempData.Put("FilteredPosts", posts);
            TempData["SelectedBrandId"] = homePostVM.BrandID;
            TempData["SelectedVehicleTypeId"] = homePostVM.VehicleTypeID;

            return RedirectToAction("Index", new { page = 1, resetFilter = false });
        }
        public async Task<IActionResult> Details(Guid id,int? page)
        {
            Post post = await _unitOfWork.Post.GetByIdAsync(id);

            //var post = await _unitOfWork.Post.GetByIdAsync(id);


            List<Post> posts = new List<Post>();

            if(post != null) 
            {
                posts = await _unitOfWork.Post.GetAllPost(post.Id, post.BrandID);
            
            }

            ViewBag.CurrentPage = page;

            CustomerDetailsVM customerDetailsVM = new CustomerDetailsVM
            {
                Post = post,
                Posts= posts
            };
            return View(customerDetailsVM);
        }

        public ActionResult GoBack(int? page)
        {
            string? previousUrl = HttpContext.Session.GetString("PreviousUrl");

            if(!string.IsNullOrEmpty(previousUrl))
            {
                //Append the page number to the previous URL if is exits
                if(page.HasValue)
                {
                    previousUrl = QueryHelpers.AddQueryString(previousUrl,"Page",page.Value.ToString());
                }

                HttpContext.Session.Remove(previousUrl); //Remove the session Variable

                return Redirect(previousUrl); 
            }
            else
            {
                return RedirectToAction("Index");
            }
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
