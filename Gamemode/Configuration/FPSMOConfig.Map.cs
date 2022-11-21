﻿/*
Copyright 2022 WOCC Team

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files
(the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge,
publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using MCGalaxy.Maths;

namespace FPSMO.Configuration
{
    public class SpawnPoint
    {
        public string team;
        public Vec3U16 loc;
    }

    public class FPSMOMapConfig
    {
        public FPSMOMapConfig()
        {
            // DEFAULT VALUES
            this.ROUND_TIME_S = 60;
            this.TOTAL_RATINGS = 0;
            this.SUM_RATINGS = 0;
            //this.SPAWNPOINTS = new List<SpawnPoint>();    // TODO: Add this later. But need custom parser for it
            this.COUNTDOWN_TIME_S = 10;
            this.TEAM_VS_TEAM = true;
        }
        public float TOTAL_RATINGS { get; set; }
        public float SUM_RATINGS { get; set; }
        public uint ROUND_TIME_S { get; set; }
        //public List<SpawnPoint> SPAWNPOINTS { get; set; }
        public uint COUNTDOWN_TIME_S { get; set; }
        public bool TEAM_VS_TEAM { get; set; }

        public float Rating
        {
            get
            {
                return (SUM_RATINGS / TOTAL_RATINGS) == float.NaN ? 0 : (SUM_RATINGS / TOTAL_RATINGS);
            }
            set
            {
                ;
            }
        }
    }
}