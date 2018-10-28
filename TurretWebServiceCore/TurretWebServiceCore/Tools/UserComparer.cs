using System.Collections.Generic;
using TurretWebServiceCore.Models;

namespace TurretWebServiceCore.Tools
{
    public abstract class UserComparer : IComparer<User>
    {
        public abstract int Compare(User x, User y);
    }

    public class UserScoreComparer : UserComparer // Sort desc
    {
        public override int Compare(User x, User y)
        {
            if (x.MaxScore > y.MaxScore)
            {
                return -1;
            }
            else if (x.MaxScore < y.MaxScore)
            {
                return 1;
            }
            else return 0;
        }
    }

    public class UserLevelComparer : UserComparer // Sort desc
    {
        public override int Compare(User x, User y)
        {
            if (x.MaxLevel > y.MaxLevel)
            {
                return -1;
            }
            else if (x.MaxLevel < y.MaxLevel)
            {
                return 1;
            }
            else return 0;
        }
    }
}
