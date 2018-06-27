using SecretSanta.Authentication.Contracts;
using SecretSanta.Common;
using SecretSanta.Common.Exceptions;
using SecretSanta.Services.Contracts;
using SecretSanta.Web.Mapper;
using SecretSanta.Web.Models;
using SecretSanta.Web.Models.Invitations;
using System;
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
                var currentUsername = this.authenticationProvider.CurrentUserName;
                this.invitationService.CheckUserAcccessRights(currentUsername, group.Admin.UserName);

                var receiver = this.userService.GetUserByUserName(username);

                this.invitationService.CreateInvitation(group.Id, model.SentDate, receiver.Id);
                var invitation = this.invitationService.GetByGroupAndUser(group.Id, receiver.Id);

                var invitationModel = this.mapper.MapInvitation(invitation);

                return this.Content(HttpStatusCode.Created, invitationModel);
            }
            catch (ItemNotFoundException notFoundException)
            {
                return Content(HttpStatusCode.NotFound, notFoundException.Message);
            }
            catch (AccessForbiddenException accessForbiddenException)
            {
                return this.Content(HttpStatusCode.Forbidden, accessForbiddenException.Message);
            }
            catch (ItemAlreadyExistingException alreadyExistingException)
            {
                return Content(HttpStatusCode.Conflict, alreadyExistingException.Message);
            }
            catch (Exception ex)
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

            try
            {
                var currentUsername = this.authenticationProvider.CurrentUserName;
                this.invitationService.CheckUserAcccessRights(currentUsername, username);

                var invitations = this.invitationService.GetByUser(currentUsername, model.Skip, model.Take, model.Order);
                var invitationsModel = this.mapper.MapInvitations(invitations);

                return this.Ok(invitationsModel);
            }
            catch (AccessForbiddenException accessForbiddenException)
            {
                return this.Content(HttpStatusCode.Forbidden, accessForbiddenException.Message);
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // POST ~/groups/{groupName}/invitations
        [HttpPost]
        [Route("api/groups/{groupname}/invitations")]
        public IHttpActionResult AcceptInvitation(string groupname)
        {
            if (string.IsNullOrEmpty(groupname))
            {
                return this.BadRequest(Constants.INVALID_MODEL);
            }

            try
            {
                var group = this.groupService.GetGroupByName(groupname);
                var currentUserId = this.authenticationProvider.CurrentUserId;
                var invitation = this.invitationService.GetUserInvitation(group.Id, currentUserId);

                this.groupService.AddParticipant(group, invitation.Receiver);
                this.invitationService.DeleteInvitation(invitation);

                return this.Ok(Constants.INVITATION_ACCEPT_SUCCESS);
            }
            catch (ItemNotFoundException notFoundException)
            {
                return Content(HttpStatusCode.NotFound, notFoundException.Message);
            }
            catch (ItemAlreadyExistingException alreadyExistingException)
            {
                return Content(HttpStatusCode.Conflict, alreadyExistingException.Message);
            }
            catch (AccessForbiddenException forbiddenException)
            {
                return this.Content(HttpStatusCode.Forbidden, forbiddenException.Message);
            }
            catch (Exception ex)
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

            try
            {
                var currentUsername = this.authenticationProvider.CurrentUserName;
                this.invitationService.CheckUserAcccessRights(currentUsername, username);

                var invitation = this.invitationService.GetById(id);
                this.invitationService.CheckUserAcccessRights(currentUsername, invitation.Receiver.UserName);

                this.invitationService.DeleteInvitation(invitation);

                return this.StatusCode(HttpStatusCode.NoContent);
            }
            catch (ItemNotFoundException itemNotFoundException)
            {
                return this.Content(HttpStatusCode.NotFound, itemNotFoundException.Message);
            }
            catch (AccessForbiddenException accessForbiddenException)
            {
                return this.Content(HttpStatusCode.Forbidden, accessForbiddenException.Message);
            }
            catch (Exception exception)
            {
                return this.Content(HttpStatusCode.InternalServerError, exception.Message);
            }
        }
    }
}
