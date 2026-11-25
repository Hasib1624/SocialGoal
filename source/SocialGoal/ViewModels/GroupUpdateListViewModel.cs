using System;
using System.Collections.Generic;
using SocialGoal.Model.Models;
using System.Linq;

namespace SocialGoal.Web.ViewModels
{
    public class GroupUpdateListViewModel
    {
        public IEnumerable<GroupUpdateViewModel> GroupUpdates { get; set; }

        public double? Target { get; set; }

        public Metric Metric { get; set; }
    }
}