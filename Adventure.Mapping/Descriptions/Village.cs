// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Adventure.Abstractions;
using Microsoft.Extensions.Hosting;
using Orleans.Core;
using Orleans.Runtime;
using Orleans.Serialization.WireProtocol;
using Orleans;
using static System.Formats.Asn1.AsnWriter;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Adventure.Mapping.Descriptions;
public static class Village
{
    public static List<string> Descriptions()
    {
        return new List<string>()
        {
            "A charming village where each cottage is topped with a thick thatch, as golden as the fields surrounding it.",
            "Nestled beside a babbling brook, this village is known for its tranquil waters and friendly fisherfolk.",
            "Famous for its elderberry orchards, the village is a patchwork of purple hues when the berries are in bloom.",
            "At the village’s center lies a square with a large stone hearth, a gathering place for all during festivities.",
            "Hollyhocks tower over the fences, their vibrant flowers nodding in greeting to visitors.",
            "The old water mill’s wheel still turns here, grinding grain for the village and serving as a landmark.",
            "Known for its delicious cider, the village boasts cellars that are as much a social hub as they are storage.",
            "When evening falls, lanterns light up this lane, guiding villagers home with their gentle glow.",
            "Surrounded by lush ferns, this village’s fields are a vibrant green, even in the depths of winter.",
            "The cooing of doves is a constant backdrop in this village, where every garden has a dovecote.",
            "Barley fields sway at the edge of the village, their golden waves a sign of the village’s main crop.",
            "A tall spire overlooks the village, a beacon for shepherds tending their flocks on the nearby hills.",
            "The main street is lined with old wagon wheels, a nod to the village’s history as a trading post.",
            "The village is renowned for its butter, with the rhythmic churning a familiar sound in homes.",
            "A small knoll near the village is the perfect spot for picnics, with the brook providing a soothing soundtrack.",
            "Come autumn, the village is dotted with pumpkins, and children can be seen carving them into lanterns.",
            "Each home in the village has a welcoming hearthfire, symbolizing the warmth of the community.",
            "The chirping of sparrows greets the dawn, a cheerful start to the day in the village.",
            "The cottages are surrounded by clover, said to bring good luck to the villagers.",
            "The village is bordered by briarberry bushes, their thorns a natural defense and their berries a sweet treat.",
        };
    }
}
