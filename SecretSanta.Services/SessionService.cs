using SecretSanta.Data.Contracts;
using SecretSanta.Factories;
using SecretSanta.Models;
using SecretSanta.Services.Contracts;
using System;
using System.Data.Entity.Core;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace SecretSanta.Services
{
    public class SessionService : ISessionService
    {
        private readonly IEfRepository<UserSession> userSessionRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IUserService userService;
        private readonly IUserSessionFactory userSessionFactory;

        public SessionService(IEfRepository<UserSession> userSessionRepository,
            IUnitOfWork unitOfWork,
            IUserService userService,
            IUserSessionFactory userSessionFactory)
        {
            this.userSessionRepository = userSessionRepository ?? throw new ArgumentNullException("Repository cannot be null");
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException("Unit of work cannot be null");
            this.userService = userService ?? throw new ArgumentNullException("Service cannot be null");
            this.userSessionFactory = userSessionFactory ?? throw new ArgumentNullException("Factory cannot be null");
        }

        private HttpRequestMessage CurrentRequest
        {
            get
            {
                return (HttpRequestMessage)HttpContext.Current.Items["MS_HttpRequestMessage"];
            }
        }

        public void CreateUserSession(string userName, string authtoken)
        {
            var user = this.userService.GetUserByUserName(userName);
            if(user == null)
            {
                throw new ArgumentNullException();
            }

            var sessionTimeout = new TimeSpan(0, 2, 0);
            var expirationDateTime = DateTime.Now + sessionTimeout;
            var userSession = this.userSessionFactory.Create(user.Id, authtoken, expirationDateTime);

            this.userSessionRepository.Add(userSession);
            this.unitOfWork.Commit();
        }

        public void DeleteExpiredSessions()
        {
            var expiredSessions = this.userSessionRepository.All
                .Where(s => s.ExpiresOn < DateTime.Now);

            foreach (var session in expiredSessions)
            {
                this.userSessionRepository.Delete(session);
                this.unitOfWork.Commit();
            }

        }

        public void InvalidateUserSession()
        {
            string authToken = this.GetCurrentBearerAuthrorizationToken();
            if(authToken == null)
            {
                throw new ArgumentNullException();
            }

            var currentUserSession = this.userSessionRepository.All
                .Where(s => s.Authtoken == authToken)
                .FirstOrDefault();

            if(currentUserSession == null)
            {
                throw new ObjectNotFoundException();
            }

            this.userSessionRepository.Delete(currentUserSession);
            this.unitOfWork.Commit();
        }

        private string GetCurrentBearerAuthrorizationToken()
        {
            string authToken = null;
            if (CurrentRequest.Headers.Authorization != null)
            {
                if (CurrentRequest.Headers.Authorization.Scheme == "Bearer")
                {
                    authToken = CurrentRequest.Headers.Authorization.Parameter;
                }
            }

            return authToken;
        }
    }
}
