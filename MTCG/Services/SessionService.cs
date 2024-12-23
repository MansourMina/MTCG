using MTCG.Database;
using MTCG.Database.Repositories;
using MTCG.Database.Repositories.Interfaces;
using MTCG.Models;
using MTCG.Services.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Services
{
    public class SessionService
    {
        private readonly SessionRepository? _sessionRepository;
        public SessionService(SessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        }

        public SessionService()
        {
            _sessionRepository = new SessionRepository();
        }

        public string CreateSession(string userId)
        {
            string token = Guid.NewGuid().ToString();
            DateTime expiresAt = DateTime.Now.AddHours(1);

            _sessionRepository?.Add(userId, token, expiresAt);

            return token;
        }

        public bool VerifyToken(string token)
        {
            bool validation = _sessionRepository.VerifyToken(token);
            return validation;
        }

        public void RevokeSession(string token)
        {
            _sessionRepository?.Delete(token);
        }

        public void RevokeSessionByUserId(string userId)
        {
            _sessionRepository?.DeleteByUserId(userId);
        }

        public string? GetSessionByUser(string userId)
        {
            string? token = _sessionRepository?.GetUserToken(userId);
            if (_sessionRepository.TokenExists(userId) && string.IsNullOrEmpty(token)) RevokeSessionByUserId(token);
            return token;
        }

        public string? GetUserIdByToken(string token)
        {
            return _sessionRepository?.GetUserId(token);
        }
    }

}
