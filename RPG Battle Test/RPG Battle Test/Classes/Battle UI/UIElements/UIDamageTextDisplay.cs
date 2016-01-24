using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using SFML.Audio;
using static RPG_Battle_Test.Globals;

namespace RPG_Battle_Test
{
    /// <summary>
    /// The UIElement for the damage display above BattleEntity's heads when taking damage
    /// </summary>
    public sealed class UIDamageTextDisplay : UITimedTextElement
    {
        private const float MOVE_AMOUNT = 3f;
        private const float POS_ABOVE = -20f;
        private const float POS_BELOW = 5f;

        private Vector2f TargetPosition = new Vector2f();
        private bool MovedUp = false;
        private bool DoneMoving = false;

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="damageType"></param>
        /// <param name="element"></param>
        /// <param name="time"></param>
        /// <param name="text"></param>
        /// <param name="position"></param>
        /// <param name="drawDepth"></param>
        public UIDamageTextDisplay(DamageTypes damageType, Elements element, float time,
                                   string text, Vector2f position, float drawDepth) : base(time, text, position, Color.White, drawDepth)
        {
            //Change color based on DamageType and/or Element
            //Prioritize Element color
            Color = GetElementColor(element);

            if (Color == Color.White)
            {
                Color = GetDamageColor(damageType);
            }

            TargetPosition = Position + new Vector2f(0f, POS_ABOVE);
        }

        /// <summary>
        /// Additional constructor for passing in a specific color. Useful for heals
        /// </summary>
        /// <param name="time"></param>
        /// <param name="text"></param>
        /// <param name="position"></param>
        /// <param name="color"></param>
        /// <param name="drawDepth"></param>
        public UIDamageTextDisplay(float time, string text, Vector2f position, Color color, float drawDepth)
            : this(DamageTypes.None, Elements.Neutral, time, text, position, drawDepth)
        {
            Color = color;
        }

        /// <summary>
        /// Returns a Color corresponding to a type of Elemental damage
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private Color GetElementColor(Elements element)
        {
            switch(element)
            {
                case Elements.Fire: return Color.Red;
                case Elements.Ice: return Color.Blue;
                case Elements.Poison: return Color.Black;
                case Elements.Earth: return new Color(165, 42, 42);
                default: return Color.White;
            }
        }

        /// <summary>
        /// Returns a Color corresponding to a DamageType
        /// </summary>
        /// <param name="damageType"></param>
        /// <returns></returns>
        private Color GetDamageColor(DamageTypes damageType)
        {
            switch (damageType)
            {
                case DamageTypes.None: return Color.Green;
                case DamageTypes.Magic: return Color.Yellow;
                default: return Color.White;
            }
        }

        public override void Update()
        {
            base.Update();

            if (DoneMoving == true)
                return;

            if (MovedUp == false)
            {
                Position = new Vector2f(Position.X, Position.Y - MOVE_AMOUNT);
                if (Position.Y < TargetPosition.Y)
                {
                    Position = TargetPosition;
                    MovedUp = true;
                }
            }
            else
            {
                Position = new Vector2f(Position.X, Position.Y + MOVE_AMOUNT);
                if (Position.Y > (TargetPosition.Y + POS_BELOW))
                {
                    Position = new Vector2f(TargetPosition.X, TargetPosition.Y + POS_BELOW);
                    DoneMoving = true;
                }
            }
        }
    }
}
