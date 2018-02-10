using SecretSanta.Services.Contracts;
using SecretSanta.Web.Models.Groups;
using System.Web.Http;
using System.Net;
using System.Linq;
using SecretSanta.Web.Models.Users;

namespace SecretSanta.Web.Controllers
{
    [RoutePrefix("api/groups")]
    public class GroupsController : ApiController
    {
        private readonly IGroupService groupService;
        private readonly ISessionService sessionService;
        private readonly IUserService userService;
        private readonly IInvitationService invitationService;

        public GroupsController(IGroupService groupService, 
            ISessionService sessionService, 
            IUserService userService, 
            IInvitationService invitationService)
        {
            this.groupService = groupService;
            this.sessionService = sessionService;
            this.userService = userService;
            this.invitationService = invitationService;
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

            var members = group.Users.Select(u => new DisplayUserViewModel(u.Email, u.FirstName, u.LastName, u.DisplayName, u.UserName))
                .ToList();
            var groupModel = new DisplayGroupViewModel(group.Name, group.Admin.UserName, members);

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

        // GET ~/groups/{groupname}/participants
        [HttpGet]
        [Route("{groupname}/participants")]
        public IHttpActionResult GetGroupParticipants(string groupname)
        {
            if (groupname == null)
            {
                return this.BadRequest();
            }

            var group = this.groupService.GetGroupByName(groupname);
            if(group == null)
            {
                return this.Content(HttpStatusCode.NotFound, "Group with this name does not exist.");
            }

            var currentUser = this.sessionService.GetCurrentUser();
            if(group.Admin.UserName != currentUser.UserName)
            {
                return this.Content(HttpStatusCode.Forbidden, "This user is not admin of the group.");
            }

            var participants = group.Users;
            var modelParticipants = participants.Select(p => new DisplayUserViewModel(p.Email, p.FirstName, p.LastName, p.DisplayName, p.UserName));

            return this.Content(HttpStatusCode.OK, modelParticipants);
        }

        // DELETE ~/groups/{groupName}/participants/{participantUsername}
        [HttpDelete]
        [Route("{groupname}/participants/{participantUsername}")]
        public IHttpActionResult RemoveGroupParticipant(string groupname, string participantUsername)
        {
            if(string.IsNullOrEmpty(groupname) || string.IsNullOrEmpty(participantUsername))
            {
                return this.BadRequest();
            }

            var participant = this.userService.GetUserByUserName(participantUsername);
            if(participant == null)
            {
                return this.Content(HttpStatusCode.NotFound, "Participant does not exist.");
            }

            var group = this.groupService.GetGroupByName(groupname);
            if(group == null)
            {
                return this.Content(HttpStatusCode.NotFound, "Group with this name does not exist.");
            }

            var currentUser = this.sessionService.GetCurrentUser();
            if(currentUser.UserName != group.Admin.UserName)
            {
                return this.Content(HttpStatusCode.Forbidden, "Only administrators are allowed to remove participants from group.");
            }

            var isSuccess = this.groupService.RemoveParticipant(group, participant);
            if(!isSuccess)
            {
                return this.Content(HttpStatusCode.NotFound, "Participant does not exist in this group.");
            }

            return this.StatusCode(HttpStatusCode.NoContent);
        }
    }
}
