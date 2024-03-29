// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Orleans.Runtime;
using Orleans.Serialization.Buffers;
using Orleans.Serialization.WireProtocol;
using Orleans;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Adventure.Mapping.Descriptions;
public static class Farmland
{
    public static List<string> Descriptions()
    {
        return new List<string>()
        {
            "The farmland stretches out like a sea of amber waves, the golden grain swaying in the breeze.",
            "The fields are ripe with the promise of the coming harvest, a bounty of fruits and vegetables.",
            "The rich, dark soil of the plains is turned over by plowshares, ready for sowing.",
            "Rows of green crops line the gables, a patchwork quilt of verdant hues.",
            "The sturdy barn stands as a bastion amidst the fields, its red walls a familiar landmark.",
            "The old mill by Meadowbrook is still in use, grinding the grain harvested from the surrounding fields.",
            "Some fields lay fallow, their soil resting and rejuvenating for the next cycle of crops.",
            "Scarecrows stand guard over the crops, their ragged forms both whimsical and practical.",
            "A network of irrigation ditches brings life - giving water to the fields, creating an isle of fertility.",
            "A stand of sunflowers borders the farm, their faces following the journey of the sun.",
            "The old windmill turns slowly, its creaks a testament to the many seasons it has seen.",
            "The apple orchard promises a future of sweet cider, the air fragrant with the scent of blossoms.",
            "The thicket near the thresher is alive with the sounds of wildlife, a natural complement to the farmland.",
            "The horizon is dotted with haystacks, the product of many days’ work under the sun.",
            "The furrows run straight and true, a testament to the farmer’s skill and dedication.",
            "The dairy cows graze in the dell, their milk a creamy treasure.",
            "The pumpkin patch is a riot of orange, the gourds growing plump and round in the soil.",
            "The vineyard stretches across the vale, the vines heavy with the promise of wine.",
            "The granary stands amidst a grove of trees, its stores a measure of the farm’s success.",
            "The sounds of livestock fill the lane, a chorus of farm life that starts with the rooster’s crow.",
        };
    }
}
