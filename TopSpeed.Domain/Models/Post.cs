using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopSpeed.Domain.ApplicationEnum;
using TopSpeed.Domain.Common;

namespace TopSpeed.Domain.Models
{
    public class Post :BaseModel
    {
        [Display(Name = "Brand")]
         public Guid BrandID { get; set; }

         [ValidateNever] //install package Microsoft.AspNetCore.MVC.Core
       //  [ForeignKey(nameof(BrandID))]
        [ForeignKey("BrandID")]
         public Brand Brand { get; set; }

        [Display(Name = "VehicleType")]
        public Guid VehicleTypeID { get; set; }

        [ValidateNever]
       // [ForeignKey(nameof(VehicleTypeID))]

        [ForeignKey("VehicleTypeID")]
        public VehicleType VehicleType { get; set; }   
        
        public string Name { get; set; }

        [Display(Name ="Select Engine Name /Fuel Name")]
        public EngineAndFuelType EngineAndFuelType {  get; set; }

        [Display(Name="Select Transmission Mode")]
        public Transmission Transmission { get; set; }  

        public int Engine { get; set; } 

        public int TopSpeed { get; set; }   

        public int Mileage { get; set; }    

        public int Range { get; set; }

        [Display(Name="Seating Capacity")]
        public string SeatingCapacity { get; set; }

        [Display(Name="Base Price")]
        public double PriceFrom {  get; set; }

        [Display(Name="Top-End Price")]
        public double PriceTo { get; set; }

        [Range(1,5,ErrorMessage ="Rating should be From 1 to 5 only")]
        public int Ratings { get; set; }

        [Display(Name ="Upload Vehicle Image")]
        public string vehicleImage {  get; set; }   

    }
}
