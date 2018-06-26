using SecretSanta.Services.Contracts;
using SecretSanta.Web.Models;
using SecretSanta.Web.Models.Invitations;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace SecretSanta.Web.Controllers
{
    public class InvitationsController : ApiController
    {
        private readonly IUserService userService;
        private readonly IGroupService groupService;
        private readonly ISessionService sessionService;
        private readonly IInvitationService invitationService;

        public InvitationsController(IUserService userService, IGroupService groupService,
            ISessionService sessionService, IInvitationService invitationService)
        {
            this.userService = userService;
            this.groupService = groupService;
            this.sessionService = sessionService;
            this.invitationService = invitationService;
        }

        // POST ~/users/{username}/invitations
        [HttpPost]
        [Route("api/users/{username}/invitations")]
        public IHttpActionResult SendInvitation([FromUri] string username, InvitationViewModel model)
        {
            if (string.IsNullOrEmpty(username) || model == null || !ModelState.IsValid)
            {
                return this.BadRequest();
            }

            var group = this.groupService.GetGroupByName(model.GroupName);
            var receiver = this.userService.GetUserByUserName(username);
            if (group == null || receiver == null)
            {
                return this.NotFound();
            }

            var currentUser = this.sessionService.GetCurrentUser();
            if (currentUser.UserName != group.Admin.UserName)
            {
                return this.Content(HttpStatusCode.Forbidden, "Only admins can send invitations for groups.");
            }

            var existingInvitation = this.invitationService.GetByGroupAndUser(model.GroupName, username);
            if (existingInvitation != null)
            {
                return this.Content(HttpStatusCode.Conflict, "This user already has an invitation for this group.");
            }

            this.invitationService.CreateInvitation(group.Id, model.SentDate, receiver.Id);
            var invitationId = this.invitationService.GetByGroupAndUser(group.Name, receiver.UserName).Id;
            var invitationModel = new InvitationViewModel(invitationId, model.SentDate, group.Name, receiver.UserName);

            return this.Content(HttpStatusCode.Created, invitationModel);
        }

        // GET ~/users/{username}/invitations?skip={s}&take={t}&order={A|D}
        [HttpGet]
        [Route("api/users/{username}/invitations")]
        public IHttpActionResult GetUserInvitations(string username, [FromUri]ResultFormatViewModel model)
        {
            if (string.IsNullOrEmpty(username) || model == null)
            {
                return this.BadRequest();
            }

            var currentUser = this.sessionService.GetCurrentUser();
            if (currentUser.UserName != username)
            {
                return this.Content(HttpStatusCode.Forbidden, "Users are only allowed to access their invitations.");
            }

            var invitations = this.userService.GetUserInvitations(currentUser, model.Skip, model.Take, model.Order);

            var invitationsModel = invitations
                .Select(i => new InvitationViewModel(i.Id, i.SentDate, i.Group.Name, i.Receiver.UserName));

            return this.Ok(invitationsModel);
        }

        // POST ~/groups/{groupName}/participants
        [HttpPost]
        [Route("api/groups/{groupname}/participants")]
        public IHttpActionResult AcceptInvitation(string groupname)
        {
            var group = this.groupService.GetGroupByName(groupname);
            if (group == null)
            {
                return this.NotFound();
            }

            var currentUser = this.sessionService.GetCurrentUser();
            var invitation = currentUser.Invitations.Where(i => i.Group.Name == groupname).FirstOrDefault();
            if (invitation == null)
            {
                return this.Content(HttpStatusCode.Forbidden, "User has no invitations for this group.");
            }

            this.groupService.AddParticipant(group, currentUser);
            this.invitationService.DeleteInvitation(invitation);

            return this.Ok();
        }

        // DELETE ~/users/{username}/invitations/{id}
        [HttpDelete]
        [Route("api/users/{username}/invitations/{id}")]
        public IHttpActionResult DeleteInvitation(string username, string id)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(id))
            {
                return this.BadRequest();
            }

            var currentUser = this.sessionService.GetCurrentUser();
            if (currentUser.UserName != username)
            {
                return this.StatusCode(HttpStatusCode.Forbidden);
            }

            var invitation = this.invitationService.GetById(id);
            if (invitation == null || invitation.ReceiverId != currentUser.Id)
            {
                return this.Content(HttpStatusCode.Conflict, "Invitation does not exist.");
            }

            this.invitationService.DeleteInvitation(invitation);

            return this.StatusCode(HttpStatusCode.NoContent);
        }
    }
}
