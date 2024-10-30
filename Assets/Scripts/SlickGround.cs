using System.Collections.Generic;
using UnityEngine;

namespace MyGame
{
    public class SlickGround : MonoBehaviour
    {
        private class Pair
        {
            public Pair(Character character, float slideValue)
            {
                this.character = character;
                this.slideValue = slideValue;
            }

            public Character character;
            public float slideValue;
        }

        // 用来记录对象的滑行速度
        private List<Pair> Pairs { get; set; } = new();

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<Character>(out var c))
            {
                Pairs.Add(new(c, 0));
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<Character>(out var c))
            {
                for (int i = 0; i < Pairs.Count; i++)
                {
                    if (ReferenceEquals(Pairs[i].character, c))
                    {
                        Pairs.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        private void Update()
        {
            for (int i = 0; i < Pairs.Count; i++)
            {
                Pair pair = Pairs[i];

                // 角色被销毁
                if (pair.character == null)
                {
                    Pairs.RemoveAt(i);
                    i--;
                    continue;
                }

                // 角色不在地上
                if (!pair.character.IsOnSlickGround)
                {
                    Pairs[i].slideValue = 0;
                    return;
                }

                if (Pairs[i].slideValue == 0)
                {
                    // 未开始滑行
                    NoSlide(i);
                }
                else
                {
                    // 角色因碰撞而停下
                    if (pair.character.Rigidbody.velocity.x == 0)
                    {
                        Pairs[i].slideValue = 0;
                        return;
                    }
                    else if (pair.slideValue > 0)
                    {
                        // 向左滑行
                        SlideLeft(i);
                    }
                    else
                    {
                        // 向右滑行
                        SlideRight(i);
                    }
                }
            }
        }

        private void NoSlide(int index)
        {
            var c = Pairs[index].character;
            // 角色在当前帧停止移动
            if (c.IsStoppedMovementThisFrame)
            {
                float newVelocity;
                // 根据角色方向计算力的方向和大小
                if (c.IsFaceRight)
                    newVelocity = c.CurrentSelfMoveSpeed;
                else
                    newVelocity = -c.CurrentSelfMoveSpeed;
                // 记录滑行速度
                Pairs[index].slideValue = newVelocity;
                // 向对象施加动力
                c.AddedVelocityX += newVelocity;
            }
        }

        private void SlideLeft(int index)
        {
            var p = Pairs[index];
            var c = p.character;
            if (c.IsSelfMoving)
            {
                if (c.IsFaceRight)
                {
                    // 如果角色往滑行方向移动，结束滑行
                    p.slideValue = 0;
                    return;
                }
                // 角色往滑行相反方向移动，削减滑行速度
                p.slideValue -= c.CurrentSelfMoveSpeed * Time.deltaTime;
            }
            // 削减滑行速度
            p.slideValue -= c.CurrentSelfMoveSpeed * Time.deltaTime;
            if (p.slideValue <= 0)
            {
                // 滑行结束
                p.slideValue = 0;
                return;
            }
            else if (c.IsSelfMoving)
            {
                // 有滑行速度的情况下，抵消角色自身移动速度
                c.AddedVelocityX += c.CurrentSelfMoveSpeed;
            }
            // 向对象施加动力
            c.AddedVelocityX += p.slideValue;
        }

        private void SlideRight(int index)
        {
            var p = Pairs[index];
            var c = p.character;
            if (c.IsSelfMoving)
            {
                if (!c.IsFaceRight)
                {
                    // 如果角色往滑行方向移动，结束滑行
                    p.slideValue = 0;
                    return;
                }
                // 角色往滑行相反方向移动，削减滑行速度
                p.slideValue += c.CurrentSelfMoveSpeed * Time.deltaTime;
            }
            // 削减滑行速度
            p.slideValue += c.CurrentSelfMoveSpeed * Time.deltaTime;
            if (p.slideValue >= 0)
            {
                // 滑行结束
                p.slideValue = 0;
                return;
            }
            else if (c.IsSelfMoving)
            {
                // 有滑行速度的情况下，抵消角色自身移动速度
                c.AddedVelocityX -= c.CurrentSelfMoveSpeed;
            }
            // 向对象施加动力
            c.AddedVelocityX += p.slideValue;
        }
    }
}
