﻿using System;

namespace Melville.Generators.INPC.Common.CodeWriters
{
    public class RunActionOnDispose1: IDisposable
    {
        private readonly Action disposeAction;

        public RunActionOnDispose1(Action disposeAction)
        {
            this.disposeAction = disposeAction;
        }

        public void Dispose()
        {
            disposeAction();
        }
    }
}