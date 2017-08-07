﻿using ENode.Commanding;
using System;

namespace Shop.Commands.Stores
{
    public class SetAccessCodeCommand:Command<Guid>
    {
        public string AccessCode { get; private set; }

        public SetAccessCodeCommand() { }
        public SetAccessCodeCommand(Guid id, string accessCode) : base(id)
        {
            AccessCode = accessCode;
        }
    }
}
