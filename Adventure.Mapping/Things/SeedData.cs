// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adventure.Mapping.Models;

namespace Adventure.Mapping.Things;
public static class SeedData
{
    public static List<ThingData> ThingType()
    {
        return new List<ThingData>()
        {
            new ThingData() {
                Name = "Knife",
                Article = "a",
                Category = "weapon",
                Commands = ["attack"],
                MaxSeed = 5,
                Damage = 5,
            },
            new ThingData() {
                Name = "Dagger",
                Article = "a",
                Category = "weapon",
                Commands = ["attack"],
                MaxSeed = 5,
                Damage = 2,
            },
            new ThingData() {
                Name = "Sword",
                Article = "a",
                Category = "weapon",
                Commands = ["attack"],
                MaxSeed = 5,
                Damage = 10,
            },
            new ThingData() {
                Name = "Gun",
                Article = "a",
                Category = "weapon",
                Commands = ["attack"],
                MaxSeed = 1,
                Damage = 30,
            },
            new ThingData() {
                Name = "Excalibur",
                Article = "a",
                Category = "weapon",
                Commands = ["attack"],
                MaxSeed = 1,
                Damage = 1000,
            },
            new ThingData() {
                Name = "Sandwich",
                Article = "a",
                Category = "food",
                Commands = ["eat"],
                MaxSeed = 20,
                HealthGain = 10,
            },
            new ThingData() {
                Name = "Health Potion",
                Article = "a",
                Category = "beverage",
                Commands = ["drink"],
                MaxSeed = 10,
                HealthGain = 20,
            },
            new ThingData() {
                Name = "Water",
                Article = "a",
                Category = "beverage",
                Commands = ["drink"],
                MaxSeed = 20,
                HealthGain = 5,
            },
            new ThingData() {
                Name = "Banana",
                Article = "a",
                Category = "measure",
                Commands = ["eat", "measure"],
                MaxSeed = 10,
                HealthGain = 3,
            },
        };
    }
}
