using System.Linq;
using CRS.Business.Interfaces;
using CRS.Business.Models.Entities;
using CRS.Common.Caching;

namespace CRS.Business.Models.Caching
{
    public class TitleCollection : CacheCollection<Title>
    {
        private IReferenceDataRepository _repository;

        public TitleCollection(IReferenceDataRepository repository)
        {
            _repository = repository;
        }

        #region Overrides of CacheCollection<Title>

        public override void Execute()
        {
            var feedback = _repository.GetAllTitles();
            IsPopulated = feedback.Success;
            if (feedback.Success)
            {
                Clear();
                AddRange(feedback.Data);
                InitShortcutProperties();
            }
        }

        #endregion

        public int PhuBep { get; private set; }

        public int DauBepTapSu { get; private set; }

        public int DauBepChinhThuc { get; private set; }

        public int DauBepTruDanh { get; private set; }

        public int BepPho { get; private set; }

        public int BepTruong { get; private set; }

        public int SieuDauBep { get; private set; }

        public int VuaDauBep { get; private set; }

        private void InitShortcutProperties()
        {
            PhuBep = this.Single(i => i.Key == KeyObject.Title.PhuBep).Value;
            DauBepTapSu = this.Single(i => i.Key == KeyObject.Title.DauBepTapSu).Value;
            DauBepChinhThuc = this.Single(i => i.Key == KeyObject.Title.DauBepChinhThuc).Value;
            DauBepTruDanh = this.Single(i => i.Key == KeyObject.Title.DauBepTruDanh).Value;
            BepPho = this.Single(i => i.Key == KeyObject.Title.BepPho).Value;
            BepTruong = this.Single(i => i.Key == KeyObject.Title.BepTruong).Value;
            SieuDauBep = this.Single(i => i.Key == KeyObject.Title.SieuDauBep).Value;
            VuaDauBep = this.Single(i => i.Key == KeyObject.Title.VuaDauBep).Value;
        }
    }
}