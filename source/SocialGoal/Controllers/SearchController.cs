using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SocialGoal.Mappings;
using SocialGoal.Web.Core.Extensions;
using SocialGoal.Model.Models;
using SocialGoal.Service;
using SocialGoal.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SocialGoal.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly IGoalService goalService;
        private readonly IUserService userService;
        private readonly IGroupService groupService;

        public SearchController(IGoalService goalService, IUserService userService, IGroupService groupService)
        {
            this.goalService = goalService;
            this.userService = userService;
            this.groupService = groupService;
        }

        public ViewResult SearchAll(string searchText)
        {
            SearchViewModel searchViewModel = new SearchViewModel()
            {
                Goals = Mapper.Map<IEnumerable<Goal>, IEnumerable<GoalViewModel>>(goalService.SearchGoal(searchText)),
                Users = userService.SearchUser(searchText),
                Groups = Mapper.Map<IEnumerable<Group>, IEnumerable<GroupViewModel>>(groupService.SearchGroup(searchText)),
                SearchText = searchText
            };
            ViewBag.searchtext = searchText;
            return View("SearchResult", searchViewModel);
        }

    }
}