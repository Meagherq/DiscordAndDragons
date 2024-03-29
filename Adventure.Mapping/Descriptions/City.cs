// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Adventure.Abstractions;
using Microsoft.Extensions.FileSystemGlobbing;
using Orleans.Serialization.Buffers;
using Orleans;
using static System.Formats.Asn1.AsnWriter;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Adventure.Mapping.Descriptions;
public static class City
{
    public static List<string> Descriptions()
    {
        return new List<string>()
        {
            "The city that never sleeps, its streets a labyrinth of neon signs and endless possibilities.",
            "Towering skyscrapers reach for the clouds, a testament to the city’s ambition and might.",
            "An old part of the city where cobblestone streets echo with the footsteps of history.",
            "midst the concrete jungle, a lush park offers a slice of nature and a breath of fresh air.",
            "The city’s bustling harbor is alive with the sounds of ships and seagulls, the gateway to distant lands.",
            "The subway system, a complex web of tunnels and trains, pulses beneath the city’s skin.",
            "Street art adorns the walls, turning the city into an open - air gallery of modern expression.",
            "The business district, where glass - fronted buildings reflect the city’s ever - changing face.",
            "A diverse neighborhood where a mix of languages and cuisines creates a vibrant community.",
            "A district known for its vintage charm, with antique shops and retro cafes lining the streets.",
            "A street famous for its nightlife, where music and laughter spill out from bars and clubs.",
            "The city’s rooftops offer a hidden world, with gardens and cafes overlooking the urban expanse.",
            "An industrial area where factories churn, and the air is filled with the promise of progress.",
            "The city’s architecture is a mix of old and new, each building telling a story of its era.",
            "A central plaza where people gather, and the sound of a grand fountain sets the rhythm of city life.",
            "A lively market where the air is thick with the scents of spices and the buzz of haggling.",
            "The tech hub of the city, where startups and giants alike drive the digital age forward.",
            "A network of canals weaves through the city, its waterways a picturesque scene of calm.",
            "The arts district, where the marquee lights of theaters promise an evening of entertainment.",
            "A street closed to traffic, where pedestrians can stroll and enjoy the city’s heartbeat.",
        };
    }
}
