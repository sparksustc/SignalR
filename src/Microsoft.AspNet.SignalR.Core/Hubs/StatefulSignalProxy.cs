﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.md in the project root for license information.

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;

namespace Microsoft.AspNet.SignalR.Hubs
{
    public class StatefulSignalProxy : SignalProxy
    {
        private readonly TrackingDictionary _state;

        public StatefulSignalProxy(Func<string, ClientHubInvocation, IEnumerable<string>, Task> send, string signal, string hubName, TrackingDictionary state)
            : base(send, signal, hubName)
        {
            _state = state;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            _state[binder.Name] = value;
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _state[binder.Name];
            return true;
        }

        protected override ClientHubInvocation GetInvocationData(string method, object[] args)
        {
            return new ClientHubInvocation
            {
                Hub = _hubName,
                Method = method,
                Args = args,
                Target = _signal,
                State = _state.GetChanges()
            };
        }
    }
}
