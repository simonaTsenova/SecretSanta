using SecretSanta.Services.Contracts;
using SecretSanta.Web.Models.Groups;
using System.Web.Http;
using System.Net;
using System.Linq;
using SecretSanta.Web.Models.Users;
using SecretSanta.Web.Models;
using SecretSanta.Authentication.Contracts;
using SecretSanta.Common.Exceptions;
using System;
using SecretSanta.Common;

namespace SecretSanta.Web.Controllers
{
    public class GroupsController : ApiController
    {
        private readonly IAuthenticationProvider authenticationProvider;
        private readonly IGroupService groupService;
        private readonly IUserService userService;

        public GroupsController(IAuthenticationProvider authenticationProvider,
            IGroupService groupService,
            IUserService userService)
        {
            this.authenticationProvider = authenticationProvider;
            this.groupService = groupService;
            this.userService = userService;
        }

        // POST ~/groups 
        [HttpPost]
        [Route("api/groups")]
        public IHttpActionResult CreateGroup(CreateGroupViewModel model)
        {
            if(model == null)
            {
                return this.BadRequest(Constants.GROUP_NAME_REQUIRED);
            }

            if(!ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            try
            {
                var currentUserId = this.authenticationProvider.CurrentUserId;
                var admin = this.userService.GetUserById(currentUserId);
                var group = this.groupService.CreateGroup(model.Name, admin);

                var members = group.Users.Select(u => new DisplayUserViewModel(u.Email, u.FirstName, u.LastName, u.DisplayName, u.UserName))
                    .ToList();
                var groupModel = new DisplayGroupViewModel(group.Name, group.Admin.UserName, members);

                return this.Content(HttpStatusCode.Created, groupModel);
            }
            catch (ItemNotFoundException notFoundException)
            {
                return Content(HttpStatusCode.NotFound, notFoundException.Message);
            }
            catch (ItemAlreadyExistingException alreadyExistingException)
            {
                return this.Content(HttpStatusCode.Conflict, alreadyExistingException.Message);
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.InternalServerError, ex.Message);
            }
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

            var currentUsername = this.authenticationProvider.CurrentUserName;
            if (currentUsername != username)
            {
                return this.Content(HttpStatusCode.Forbidden, Constants.GROUP_ACCESS_FORBIDDEN);
            }

            var groups = this.groupService.GetGroupsByUser(currentUsername, model.Skip, model.Take);
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

            try
            {
                var group = this.groupService.GetGroupByName(groupname);

                var currentUserId = this.authenticationProvider.CurrentUserId;
                if (group.Admin.Id != currentUserId)
                {
                    return this.Content(HttpStatusCode.Forbidden, Constants.GROUP_ACCESS_FORBIDDEN);
                }

                var participants = group.Users;
                var modelParticipants = participants.Select(p => new DisplayUserViewModel(p.Email, p.FirstName, p.LastName, p.DisplayName, p.UserName));

                return this.Content(HttpStatusCode.OK, modelParticipants);
            }
            catch (ItemNotFoundException notFoundException)
            {
                return Content(HttpStatusCode.NotFound, notFoundException.Message);
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.InternalServerError, ex.Message);
            }
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

            try
            {
                var group = this.groupService.GetGroupByName(groupname);

                var currentUserId = this.authenticationProvider.CurrentUserId;
                if (currentUserId != group.Admin.Id)
                {
                    return this.Content(HttpStatusCode.Forbidden, Constants.REMOVE_PARTICIPANT_FORBIDDEN);
                }

                var participant = this.userService.GetUserByUserName(participantUsername);
                this.groupService.RemoveParticipant(group, participant);

                return this.StatusCode(HttpStatusCode.NoContent);
            }
            catch (ItemNotFoundException notFoundException)
            {
                return this.Content(HttpStatusCode.NotFound, notFoundException.Message);
            }
            catch (Exception ex)
            {
                return this.Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
