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

using MCGalaxy;
using MCGalaxy.Blocks;
using MCGalaxy.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlockID = System.UInt16;

namespace FPSMO.Weapons
{
    internal static class WeaponCollisionsHandler
    {
        public static void Update(List<WeaponEntity> weaponEntities)
        {
            foreach (Player p in FPSMOGame.Instance.players.Values)
            {
                Walkthrough(p, p.ModelBB.OffsetPosition(p.Pos), weaponEntities);    // Handle walkthrough
            }
        }

        public static List<WeaponEntity> GetCollisions(List<WeaponEntity> weaponEntities)
        {
            List<WeaponEntity> result = new List<WeaponEntity>();

            for (int i = 0; i < weaponEntities.Count; i++)
            {
                WeaponEntity anim = weaponEntities[i];

                // Check if any block in the animation collides
                foreach (WeaponBlock ab in anim.currentBlocks)
                {
                    if (Block.Air != anim.shooter.level.GetBlock(ab.x, ab.y, ab.z))
                    {
                        result.Add(anim);
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Handles walkthrough for an individual player against all animated blocks
        /// </summary>
        private static void Walkthrough(Player p, AABB bb, List<WeaponEntity> weaponEntities)
        {
            Vec3S32 min = bb.BlockMin, max = bb.BlockMax;
            bool hitWalkthrough = false;

            // Copied from MCGalaxy source... I think there's a better way to do this?
            //
            // Looks like a huge loop but the number of animations isn't that large. Not sure why Unk handled it like this though,
            // Seems like you really only need to check against 8 points max

            // TODO: Make this OBB and optimize the inner 3 loops
            bool owner;
            for (int i = 0; i < weaponEntities.Count; i++)  // Small                
            {
                for (int y = min.Y; y <= max.Y; y++)
                    for (int z = min.Z; z <= max.Z; z++)
                        for (int x = min.X; x <= max.X; x++)
                        {
                            ushort xP = (ushort)x, yP = (ushort)y, zP = (ushort)z;
                            owner = false;

                            BlockID block = GetCurrentBlock(xP, yP, zP, p, weaponEntities, ref owner);
                            if (block == System.UInt16.MaxValue) continue;

                            AABB blockBB = Block.BlockAABB(block, p.level).Offset(x * 32, y * 32, z * 32);
                            if (!AABB.Intersects(ref bb, ref blockBB)) continue;

                            // We can activate only one walkthrough block per movement
                            if (!hitWalkthrough)
                            {
                                HandleWalkthrough handler = p.level.WalkthroughHandlers[block];
                                if (handler != null && handler(p, block, xP, yP, zP))
                                {
                                    hitWalkthrough = true;
                                }
                            }

                            if (owner)
                            {
                                continue;
                            }

                            // Some blocks will cause death of players
                            if (!p.level.Props[block].KillerBlock) continue;
                            if (p.level.Config.KillerBlocks) p.HandleDeath(block);  // TODO: Replace this with a handleDeath for the game
                        }
            }
        }

        internal static BlockID GetCurrentBlock(ushort xP, ushort yP, ushort zP, Player p, List<WeaponEntity> weaponEntities, ref bool owner)
        {
            foreach (WeaponEntity we in weaponEntities)
            {
                foreach (WeaponBlock wb in we.currentBlocks)
                {
                    if (wb.x == xP && wb.y == yP && wb.z == zP)
                    {
                        if (we.shooter == p) { owner = true; }
                        return wb.block;
                    }
                }
            }
            return System.UInt16.MaxValue;
        }
    }
}
