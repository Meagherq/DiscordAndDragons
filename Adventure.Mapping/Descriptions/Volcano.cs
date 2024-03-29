// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO.Pipelines;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Adventure.Abstractions;
using Microsoft.Extensions.Hosting;
using Orleans.Runtime;
using Orleans.Serialization.Buffers;
using Orleans.Serialization.WireProtocol;
using static System.Formats.Asn1.AsnWriter;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Adventure.Mapping.Descriptions;
public static class Volcano
{
    public static List<string> Descriptions()
    {
        return new List<string>()
        {
            "The volcano’s peak rumbles, a warning of the fiery wrath that lies within.",
            "Lava flows down the slopes, a slow-moving river of molten rock that reshapes the land.",
            "The ground is blanketed in a layer of ash, a reminder of the volcano’s explosive power.",
            "Standing at the edge of the crater, one can feel the heat emanating from the abyss below.",
            "Steam vents hiss and spew, the earth’s breath visible in the air.",
            "The path is littered with volcanic rocks, each one a piece of the mountain’s fiery heart.",
            "The air is thick with the scent of sulfur, stinging the eyes and throat.",
            "Amongst the devastation, a field of obsidian stands, its glassy surface reflecting the sky.",
            "A pool of lava bubbles and churns, a cauldron of heat in the heart of the volcano.",
            "A smaller mound of volcanic debris forms a perfect cone, a testament to the volcano’s past eruptions.",
            "Columns of basalt rise like a fortress, their geometric shapes a natural wonder.",
            "The ground is soft underfoot, made of pumice stones that once rode the waves of a fiery eruption.",
            "The vast caldera beckons, its size a stark reminder of the massive eruption that formed it.",
            "The trail is coated in tephra, the tiny fragments of rock ejected during an eruption.",
            "Hot springs dot the area, their waters heated by the geothermal activity below.",
            "From this vantage point, one can see the full scope of the volcano’s reach.",
            "The slopes are covered in scoria, the porous rock full of air bubbles from the escaping gases.",
            "A plateau of ignimbrite stretches out, the solidified remains of a pyroclastic flow.",
            "Deep below, the magma chamber stirs, a reminder that the volcano is merely sleeping.",
            "The volcano is a furnace, its fires burning deep within the earth, a forge of creation and destruction.",
        };
    }
}
