using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopSpeed.Application.ApplicationConstants
{
    public class ApplicationConstant
    {
    }

    public static class CommonMessage
    {
        public static string RecordCreate = "Records save successfully";
        public static string RecordUpdate = "Records Update successfully";
        public static string RecordDelete = "Record Delete successfully";
    }

    public static class CustomRole
    {
        public const string MasterAdmin = "MASTERADMIN";
        public const string Admin = "ADMIN";
        public const string Customer = "CUSTOMER";
    }
}
