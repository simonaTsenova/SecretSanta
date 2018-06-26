using SecretSanta.Authentication.Contracts;
using SecretSanta.Common;
using SecretSanta.Common.Exceptions;
using SecretSanta.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace SecretSanta.Web.Controllers
{
    public class LinksController : ApiController
    {
        private readonly IAuthenticationProvider authenticationProvider;
        private readonly IGroupService groupService;
        private readonly ILinkService linkService;

        public LinksController(IAuthenticationProvider authenticationProvider, IGroupService groupService, ILinkService linkService)
        {
            this.authenticationProvider = authenticationProvider;
            this.groupService = groupService;
            this.linkService = linkService;
        }

        // GET ~users/{username}/groups/{groupname}/links
        [HttpGet]
        [Route("api/users/{username}/groups/{groupname}/links")]
        public IHttpActionResult GetUserGroupLinks(string username, string groupname)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(groupname))
            {
                return this.BadRequest();
            }

            try
            {
                var currentUsername = this.authenticationProvider.CurrentUserName;
                if (currentUsername != username)
                {
                    return this.Content(HttpStatusCode.Forbidden, Constants.LINKS_ACCESS_FORBIDDEN);
                }

                var group = this.groupService.GetGroupByName(groupname);
                if (!group.hasLinkingProcessStarted)
                {
                    return this.Content(HttpStatusCode.NotFound, Constants.LINKING_PROCESS_NOT_STARTED);
                }

                var link = this.linkService.GetByGroupAndSender(groupname, username);
                var model = new { receiver = link.Receiver.UserName };

                return this.Ok(model);
            }
            catch(ItemNotFoundException notFoundException)
            {
                return this.Content(HttpStatusCode.NotFound, notFoundException.Message);
            }
            catch(Exception ex)
            {
                return this.Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        // POST ~/groups/{groupname}/links
        [HttpPost]
        [Route("api/groups/{groupname}/links")]
        public IHttpActionResult StartLinkingProcess(string groupname)
        {
            if (string.IsNullOrEmpty(groupname))
            {
                return this.BadRequest();
            }

            try
            {
                var group = this.groupService.GetGroupByName(groupname);
                if (group.hasLinkingProcessStarted)
                {
                    return this.Content(HttpStatusCode.PreconditionFailed, Constants.LINKING_PROCESS_ALREADY_DONE);
                }

                if (group.Users.Count < 2 || group.Users == null)
                {
                    return this.Content(HttpStatusCode.PreconditionFailed, Constants.LINKING_PROCESS_MEMBERS_MINIMUM_COUNT);
                }

                if (group.Users.Count % 2 != 0)
                {
                    return this.Content(HttpStatusCode.PreconditionFailed, Constants.LINKING_PROCESS_MEMBERS_ODD_COUNT);
                }

                var currentUserId = this.authenticationProvider.CurrentUserId;
                if (currentUserId != group.Admin.Id)
                {
                    return this.Content(HttpStatusCode.Forbidden, Constants.LINKING_PROCESS_START_FORBIDDEN);
                }

                this.linkService.CreateLinks(group);
                this.groupService.MakeProcessStarted(group);

                return this.Content(HttpStatusCode.Created, Constants.LINKING_PROCESS_COMPLETE_SUCCESS);
            }
            catch(ItemNotFoundException notFoundException)
            {
                return Content(HttpStatusCode.NotFound, notFoundException.Message);
            }
            catch(Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
