using System.Linq;
using CRS.Business.Interfaces;
using CRS.Business.Models.Entities;
using CRS.Common.Caching;

namespace CRS.Business.Models.Caching
{
    public class PointConfigCollection : CacheCollection<PointConfig>
    {
        private IReferenceDataRepository _repository;

        public PointConfigCollection(IReferenceDataRepository repository)
        {
            _repository = repository;
        }

        #region Overrides of CacheCollection<PointConfig>

        public override void Execute()
        {
            var feedback = _repository.GetAllPointConfigs();
            IsPopulated = feedback.Success;
            if (feedback.Success)
            {
                Clear();
                AddRange(feedback.Data);
                InitShortcutProperties();
            }
        }

        #endregion

        public int StartingPoint { get; private set; }

        public int CreateNews { get; private set; }

        public int CreateTip { get; private set; }

        public int CreateRecipe { get; private set; }

        public int MaxPointPerDay { get; private set; }

        private void InitShortcutProperties()
        {
            StartingPoint = this.Single(i => i.Key == KeyObject.PointConfig.StartingPoint).Value;
            CreateNews = this.Single(i => i.Key == KeyObject.PointConfig.CreateNews).Value;
            CreateTip = this.Single(i => i.Key == KeyObject.PointConfig.CreateTip).Value;
            CreateRecipe = this.Single(i => i.Key == KeyObject.PointConfig.CreateRecipe).Value;
            MaxPointPerDay = this.Single(i => i.Key == KeyObject.PointConfig.MaxPointPerDay).Value;
        }
    }
}