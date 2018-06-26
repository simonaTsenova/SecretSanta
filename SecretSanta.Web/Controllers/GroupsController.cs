using SecretSanta.Services.Contracts;
using SecretSanta.Web.Models.Groups;
using System.Web.Http;
using System.Net;
using System.Linq;
using SecretSanta.Web.Models.Users;
using SecretSanta.Web.Models;

namespace SecretSanta.Web.Controllers
{
    public class GroupsController : ApiController
    {
        private readonly IGroupService groupService;
        private readonly ISessionService sessionService;
        private readonly IUserService userService;

        public GroupsController(IGroupService groupService, 
            ISessionService sessionService, 
            IUserService userService)
        {
            this.groupService = groupService;
            this.sessionService = sessionService;
            this.userService = userService;
        }

        // POST ~/groups 
        [HttpPost]
        [Route("api/groups")]
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

        // GET ~/users/{username}/groups?skip={s}&take={t}
        [HttpGet]
        [Route("api/users/{username}/groups")]
        public IHttpActionResult GetUserGroups(string username, [FromUri]PagingViewModel model)
        {
            if (string.IsNullOrEmpty(username) || model == null)
            {
                return this.BadRequest();
            }

            var currentUser = this.sessionService.GetCurrentUser();
            if (currentUser.UserName != username)
            {
                return this.Content(HttpStatusCode.Forbidden, "Users are only allowed to see their groups.");
            }

            var groups = this.userService.GetUserGroups(currentUser, model.Skip, model.Take);
            var modelGroups = groups.Select(g => new ShortGroupViewModel(g.Name, g.Admin.UserName));

            return this.Ok(modelGroups);
        }

        // GET ~/groups/{groupname}/participants
        [HttpGet]
        [Route("api/groups/{groupname}/participants")]
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
        [Route("api/groups/{groupname}/participants/{participantUsername}")]
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
