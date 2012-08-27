using System;
using System.Collections.Generic;
using System.Linq;
using CRS.Business.Feedbacks;
using CRS.Business.Interfaces;
using CRS.Business.Models.Entities;
using CRS.Common.Logging;
using CRS.Resources;

namespace CRS.Business.Repositories
{
    public class ReferenceDataRepository : IReferenceDataRepository
    {
        #region Implementation of IReferenceDataRepository

        public Feedback<IList<PointConfig>> GetAllPointConfigs()
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var pointConfigs = entities.PointConfigs.ToList();
                    return new Feedback<IList<PointConfig>>(true, null, pointConfigs);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<PointConfig>>(false, Messages.GeneralError);
            }
        }

        public Feedback<IList<Title>> GetAllTitles()
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var titles = entities.Titles.ToList();
                    return new Feedback<IList<Title>>(true, null, titles);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<Title>>(false, Messages.GeneralError);
            }
        }

        public Feedback<IList<Location>> GetAllLocations()
        {
            try
            {
                using (var entities = new CrsEntities())
                {
                    var locations = entities.Locations.ToList();
                    return new Feedback<IList<Location>>(true, null, locations);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return new Feedback<IList<Location>>(false, Messages.GeneralError);
            }
        }

        #endregion
    }
}