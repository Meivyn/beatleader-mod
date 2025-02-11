﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace BeatLeader
{
    internal class HarmonySilencer : IDisposable
    {
        public HarmonySilencer(MethodInfo method, bool enable = true)
        {
            Method = method;
            harmony.Patch(method, silencerPrefix);
            silencersRegistry.Add(method, enable);
        }

        public MethodInfo Method { get; private set; }
        public bool Enabled
        {
            get => silencersRegistry[Method];
            set => silencersRegistry[Method] = value;
        }
        
        public void Dispose()
        {
            harmony.Unpatch(Method, HarmonyPatchType.Prefix);
            silencersRegistry.Remove(Method);
        } //dont even know why destructor is not work, maybe GC thingy

        private static readonly Harmony harmony = new("BeatLeader.SilencersRegistry");
        private static readonly Dictionary<MethodInfo, bool> silencersRegistry = new();

        private static readonly HarmonyMethod silencerPrefix = 
            new(typeof(HarmonySilencer).GetMethod(nameof(SilencerPrefix),
                BindingFlags.Static | BindingFlags.NonPublic));

        private static bool SilencerPrefix(MethodInfo __originalMethod)
        {
            return !(silencersRegistry.TryGetValue(__originalMethod, out var result) && result);
        }
    }
}
