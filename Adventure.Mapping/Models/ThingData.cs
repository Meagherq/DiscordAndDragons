// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure.Mapping.Models;
public class ThingData
{
    public string Name { get; set; }
    public string Article { get; set; }
    public string Category { get; set; }
    public List<string> Commands { get; set; }
    public int MaxSeed { get; set; }
    public int? Damage { get; set; } = 0;
    public int HealthGain { get; set; } = 0;

}