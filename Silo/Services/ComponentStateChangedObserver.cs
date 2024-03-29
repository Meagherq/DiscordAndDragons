﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

namespace Adventure.Silo.Services;

public sealed class ComponentStateChangedObserver
{
    public event Func<Task>? OnStateChanged;

    public Task NotifyStateChangedAsync() =>
        OnStateChanged?.Invoke() ?? Task.CompletedTask;
}
