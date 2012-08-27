using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRS.Business.Models
{
    public class KeyObject
    {
        public const int UserState_Active = 1;
        public const int UserState_Banned = 2;

        public static class PointConfig
        {
            public const string StartingPoint = "StartingPoint";
            public const string CreateNews = "CreateNews";
            public const string CreateTip = "CreateTip";
            public const string CreateRecipe = "CreateRecipe";
            public const string MaxPointPerDay = "MaxPointPerDay";
        }

        public static class Title
        {
            public const string PhuBep = "PhuBep";
            public const string DauBepTapSu = "DauBepTapSu";
            public const string DauBepChinhThuc = "DauBepChinhThuc";
            public const string DauBepTruDanh = "DauBepTruDanh";
            public const string BepPho = "BepPho";
            public const string BepTruong = "BepTruong";
            public const string SieuDauBep = "SieuDauBep";
            public const string VuaDauBep = "VuaDauBep";

            public const string PhuBepLevel = "Phụ bếp";
            public const string DauBepTapSuLevel = "Đầu bếp tập sự";
            public const string DauBepChinhThucLevel = "Đầu bếp chính thức";
            public const string DauBepTruDanhLevel = "Đầu bếp trứ danh";
            public const string BepPhoLevel = "Bếp phó";
            public const string BepTruongLevel = "Bếp trưởng";
            public const string SieuDauBepLevel = "Siêu đầu bếp";
            public const string VuaDauBepLevel = "Vua đầu bếp";
        }

        public static class Permission
        {
            public const string ManageDictionary = "Manage Dictionary";
            public const string ViewAdminPages = "View admin pages";
            public const string ManageContents = "Manage Contents";
            public const string ManageSpam = "Manage Spam";
            public const string ManageUsers = "Manage Users";
            public const string ManageCategories = "Manage Categories";
            public const string ManageApproval = "Manage Approval";
            public const string ViewStatistics = "View statistics";
            public const string EditContents = "Edit Contents";
        }
    }
}