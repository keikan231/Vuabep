using CRS.Business.Models;
using CRS.Business.Models.Caching;

namespace CRS.Business.LevelManagement
{
    public class LevelHandler
    {
        public static string CurrentLevel(int point)
        {
            string level;
            var phuBep = ReferenceDataCache.TitleCollection.PhuBep;
            int dauBepTapSu = ReferenceDataCache.TitleCollection.DauBepTapSu;
            int dauBepChinhThuc = ReferenceDataCache.TitleCollection.DauBepChinhThuc;
            int dauBepTruDanh = ReferenceDataCache.TitleCollection.DauBepTruDanh;
            int bepPho = ReferenceDataCache.TitleCollection.BepPho;
            int bepTruong = ReferenceDataCache.TitleCollection.BepTruong;
            int sieuDauBep = ReferenceDataCache.TitleCollection.SieuDauBep;
            int vuaDauBep = ReferenceDataCache.TitleCollection.VuaDauBep;
            if (point >= phuBep && point < dauBepTapSu)
            {
                level = KeyObject.Title.PhuBepLevel;
            }
            else if (point >= dauBepTapSu && point < dauBepChinhThuc)
            {
                level = KeyObject.Title.DauBepTapSuLevel;
            }
            else if (point >= dauBepChinhThuc && point < dauBepTruDanh)
            {
                level = KeyObject.Title.DauBepChinhThucLevel;
            }
            else if (point >= dauBepTruDanh && point < bepPho)
            {
                level = KeyObject.Title.DauBepTruDanhLevel;
            }
            else if (point >= bepPho && point < bepTruong)
            {
                level = KeyObject.Title.BepPhoLevel;
            }
            else if (point >= bepTruong && point < sieuDauBep)
            {
                level = KeyObject.Title.BepTruongLevel;
            }
            else if (point >= sieuDauBep && point < vuaDauBep)
            {
                level = KeyObject.Title.SieuDauBepLevel;
            }
            else
            {
                level = KeyObject.Title.VuaDauBepLevel;
            }

            return level;
        }
    }
}