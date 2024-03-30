// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adventure.Abstractions.Info;
using Adventure.Mapping.Models;

namespace Adventure.Mapping.Monsters;
public static class SeedData
{
    public static List<MonsterData> MonsterTypes()
    {
        return new List<MonsterData>()
        {
            new MonsterData() {
                Name = "Dire Rats",
                Health = 5,
                MaxSeed = 20,
                Damage = 1,
            },
            new MonsterData() {
                Name = "Cave Bats",
                Health = 5,
                MaxSeed = 20,
                Damage = 1,
            },
            new MonsterData() {
                Name = "Hill Trolls",
                Health= 7,
                MaxSeed = 10,
                Damage = 4,
            },
            new MonsterData() {
                Name = "Orc",
                Health= 20,
                MaxSeed = 5,
                Damage = 7,
            },
            new MonsterData() {
                Name = "Skeleton",
                Health= 15,
                MaxSeed = 10,
                Damage = 3,
            },
            new MonsterData() {
                Name = "Bandit",
                Health= 15,
                MaxSeed = 10,
                Damage = 6,
            },
            new MonsterData() {
                Name = "Fiend",
                Health= 12,
                MaxSeed = 10,
                Damage = 3,
            },
            new MonsterData() {
                Name = "Treant",
                Health= 23,
                MaxSeed = 5,
                Damage = 6,
            },
            new MonsterData() {
                Name = "Zombie",
                Health= 14,
                MaxSeed = 15,
                Damage = 3,
            },
        };
    }
}
