#nullable enable
using System.Collections.Generic;
using System.Linq;
using Riok.Mapperly.Abstractions;
using SocialGoal.Model.Models;
using SocialGoal.Web.ViewModels;
using X.PagedList;

namespace SocialGoal.Mappings
{
    /// <summary>
    /// Mapperly (compile-time) replacement for the AutoMapper profiles. Call as Mapper.Map&lt;TSource, TTarget&gt;(source);
    /// every pair must be declared below.
    /// </summary>
    [Mapper(ThrowOnPropertyMappingNullMismatch = false, RequiredMappingStrategy = RequiredMappingStrategy.None)]
    public static partial class Mapper
    {
        public static partial TTarget Map<TSource, TTarget>(TSource source);

        // domain -> view model
        private static partial GoalViewModel ToView(Goal source);
        private static partial GoalFormModel ToForm(Goal source);
        private static partial CommentsViewModel ToView(Comment source);
        private static partial UserProfileFormModel ToForm(UserProfile source);
        private static partial GroupFormModel ToForm(Group source);
        private static partial GroupGoalFormModel ToForm(GroupGoal source);
        private static partial NotificationViewModel ToView(GroupInvitation source);
        private static partial NotificationViewModel ToView(SupportInvitation source);
        private static partial NotificationViewModel ToView(FollowRequest source);
        private static partial GroupViewModel ToView(Group source);
        private static partial GroupGoalViewModel ToView(GroupGoal source);
        private static partial GroupCommentsViewModel ToView(GroupComment source);
        private static partial FocusViewModel ToView(Focus source);
        private static partial FocusFormModel ToForm(Focus source);
        private static partial GroupRequestViewModel ToView(GroupRequest source);
        private static partial FollowersViewModel ToFollower(ApplicationUser source);
        private static partial FollowingViewModel ToFollowing(ApplicationUser source);
        private static partial UpdateFormModel ToForm(Update source);
        private static partial GroupUpdateFormModel ToForm(GroupUpdate source);
        private static partial UpdateViewModel ToView(Update source);
        private static partial GroupUpdateViewModel ToView(GroupUpdate source);

        private static GoalListViewModel ToListItem(Goal source)
        {
            var target = ToListItemBase(source);
            target.SupportsCount = source.Supports?.Count ?? 0;
            target.UserName = source.User?.UserName;
            target.StartDate = source.StartDate.ToString("dd MMM yyyy");
            target.EndDate = source.EndDate.ToString("dd MMM yyyy");
            return target;
        }

        [MapperIgnoreTarget(nameof(GoalListViewModel.SupportsCount))]
        [MapperIgnoreTarget(nameof(GoalListViewModel.UserName))]
        [MapperIgnoreTarget(nameof(GoalListViewModel.StartDate))]
        [MapperIgnoreTarget(nameof(GoalListViewModel.EndDate))]
        private static partial GoalListViewModel ToListItemBase(Goal source);

        private static GroupsItemViewModel ToItem(Group source)
        {
            var target = ToItemBase(source);
            target.CreatedDate = source.CreatedDate.ToString("dd MMM yyyy");
            return target;
        }

        [MapperIgnoreTarget(nameof(GroupsItemViewModel.CreatedDate))]
        private static partial GroupsItemViewModel ToItemBase(Group source);

        private static IPagedList<GroupsItemViewModel> ToPage(IPagedList<Group> source) =>
            new StaticPagedList<GroupsItemViewModel>(source.Select(ToItem).ToList(), source.PageNumber, source.PageSize, source.TotalItemCount);

        // collections are materialized so the views see one stable set of objects
        private static IEnumerable<CommentsViewModel> ToViews(IEnumerable<Comment> source) => source.Select(ToView).ToList();
        private static IEnumerable<GoalViewModel> ToViews(IEnumerable<Goal> source) => source.Select(ToView).ToList();
        private static IEnumerable<GoalListViewModel> ToListItems(IEnumerable<Goal> source) => source.Select(ToListItem).ToList();
        private static IEnumerable<GroupViewModel> ToViews(IEnumerable<Group> source) => source.Select(ToView).ToList();
        private static IEnumerable<GroupsItemViewModel> ToItems(IEnumerable<Group> source) => source.Select(ToItem).ToList();
        private static IEnumerable<GroupGoalViewModel> ToViews(IEnumerable<GroupGoal> source) => source.Select(ToView).ToList();
        private static IEnumerable<GroupCommentsViewModel> ToViews(IEnumerable<GroupComment> source) => source.Select(ToView).ToList();
        private static IEnumerable<FocusViewModel> ToViews(IEnumerable<Focus> source) => source.Select(ToView).ToList();
        private static IEnumerable<GroupRequestViewModel> ToViews(IEnumerable<GroupRequest> source) => source.Select(ToView).ToList();
        private static IEnumerable<NotificationViewModel> ToViews(IEnumerable<GroupInvitation> source) => source.Select(ToView).ToList();
        private static IEnumerable<NotificationViewModel> ToViews(IEnumerable<SupportInvitation> source) => source.Select(ToView).ToList();
        private static IEnumerable<NotificationViewModel> ToViews(IEnumerable<FollowRequest> source) => source.Select(ToView).ToList();
        private static IEnumerable<FollowersViewModel> ToFollowers(IEnumerable<ApplicationUser> source) => source.Select(u => u == null ? null! : ToFollower(u)).ToList();
        private static IEnumerable<FollowingViewModel> ToFollowings(IEnumerable<ApplicationUser> source) => source.Select(u => u == null ? null! : ToFollowing(u)).ToList();
        private static IEnumerable<UpdateViewModel> ToViews(IEnumerable<Update> source) => source.Select(ToView).ToList();
        private static IEnumerable<GroupUpdateViewModel> ToViews(IEnumerable<GroupUpdate> source) => source.Select(ToView).ToList();

        // view model -> domain
        private static partial Comment ToDomain(CommentFormModel source);
        private static partial Group ToDomain(GroupFormModel source);
        private static partial Focus ToDomain(FocusFormModel source);
        private static partial Update ToDomain(UpdateFormModel source);
        private static partial ApplicationUser ToDomain(UserFormModel source);
        private static partial UserProfile ToDomain(UserProfileFormModel source);
        private static partial GroupGoal ToDomain(GroupGoalFormModel source);
        private static partial GroupUpdate ToDomain(GroupUpdateFormModel source);
        private static partial GroupComment ToDomain(GroupCommentFormModel source);
        private static partial GroupRequest ToDomain(GroupRequestFormModel source);
        private static partial FollowRequest ToDomain(FollowRequestFormModel source);
        private static partial Goal ToDomain(GoalFormModel source);
    }
}
