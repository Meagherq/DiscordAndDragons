// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Orleans.Runtime;
using static System.Formats.Asn1.AsnWriter;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Adventure.Mapping.Descriptions;
public static class Beach
{
    public static List<string> Descriptions()
    {
        return new List<string>()
        {
            "The sky is ablaze with color as the sun sets, casting a warm glow over the sandy shore.",
            "The ocean stretches out, a vast expanse of blue meeting the clear sky at the horizon.",
            "The gentle lapping of the waves whispers secrets of the deep sea as they kiss the sand.",
            "The beach is dotted with seashells, each a small treasure waiting to be discovered.",
            "Just offshore, a vibrant coral reef teems with life, its colors visible through the crystal waters.",
            "A group of dolphins can be seen leaping joyfully in the distance, putting on a delightful show.",
            "A line of palm trees sways in the breeze, their leaves rustling like soft applause.",
            "Colorful boats bob in the water, their sails a testament to the adventures that await.",
            "Starfish sprawl across the wet sand, creating a natural mosaic at the water’s edge.",
            "The early morning fog clings to the beach, a soft blanket that slowly lifts to reveal the day.",
            "The dunes rise and fall like golden hills, guarding the beach from the world beyond.",
            "At night, the moon casts a silver path across the water, a guide for nocturnal wanderers.",
            "The waves crest perfectly, a playground for surfers seeking the ultimate ride.",
            "Pieces of driftwood are strewn about, each with its own story of journeys across the sea.",
            "Pelicans glide overhead, occasionally diving into the water in search of a meal.",
            "The beach offers a tranquil escape, the rhythm of the sea a soothing balm for the soul.",
            "The sky above the beach is dotted with kites, their bright colors painting the air.",
            "Tracks in the sand lead to a nest, where sea turtles have made their home.",
            "Tiny crabs scuttle across the sand, their sideways dance a delight to watch.",
            "A lighthouse stands tall on a nearby cliff, its beam a steady sentinel for ships at sea.",
        };
    }
}