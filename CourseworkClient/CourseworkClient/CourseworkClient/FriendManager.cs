using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CourseworkClient
{
    public class FriendManager
    {
        List<Friend> friends = new List<Friend>();
        public FriendManager(List<Friend> f)
        {
            friends = f;
        }
    }
}
