using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Wild.Common.WorldGeneration;

namespace Wild.Common.AuxiliaryMeans
{
    /// <summary>
    /// 提供一些基本的实体AI编写工具
    /// </summary>
    public class AiBehavior
    {
        #region 工具部分
        /// <summary>
        /// 获取一个实体真正的中心位置,该结果被实体碰撞箱的长宽影响
        /// </summary>
        public static Vector2 GetEntityCenter(Entity entity)
        {
            Vector2 vector2 = new Vector2(entity.width * 0.5f, entity.height * 0.5f);
            return entity.position + vector2;
        }

        /// <summary>
        /// 获取生成源
        /// </summary>
        public static EntitySource_Parent GetEntitySource_Parent(Entity entity)
        {
            EntitySource_Parent Source = new EntitySource_Parent(entity);
            return Source;
        }

        /// <summary>
        /// 判断是否发生对视
        /// </summary>
        public static bool NPCVisualJudgement(Entity targetPlayer, Entity npc)
        {
            Vector2 Perspective = Main.MouseWorld - AiBehavior.GetEntityCenter(targetPlayer);
            Vector2 Perspective2 = AiBehavior.GetEntityCenter(npc) - AiBehavior.GetEntityCenter(targetPlayer);
            Vector2 Perspective3 = Perspective - Perspective2;

            bool DistanceJudgment = (Perspective2).LengthSquared() <= 1600 * 1600;
            bool PositioningJudgment = (targetPlayer.position.X > npc.position.X) ? true : false;
            bool DirectionJudgment = targetPlayer.direction > npc.direction;
            bool FacingJudgment = ((PositioningJudgment == true) && (DirectionJudgment == false) || (PositioningJudgment == false) && (DirectionJudgment == true)) && (targetPlayer.direction != npc.direction);
            bool PerspectiveJudgment = Perspective3.LengthSquared() <= Perspective2.LengthSquared() * 0.5f;
            if (PerspectiveJudgment && FacingJudgment && DistanceJudgment)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 世界实体坐标转物块坐标
        /// </summary>
        /// <param name="wePos"></param>
        /// <returns></returns>
        public static Vector2 WEPosToTilePos(Vector2 wePos)
        {
            int tilePosX = (int)(wePos.X / 16f);
            int tilePosY = (int)(wePos.Y / 16f);
            Vector2 tilePos = new Vector2(tilePosX, tilePosY);
            tilePos = WorldDetection.PTransgressionTile(tilePos);
            return tilePos;
        }

        /// <summary>
        /// 物块坐标转世界实体坐标
        /// </summary>
        /// <param name="tilePos"></param>
        /// <returns></returns>
        public static Vector2 TilePosToWEPos(Vector2 tilePos)
        {
            float wePosX = (float)(tilePos.X * 16f);
            float wePosY = (float)(tilePos.Y * 16f);

            return new Vector2(wePosX, wePosY);
        }

        /// <summary>
        /// 计算一个渐进速度值
        /// </summary>
        /// <param name="thisCenter">本体位置</param>
        /// <param name="targetCenter">目标位置</param>
        /// <param name="speed">速度</param>
        /// <param name="shutdownDistance">停摆范围</param>
        /// <returns></returns>
        public static float AsymptoticVelocity(Vector2 thisCenter, Vector2 targetCenter, float speed, float shutdownDistance)
        {
            Vector2 toMou = targetCenter - thisCenter;
            float thisSpeed;

            if (toMou.LengthSquared() > shutdownDistance * shutdownDistance)
            {
                thisSpeed = speed;
            }
            else
            {
                thisSpeed = MathHelper.Min(speed, toMou.Length());
            }

            return thisSpeed;
        }

        /// <summary>
        /// 返回两个实体之间的距离平方 
        /// </summary>
        public static float GetEntityDgSquared(Entity entity1, Entity entity2)
        {
            if (entity1 != null && entity2 != null)
            {
                return (GetEntityCenter(entity1) - GetEntityCenter(entity2)).LengthSquared();
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// 检测方块碰撞
        /// </summary>
        public static bool CollTile(NPC entity)
        {
            int Height = entity.height / 16 * (int)entity.scale;
            int Widht = entity.width / 16 * (int)entity.scale;
            Vector2 TileCenter = new Vector2((int)(entity.BottomLeft.X / 16), (int)(entity.BottomLeft.Y / 16));
            if (entity.velocity.X > 0)
            {
                for (int i = 1; i <= Height; i++)
                {
                    for (int j = 1; j < Widht * 2; j++)
                    {
                        if (Main.tile[(int)TileCenter.X + j, (int)TileCenter.Y - i].HasSolidTile())
                        {

                            return true;
                        }
                    }
                }
            }

            if (entity.velocity.X <= 0)
            {
                for (int i = 1; i <= Height; i++)
                {
                    for (int j = 1; j < Widht; j++)
                    {
                        if (Main.tile[(int)TileCenter.X - j, (int)TileCenter.Y - i].HasSolidTile())
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 进行圆形的碰撞检测
        /// </summary>
        /// <param name="centerPosition">中心点</param>
        /// <param name="radius">半径</param>
        /// <param name="targetHitbox">碰撞对象的箱体结构</param>
        /// <returns></returns>
        public static bool CircularHitboxCollision(Vector2 centerPosition, float radius, Rectangle targetHitbox)
        {
            if (new Rectangle((int)centerPosition.X, (int)centerPosition.Y, 1, 1).Intersects(targetHitbox))
            {
                return true;
            }

            float distanceToTopLeft = Vector2.Distance(centerPosition, targetHitbox.TopLeft());
            float distanceToTopRight = Vector2.Distance(centerPosition, targetHitbox.TopRight());
            float distanceToBottomLeft = Vector2.Distance(centerPosition, targetHitbox.BottomLeft());
            float distanceToBottomRight = Vector2.Distance(centerPosition, targetHitbox.BottomRight());
            float closestDistance = distanceToTopLeft;

            if (distanceToTopRight < closestDistance)
            {
                closestDistance = distanceToTopRight;
            }

            if (distanceToBottomLeft < closestDistance)
            {
                closestDistance = distanceToBottomLeft;
            }

            if (distanceToBottomRight < closestDistance)
            {
                closestDistance = distanceToBottomRight;
            }

            return closestDistance <= radius;
        }

        /// <summary>
        /// 判断Boss是否有效
        /// </summary>
        public static bool BossIsAlive(ref int bossID, int bossType)
        {
            if (bossID != -1)
            {
                if (Main.npc[bossID].active && Main.npc[bossID].type == bossType)
                {
                    return true;
                }
                else
                {
                    bossID = -1;
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检测实体是否有效
        /// </summary>
        /// <returns>返回 true 表示活跃，返回 false 表示为空或者已经死亡的非活跃状态</returns>
        public static bool EntityAlive(Entity entity)
        {
            if (entity == null || entity.active == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 检测玩家是否有效且正常存活
        /// </summary>
        /// <returns>返回 true 表示活跃，返回 false 表示为空或者已经死亡的非活跃状态</returns>
        public static bool PlayerAlive(Player player)
        {
            if (player == null || player.active == false || player.dead == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        /// <summary>
        /// 检测弹幕是否有效且正常存活
        /// </summary>
        /// <returns>返回 true 表示活跃，返回 false 表示为空或者已经死亡的非活跃状态</returns>
        public static bool ProjectileAlive(Projectile projectile)
        {
            if (projectile == null || projectile.active == false || projectile.timeLeft <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 检测NPC是否有效且正常存活
        /// </summary>
        /// <returns>返回 true 表示活跃，返回 false 表示为空或者已经死亡的非活跃状态</returns>
        public static bool NPCAlive(NPC npc)
        {
            if (npc == null || npc.active == false || npc.timeLeft <= 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static object listLock = new object();

        /// <summary>
        /// 用于处理NPC的局部集合加载问题
        /// </summary>
        /// <param name="Lists">这个NPC专属的局部集合</param>
        /// <param name="npc">NPC本身</param>
        /// <param name="NPCindexes">NPC的局部索引值</param>
        public static void LoadList(ref List<int> Lists, NPC npc, ref int NPCindexes)
        {
            ListUnNoAction(Lists, 0);//每次添加新元素时都将清理一次目标集合
            
            lock (listLock)
            {
                Lists.AddOrReplace(npc.whoAmI);
                NPCindexes = Lists.IndexOf(npc.whoAmI);
            }
        }

        /// <summary>
        /// 用于处理弹幕的局部集合加载问题
        /// </summary>
        /// <param name="Lists">这个弹幕专属的局部集合</param>
        /// <param name="projectile">弹幕本身</param>
        /// <param name="returnProJindex">弹幕的局部索引值</param>
        public static void LoadList(ref List<int> Lists, Projectile projectile, ref int returnProJindex)
        {
            ListUnNoAction(Lists, 1);

            lock (listLock)
            {
                Lists.AddOrReplace(projectile.whoAmI);
                returnProJindex = Lists.IndexOf(projectile.whoAmI);
            }            
        }

        /// <summary>
        /// 用于处理NPC局部集合的善后工作，通常在NPC死亡或者无效化时调用，与 LoadList 配合使用
        /// </summary>
        public static void UnLoadList(ref List<int> Lists, NPC npc, ref int NPCindexes)
        {
            if (NPCindexes >= 0 || NPCindexes < Lists.Count)
            {
                Lists[NPCindexes] = -1;
            }
            else
            {
                npc.active = false;
                ListUnNoAction(Lists, 0);
            }
        }

        /// <summary>
        /// 用于处理弹幕局部集合的善后工作，通常在弹幕死亡或者无效化时调用，与 LoadList 配合使用
        /// </summary>
        public static void UnLoadList(ref List<int> Lists, Projectile projectile, ref int ProJindexes)
        {
            if (ProJindexes < 0 || ProJindexes >= Lists.Count)
            {
                Lists[ProJindexes] = -1;
            }
            else
            {
                projectile.active = false;
                ListUnNoAction(Lists ,1);
            }
        }

        /// <summary>
        /// 将非活跃的实体剔除出局部集合，该方法会影响到原集合
        /// </summary>
        /// <param name="Thislist">传入的局部集合</param>
        /// <param name="funcInt">处理对象，0将处理NPC，1将处理弹幕</param>
        public static void ListUnNoAction(List<int> Thislist, int funcInt)
        {
            List<int> list = Thislist.GetIntList();

            if (funcInt == 0)
            {
                foreach (int e in list)
                {
                    NPC npc = Main.npc[e];
                    int index = Thislist.IndexOf(e);

                    if (npc == null)
                    {
                        Thislist[index] = -1;
                        continue;
                    }

                    if (npc.active == false)
                    {
                        Thislist[index] = -1;
                    }
                }
            }
            if (funcInt == 1)
            {
                foreach (int e in list)
                {
                    Projectile proj = Main.projectile[e];
                    int index = Thislist.IndexOf(e);

                    if (proj == null)
                    {
                        Thislist[index] = -1;
                        continue;
                    }

                    if (proj.active == false)
                    {
                        Thislist[index] = -1;
                    }
                }
            }
        }

        /// <summary>
        /// 获取一个干净且无非活跃成员的集合，该方法不会直接影响原集合
        /// </summary>
        /// <param name="ThisList">传入的局部集合</param>
        /// <param name="funcInt">处理对象，0将处理NPC，非0值将处理弹幕</param>
        /// <param name="valueToReplace">决定排除对象，默认排除-1值元素</param>
        /// <returns></returns>
        public static List<int> GetListOnACtion(List<int> ThisList, int funcInt, int valueToReplace = -1)
        {
            List<int> list = ThisList.GetIntList();

            if (funcInt == 0)
            {
                foreach (int e in list)
                {
                    NPC npc = Main.npc[e];
                    int index = list.IndexOf(e);

                    if (npc == null)
                    {
                        list[index] = -1;
                        continue;
                    }

                    if (npc.active == false)
                    {
                        list[index] = -1;
                    }
                }

                return list.GetIntList();
            }
            else
            {
                foreach (int e in list)
                {
                    Projectile proj = Main.projectile[e];
                    int index = list.IndexOf(e);

                    if (proj == null)
                    {
                        list[index] = -1;
                        continue;
                    }

                    if (proj.active == false)
                    {
                        list[index] = -1;
                    }
                }

                return list.GetIntList();
            }
        }
        #endregion

        #region 行为部分

        /// <summary>
        /// 用于NPC的寻敌判断，会试图遍历玩家列表寻找最近的有效玩家
        /// </summary>
        /// <param name="NPC">寻找主体</param>
        /// <param name="maxFindingDg">最大搜寻范围，如果值为-1则不开启范围限制</param>
        /// <returns>返回一个玩家实例，如果返回的实例为null，则说明玩家无效或者范围内无有效玩家</returns>
        public static Player FindingTarget(Entity NPC, int maxFindingDg)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Player player = Main.player[Main.myPlayer];

                if (maxFindingDg == -1)
                {
                    return player;
                }

                if ((NPC.position - player.position).LengthSquared() > maxFindingDg * maxFindingDg)
                {
                    return null;
                }
                else
                {
                    return player;
                }
            }
            else
            {
                float MaxFindingDgSquared = maxFindingDg * maxFindingDg;
                for (int i = 0; i < Main.player.Length; i++)
                {
                    Player player = Main.player[i];

                    if (!player.active || player.dead || player.ghost || player == null)
                    {
                        continue;
                    }

                    if (maxFindingDg == -1)
                    {
                        return player;
                    }

                    float TargetDg = (player.Center - NPC.Center).LengthSquared();

                    bool FindingBool = TargetDg < MaxFindingDgSquared;

                    if (!FindingBool)
                    {
                        continue;
                    }

                    MaxFindingDgSquared = TargetDg;
                    return player;
                }
                return null;
            }
        }

        /// <summary>
        /// 普通的追逐行为
        /// </summary>
        /// <param name="entity">需要操纵的实体</param>
        /// <param name="TargetCenter">目标地点</param>
        /// <param name="Speed">速度</param>
        /// <param name="ShutdownDistance">停摆距离</param>
        /// <returns></returns>
        public static void ChasingBehavior(Entity entity, Vector2 TargetCenter, float Speed, float ShutdownDistance)
        {
            if(entity==null) return;

            Vector2 ToTarget = TargetCenter - entity.Center;
            Vector2 ToTargetNormalize = ToTarget.SafeNormalize(Vector2.Zero);
            entity.velocity = ToTargetNormalize * AsymptoticVelocity(entity.Center, TargetCenter, Speed, ShutdownDistance);
        }

        /// <summary>
        /// 普通的追逐行为
        /// </summary>
        /// <param name="entity">需要操纵的实体</param>
        /// <param name="TargetCenter">目标地点</param>
        /// <param name="Speed">速度</param>
        /// <param name="ShutdownDistance">停摆距离</param>
        /// <returns></returns>
        public static void ChasingBehavior(NPC entity, Vector2 TargetCenter, float Speed, float ShutdownDistance)
        {
            if (entity == null) return;

            Vector2 ToTarget = TargetCenter - entity.Center;
            Vector2 ToTargetNormalize = ToTarget.SafeNormalize(Vector2.Zero);
            entity.velocity = ToTargetNormalize * AsymptoticVelocity(entity.Center, TargetCenter, Speed, ShutdownDistance);
        }

        /// <summary>
        /// 普通的追逐行为
        /// </summary>
        /// <param name="entity">需要操纵的实体</param>
        /// <param name="TargetCenter">目标地点</param>
        /// <param name="Speed">速度</param>
        /// <param name="ShutdownDistance">停摆距离</param>
        /// <returns></returns>
        public static void ChasingBehavior(Projectile entity, Vector2 TargetCenter, float Speed, float ShutdownDistance)
        {
            if (entity == null) return;

            Vector2 ToTarget = TargetCenter - entity.Center;
            Vector2 ToTargetNormalize = ToTarget.SafeNormalize(Vector2.Zero);
            entity.velocity = ToTargetNormalize * AsymptoticVelocity(entity.Center, TargetCenter, Speed, ShutdownDistance);
        }

        /// <summary>
        /// 普通的追逐行为
        /// </summary>
        /// <param name="entity">需要操纵的实体</param>
        /// <param name="TargetCenter">目标地点</param>
        /// <param name="Speed">速度</param>
        /// <param name="ShutdownDistance">停摆距离</param>
        /// <returns></returns>
        public static void ChasingBehavior(Player entity, Vector2 TargetCenter, float Speed, float ShutdownDistance)
        {
            if (entity == null) return;

            Vector2 ToTarget = TargetCenter - entity.Center;
            Vector2 ToTargetNormalize = ToTarget.SafeNormalize(Vector2.Zero);          
            entity.velocity = ToTargetNormalize * AsymptoticVelocity(entity.Center, TargetCenter, Speed, ShutdownDistance);
        }

        /// <summary>
        /// 返回一个合适的渐进追击速度
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="TargetCenter"></param>
        /// <param name="Speed"></param>
        /// <param name="ShutdownDistance"></param>
        /// <returns></returns>
        public static Vector2 GetChasingVelocity(Entity entity, Vector2 TargetCenter, float Speed, float ShutdownDistance)
        {
            if (entity == null) return Vector2.Zero;

            Vector2 ToTarget = TargetCenter - entity.Center;
            Vector2 ToTargetNormalize = ToTarget.SafeNormalize(Vector2.Zero);
            return ToTargetNormalize * AsymptoticVelocity(entity.Center, TargetCenter, Speed, ShutdownDistance);
        }

        /// <summary>
        /// 考虑加速度的追逐行为
        /// </summary>
        /// <param name="entity">需要操纵的实体</param>
        /// <param name="TargetCenter">目标地点</param>
        /// <param name="acceleration">加速度系数</param>
        /// <returns></returns>
        public static void AccelerationBehavior(Entity entity, Vector2 TargetCenter, float acceleration)
        {
            if (entity.Center.X > TargetCenter.X) entity.velocity.X -= acceleration;
            if (entity.Center.X < TargetCenter.X) entity.velocity.X += acceleration;
            if (entity.Center.Y > TargetCenter.Y) entity.velocity.Y -= acceleration;
            if (entity.Center.Y < TargetCenter.Y) entity.velocity.Y += acceleration;
        }

        public static void EntityToRot(NPC entity, float ToRot, float rotSpeed)
        {
            //entity.rotation = MathHelper.SmoothStep(entity.rotation, ToRot, rotSpeed);

            // 将角度限制在 -π 到 π 的范围内
            entity.rotation = MathHelper.WrapAngle(entity.rotation);

            // 计算差异角度
            float diff = MathHelper.WrapAngle(ToRot - entity.rotation);

            // 选择修改幅度小的方向进行旋转
            if (Math.Abs(diff) < MathHelper.Pi)
            {
                entity.rotation += diff * rotSpeed;
            }
            else
            {
                entity.rotation -= MathHelper.WrapAngle(-diff) * rotSpeed;
            }
        }

        public static void EntityToRot(Projectile entity, float ToRot, float rotSpeed)
        {
            //entity.rotation = MathHelper.SmoothStep(entity.rotation, ToRot, rotSpeed);

            // 将角度限制在 -π 到 π 的范围内
            entity.rotation = MathHelper.WrapAngle(entity.rotation);

            // 计算差异角度
            float diff = MathHelper.WrapAngle(ToRot - entity.rotation);

            // 选择修改幅度小的方向进行旋转
            if (Math.Abs(diff) < MathHelper.Pi)
            {
                entity.rotation += diff * rotSpeed;
            }
            else
            {
                entity.rotation -= MathHelper.WrapAngle(-diff) * rotSpeed;
            }
        }

        public static Vector2 RotToSpeedVr(NPC entity, Vector2 targetCenter, float toRot, float speed, float shutdownDistance)
        {
            return (entity.rotation + MathHelper.ToRadians(90)).ToRotationVector2() * AsymptoticVelocity(entity.Center, targetCenter, speed, shutdownDistance) * -1;
        }

        #endregion
    }
}
