using SecretSanta.Services.Contracts;
using SecretSanta.Web.Models.Groups;
using System.Web.Http;
using System.Net;
using System.Linq;
using SecretSanta.Web.Infrastructure.Factories;

namespace SecretSanta.Web.Controllers
{
    [RoutePrefix("api/groups")]
    public class GroupsController : ApiController
    {
        private readonly IGroupService groupService;
        private readonly ISessionService sessionService;
        private readonly IInvitationService invitationService;
        private readonly IDisplayGroupViewModelFactory viewModelsFactory;
        private readonly IDisplayUserViewModelFactory userViewModelFactory;

        public GroupsController(IGroupService groupService, ISessionService sessionService, IInvitationService invitationService,
            IDisplayGroupViewModelFactory viewModelsFactory, IDisplayUserViewModelFactory userViewModelFactory)
        {
            this.groupService = groupService;
            this.sessionService = sessionService;
            this.invitationService = invitationService;
            this.viewModelsFactory = viewModelsFactory;
            this.userViewModelFactory = userViewModelFactory;
        }

        // POST ~/groups 
        [HttpPost]
        [Route("")]
        public IHttpActionResult CreateGroup(CreateGroupViewModel model)
        {
            if(model == null)
            {
                return this.BadRequest("Group name must be provided");
            }

            if(!ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var currentUser = this.sessionService.GetCurrentUser();
            var existingGroup = this.groupService.GetGroupByName(model.Name);
            if(existingGroup != null)
            {
                return this.Content(HttpStatusCode.Conflict, "Group with that name already exists.");
            }

            var group = this.groupService.CreateGroup(model.Name, currentUser);

            var members = group.Users.Select(u => this.userViewModelFactory
                .CreateDisplayUserViewModel(u.Email, u.FirstName, u.LastName, u.DisplayName, u.UserName))
                .ToList();
            var groupModel = this.viewModelsFactory.CreateDisplayGroupViewModel(group.Name, group.Admin.UserName, members);

            return this.Content(HttpStatusCode.Created, groupModel);
        }

        // POST ~/groups/{groupName}/participants
        [HttpPost]
        [Route("{groupname}/participants")]
        public IHttpActionResult AcceptInvitation(string groupname)
        {
            var group = this.groupService.GetGroupByName(groupname);
            if(group == null)
            {
                return this.NotFound();
            }

            var currentUser = this.sessionService.GetCurrentUser();
            var invitation = currentUser.Invitations.Where(i => i.Group.Name == groupname).FirstOrDefault();
            if(invitation == null)
            {
                return this.Content(HttpStatusCode.Forbidden, "User has no invitations for this group.");
            }

            this.groupService.AddParticipant(group, currentUser);
            this.invitationService.DeleteInvitation(invitation);

            return this.Ok();
        }
    }
}
