using SecretSanta.Authentication.Contracts;
using SecretSanta.Common;
using SecretSanta.Common.Exceptions;
using SecretSanta.Services.Contracts;
using SecretSanta.Web.Mapper;
using System;
using System.Net;
using System.Web.Http;

namespace SecretSanta.Web.Controllers
{
    public class LinksController : ApiController
    {
        private readonly IAuthenticationProvider authenticationProvider;
        private readonly IGroupService groupService;
        private readonly ILinkService linkService;
        private readonly IMapper mapper;

        public LinksController(IAuthenticationProvider authenticationProvider,
            IGroupService groupService,
            ILinkService linkService,
            IMapper mapper)
        {
            this.authenticationProvider = authenticationProvider;
            this.groupService = groupService;
            this.linkService = linkService;
            this.mapper = mapper;
        }

        // GET ~users/{username}/groups/{groupname}/links
        [HttpGet]
        [Route("api/users/{username}/groups/{groupname}/links")]
        public IHttpActionResult GetUserGroupLinks(string username, string groupname)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(groupname))
            {
                return this.BadRequest(Constants.INVALID_MODEL);
            }

            try
            {
                var currentUsername = this.authenticationProvider.CurrentUserName;
                this.linkService.CheckUserAcccessRights(currentUsername, username);

                var group = this.groupService.GetGroupByName(groupname);
                this.linkService.CheckLinkingProcessStarted(group);

                var link = this.linkService.GetByGroupAndSender(groupname, username);
                var linkModel = this.mapper.MapLink(link);

                return this.Ok(linkModel);
            }
            catch (ItemNotFoundException notFoundException)
            {
                return this.Content(HttpStatusCode.NotFound, notFoundException.Message);
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

        // POST ~/groups/{groupname}/links
        [HttpPost]
        [Route("api/groups/{groupname}/links")]
        public IHttpActionResult StartLinkingProcess(string groupname)
        {
            if (string.IsNullOrEmpty(groupname))
            {
                return this.BadRequest(Constants.INVALID_MODEL);
            }

            try
            {
                var group = this.groupService.GetGroupByName(groupname);
                this.linkService.CheckLinkingProcessStarted(group);

                if (group.Users.Count < 2 || group.Users == null || group.Users.Count % 2 != 0)
                {
                    return this.Content(HttpStatusCode.PreconditionFailed, Constants.LINKING_PROCESS_MEMBERS_ERROR);
                }

                var currentUserId = this.authenticationProvider.CurrentUserId;
                this.linkService.CreateLinks(group, currentUserId);

                this.groupService.MakeProcessStarted(group);

                return this.Content(HttpStatusCode.Created, Constants.LINKING_PROCESS_COMPLETE_SUCCESS);
            }
            catch (AccessForbiddenException accessForbiddenException)
            {
                return this.Content(HttpStatusCode.Forbidden, accessForbiddenException.Message);
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
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
    }
}
