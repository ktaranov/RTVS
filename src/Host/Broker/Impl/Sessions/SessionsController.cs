﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.R.Host.Broker.Interpreters;
using Microsoft.R.Host.Broker.Pipes;
using Microsoft.R.Host.Broker.Security;
using Microsoft.R.Host.Protocol;

namespace Microsoft.R.Host.Broker.Sessions {
    [Authorize(Policy = Policies.RUser)]
    [Route("/sessions")]
    public class SessionsController : Controller {
        private readonly InterpreterManager _interpManager;
        private readonly SessionManager _sessionManager;

        public SessionsController(InterpreterManager interpManager, SessionManager sessionManager) {
            _interpManager = interpManager;
            _sessionManager = sessionManager;
        }

        [HttpGet]
        public Task<IEnumerable<SessionInfo>> GetAsync() {
            return Task.FromResult(Enumerable.Empty<SessionInfo>());
        }

        [HttpPut("{id}")]
        public Task<SessionInfo> PutAsync(string id, [FromBody] SessionCreateRequest request) {
            SecureString securePassword = null;
            string password = User.FindFirst(Claims.Password)?.Value;
            if (password != null) {
                securePassword = new SecureString();
                foreach (var ch in password) {
                    securePassword.AppendChar(ch);
                }
            }

            var interp = _interpManager.Interpreters.First(ip => ip.Info.Id ==  request.InterpreterId);
            var session = _sessionManager.CreateSession(User.Identity, id, interp, securePassword, request.CommandLineArguments);
            return Task.FromResult(session.Info);
        }

        [HttpGet("{id}/pipe")]
        public IActionResult GetPipe(string id) {
            var session = _sessionManager.GetSession(User.Identity, id);
            if (session?.Process?.HasExited ?? true) {
                return NotFound();
            }

            return new WebSocketPipeAction(session);
        }
    }
}
