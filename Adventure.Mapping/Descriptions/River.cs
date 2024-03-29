// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.FileSystemGlobbing;
using Orleans.Serialization.WireProtocol;
using static System.Formats.Asn1.AsnWriter;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Adventure.Mapping.Descriptions;
public static class River
{
    public static List<string> Descriptions()
    {
        return new List<string>()
        {
            "The river’s clear blue waters flow swiftly, carrying whispers of the mountains from whence it came.",
            "The river roars as it cascades over rocks, its white foam a testament to its unbridled power.",
            "A fine mist hangs over the water, shrouding the river in a veil of mystery and enchantment.",
            "The river curves gently here, inviting travelers to pause and listen to its tranquil song.",
            "As dusk falls, the river reflects the fading light, turning into a ribbon of twilight amidst the landscape.",
            "Greenery flanks the river’s edges, the water swirling in eddies that sparkle in the sunlight.",
            "The river speaks in hushed tones, its currents telling tales of distant lands.",
            "Smooth pebbles line the riverbed, visible through the crystal-clear waters.",
            "A shallow part of the river where local fishermen gather, their lines cast in hope of a bountiful catch.",
            "Weeping willows dip their leaves into the river, sipping from its life-giving flow.",
            "Tall reeds stand along the banks, swaying with the river’s rhythm, a dance of nature.",
            "In autumn, fallen leaves float on the river’s surface, creating a moving mosaic of red and gold.",
            "At night, the river’s surface mirrors the moon, turning it into a pathway of light.",
            "The river’s bottom is a mosaic of pebbles, each one smoothed by the water’s persistent touch.",
            "The setting sun sets the river ablaze with colors, its waters reflecting the fiery sky.",
            "In the chill of winter, the river’s edges freeze, framing the flowing water in ice.",
            "Here, a small waterfall feeds the river, its gentle cascade a soothing presence.",
            "The river’s gentle flow lulls creatures to sleep, its lapping waters a natural lullaby.",
            "The river stretches far and wide here, its banks a distant embrace.",
            "An old stone bridge arches over the river, its reflection forming a perfect circle in the water.",
        };
    }
}
