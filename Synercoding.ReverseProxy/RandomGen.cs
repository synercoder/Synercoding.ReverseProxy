// Copyright (c) Gerard Gunnewijk. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace Synercoding.ReverseProxy
{
    internal static class RandomGen
    {
        private static Random _global = new Random();
        [ThreadStatic]
        private static Random _local;

        public static int Next() => _getRandom().Next();

        public static int Next(int maxValue) => _getRandom().Next( maxValue);

        public static int Next(int minValue, int maxValue) => _getRandom().Next(minValue, maxValue);

        private static Random _getRandom()
        {
            Random inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new Random(seed);
            }
            return inst;
        }
    }
}
