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
        private readonly ISessionService sessionService;
        private readonly IGroupService groupService;
        private readonly ILinkService linkService;

        public LinksController(ISessionService sessionService, IGroupService groupService, ILinkService linkService)
        {
            this.sessionService = sessionService;
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

            var currentUser = this.sessionService.GetCurrentUser();
            if (currentUser.UserName != username)
            {
                return this.Content(HttpStatusCode.Forbidden, "Users are only allowed to see their links.");
            }

            var group = this.groupService.GetGroupByName(groupname);
            if (group == null)
            {
                return this.Content(HttpStatusCode.NotFound, "Group with this name does not exist.");
            }

            if (!group.hasLinkingProcessStarted)
            {
                return this.Content(HttpStatusCode.NotFound, "Linking process has not started yet.");
            }

            var connection = this.linkService.GetByGroupAndSender(groupname, username);
            var model = new { receiver = connection.Receiver.UserName };

            return this.Ok(model);
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

            var group = this.groupService.GetGroupByName(groupname);
            if (group == null)
            {
                return this.NotFound();
            }

            if (group.hasLinkingProcessStarted)
            {
                return this.Content(HttpStatusCode.PreconditionFailed, "Linking process can be started only once.");
            }

            if (group.Users.Count < 2 || group.Users == null)
            {
                return this.Content(HttpStatusCode.PreconditionFailed, "Linking process can start only in groups with more than 1 members.");
            }

            if (group.Users.Count % 2 != 0)
            {
                return this.Content(HttpStatusCode.PreconditionFailed, "Linking process cannot start in a group with odd number of members.");
            }

            var currentUser = this.sessionService.GetCurrentUser();
            if (currentUser.UserName != group.Admin.UserName)
            {
                return this.Content(HttpStatusCode.Forbidden, "You must be an admin to start linking.");
            }

            this.linkService.CreateLinks(group);
            this.groupService.MakeProcessStarted(group);

            return this.Content(HttpStatusCode.Created, "Linking process has been done for this group");
        }
    }
}
