using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopSpeed.Domain.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TopSpeed.Domain.ViewModel
{
    public class PostVM
    {
        // mvcviewFeatures
        public Post Post {  get; set; } 

        public IEnumerable<SelectListItem> BrandList { get; set; }

        public IEnumerable<SelectListItem> VehicleTypeList { get; set; }

        public IEnumerable<SelectListItem> EngineAndFuelList { get; set; }

        public IEnumerable<SelectListItem> TransmissionList { get; set; }
    }
}
