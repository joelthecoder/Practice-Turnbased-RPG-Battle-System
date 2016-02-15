using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_Battle_Test
{
    public sealed class StatusItem : Item
    {
        public StatusEffect Status = null;

        public StatusItem(string name, StatusEffect status) : base(name)
        {
            Status = status;

            Alignment = Status.StatusAlignment;
        }

        protected override void UseItem(BattleEntity User, params BattleEntity[] Entities)
        {
            for (int i = 0; i < Entities.Length; i++)
            {
                Entities[i].InflictStatus(User, Status);
            }
        }

        public override Item Copy()
        {
            return new StatusItem(Name, Status.Copy());
        }
    }
}
