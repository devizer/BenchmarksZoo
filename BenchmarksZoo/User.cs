using System;
using System.Collections.Generic;
using System.Linq;

namespace BenchmarksZoo
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool KnownClient { get; set; }
        public DateTime? Birthday { get; set; }
        
        private static int Counter = 0;

        public static User[] Generate(int count)
        {
            Random rnd = new Random(42);
            List<User> ret = new List<User>();
            for (int i = 0; i < count; i++)
            {
                var name = new string(Enumerable.Range(0, rnd.Next(10, 20)).Select(x => (char) rnd.Next(65, 90)).ToArray());
                ret.Add(new User()
                {
                    Id = Counter++,
                    Name = name,
                    KnownClient = rnd.Next(2) == 0,
                    Birthday = DateTime.Now.AddDays(-rnd.Next((16*365)-5000)),
                });
            }

            return ret.ToArray();
        }

        private sealed class NameRelationalComparer : IComparer<User>
        {
            public int Compare(User x, User y)
            {
                if (ReferenceEquals(x, y)) return 0;
                if (ReferenceEquals(null, y)) return 1;
                if (ReferenceEquals(null, x)) return -1;
                return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
            }
        }

        public static IComparer<User> ComparerByName { get; } = new NameRelationalComparer();


    }
}