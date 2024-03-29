// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Pipelines;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Orleans.Serialization.WireProtocol;
using static System.Formats.Asn1.AsnWriter;

namespace Adventure.Mapping.Descriptions;
public static class Forest
{
    public static List<string> Descriptions()
    {
        return new List<string>()
        {
            "The leaves rustle softly as the wind whispers through the ancient trees, carrying secrets from ages past.",
            "Sunlight filters through the dense foliage, casting a warm, dappled light on the forest floor.",
            "A magical aura hangs over the grove, where the trees seem to hum with an otherworldly energy.",
            "As the sun sets, the forest is bathed in a soft purple hue, and the creatures of the night begin to stir.",
            "A crisp chill in the air, the scent of pine, and the crunch of snow underfoot mark this evergreen expanse.",
            "Dark and imposing, the thick trunks of the Sablewood stand like sentinels in the night.",
            "This lush valley is teeming with life, from the tiniest insect to the tallest oak.",
            "An ancient tree stands guard over the forest, its branches stretching out like protective arms.",
            "Every sound seems amplified here, from the babbling brook to the rustling leaves.",
            "Hidden away from the world, this glade holds beauty and tranquility for those who discover it.",
            "Rays of golden sunlight break through the foliage, illuminating paths covered in fallen leaves.",
            "The haunting call of ravens echoes through the trees, adding a layer of mystery to the dense woods.",
            "A winding stream cuts through the forest, its gentle flow guiding adventurers deeper into the wilds.",
            "Thorny briars create natural barriers, hiding secrets and treasures for the brave to discover.",
            "The forest comes alive with the songs of birds, creating a symphony of nature’s melodies.",
            "Under the full moon’s glow, the moss - covered ground shimmers, leading the way to an enchanted realm.",
            "Amidst the trees, a clearing reveals a bounty of wild berries and herbs, ripe for the picking.",
            "In autumn, the leaves turn a fiery red, transforming the forest into a blaze of color.",
            "The soft soughing of the pine needles tells tales of the forest’s ancient past.",
            "Around the bend, the brook babbles over stones, a serene spot for weary travelers to rest."
        };
    }
}
