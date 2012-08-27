using System.Collections.Generic;
using CRS.Business.Feedbacks;
using CRS.Business.Models.Entities;

namespace CRS.Business.Interfaces
{
    public interface IReferenceDataRepository
    {
        Feedback<IList<PointConfig>> GetAllPointConfigs();
        Feedback<IList<Title>> GetAllTitles();
        Feedback<IList<Location>> GetAllLocations();
    }
}