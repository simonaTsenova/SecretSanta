using SecretSanta.Authentication.Contracts;
using SecretSanta.Common;
using SecretSanta.Common.Exceptions;
using SecretSanta.Services.Contracts;
using SecretSanta.Web.Mapper;
using SecretSanta.Web.Models;
using SecretSanta.Web.Models.Invitations;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace SecretSanta.Web.Controllers
{
    public class InvitationsController : ApiController
    {
        private readonly IAuthenticationProvider authenticationProvider;
        private readonly IUserService userService;
        private readonly IGroupService groupService;
        private readonly IInvitationService invitationService;
        private readonly IMapper mapper;

        public InvitationsController(IAuthenticationProvider authenticationProvider,
            IUserService userService,
            IGroupService groupService,
            IInvitationService invitationService,
            IMapper mapper)
        {
            this.authenticationProvider = authenticationProvider;
            this.userService = userService;
            this.groupService = groupService;
            this.invitationService = invitationService;
            this.mapper = mapper;
        }

        // POST ~/users/{username}/invitations
        [HttpPost]
        [Route("api/users/{username}/invitations")]
        public IHttpActionResult SendInvitation([FromUri] string username, InvitationViewModel model)
        {
            if (string.IsNullOrEmpty(username) || model == null || !ModelState.IsValid)
            {
                return this.BadRequest(Constants.INVALID_MODEL);
            }

            try
            {
                var group = this.groupService.GetGroupByName(model.GroupName);
                var receiver = this.userService.GetUserByUserName(username);

                var currentUserId = this.authenticationProvider.CurrentUserId;
                if (currentUserId != group.Admin.Id)
                {
                    return this.Content(HttpStatusCode.Forbidden, Constants.SEND_INVITATION_FORBIDDEN);
                }

                this.invitationService.CreateInvitation(group.Id, model.SentDate, receiver.Id);
                var invitation = this.invitationService.GetByGroupAndUser(group.Id, receiver.Id);

                var invitationModel = this.mapper.MapInvitation(invitation);

                return this.Content(HttpStatusCode.Created, invitationModel);
            }
            catch(ItemNotFoundException notFoundException)
            {
                return Content(HttpStatusCode.NotFound, notFoundException.Message);
            }
            catch(ItemAlreadyExistingException alreadyExistingException)
            {
                return Content(HttpStatusCode.Conflict, alreadyExistingException.Message);
            }
            catch(Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // GET ~/users/{username}/invitations?skip={s}&take={t}&order={A|D}
        [HttpGet]
        [Route("api/users/{username}/invitations")]
        public IHttpActionResult GetUserInvitations(string username, [FromUri]ResultFormatViewModel model)
        {
            if (string.IsNullOrEmpty(username) || model == null)
            {
                return this.BadRequest(Constants.INVALID_MODEL);
            }

            var currentUsername = this.authenticationProvider.CurrentUserName;
            if (currentUsername != username)
            {
                return this.Content(HttpStatusCode.Forbidden, Constants.INVITATION_ACCESS_FORBIDDEN);
            }

            var invitations = this.invitationService.GetByUser(currentUsername, model.Skip, model.Take, model.Order);

            var invitationsModel = this.mapper.MapInvitations(invitations);

            return this.Ok(invitationsModel);
        }

        // POST ~/groups/{groupName}/participants
        [HttpPost]
        [Route("api/groups/{groupname}/participants")]
        public IHttpActionResult AcceptInvitation(string groupname)
        {
            if (string.IsNullOrEmpty(groupname))
            {
                return this.BadRequest(Constants.INVALID_MODEL);
            }

            try
            {
                var currentUserId = this.authenticationProvider.CurrentUserId;
                var currentUser = this.userService.GetUserById(currentUserId);
                var invitation = currentUser.Invitations.Where(i => i.Group.Name == groupname).FirstOrDefault();
                if (invitation == null)
                {
                    return this.Content(HttpStatusCode.Forbidden, Constants.USER_HAS_NO_INVITATIONS_FOR_GROUP);
                }

                var group = this.groupService.GetGroupByName(groupname);

                this.groupService.AddParticipant(group, currentUser);
                this.invitationService.DeleteInvitation(invitation);

                return this.Ok(Constants.INVITATION_ACCEPT_SUCCESS);
            }
            catch(ItemNotFoundException notFoundException)
            {
                return Content(HttpStatusCode.NotFound, notFoundException.Message);
            }
            catch(ItemAlreadyExistingException alreadyExistingException)
            {
                return Content(HttpStatusCode.Conflict, alreadyExistingException.Message);
            }
            catch(Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // DELETE ~/users/{username}/invitations/{id}
        [HttpDelete]
        [Route("api/users/{username}/invitations/{id}")]
        public IHttpActionResult DeleteInvitation(string username, string id)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(id))
            {
                return this.BadRequest(Constants.INVALID_MODEL);
            }

            var currentUsername = this.authenticationProvider.CurrentUserName;
            if (currentUsername != username)
            {
                return this.Content(HttpStatusCode.Forbidden, Constants.INVITATION_ACCESS_FORBIDDEN);
            }

            var invitation = this.invitationService.GetById(id);
            if (invitation.Receiver.UserName != currentUsername)
            {
                return this.Content(HttpStatusCode.Conflict, Constants.INVITATION_ACCESS_FORBIDDEN);
            }

            this.invitationService.DeleteInvitation(invitation);

            return this.StatusCode(HttpStatusCode.NoContent);
        }
    }
}
