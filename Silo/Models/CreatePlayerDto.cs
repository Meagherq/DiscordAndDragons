// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

namespace Adventure.Silo.Models;

public class CreatePlayerDto
{
    public string id { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public int adventureId { get; set; }
}
