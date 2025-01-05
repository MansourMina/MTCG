using MTCG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Services
{
    public class BattleQueueService
    {
        private Queue<User> _queue = new();
        private readonly object _lock = new();

        public bool TryEnqueue(User user)
        {
            lock (_lock)
            {
                if (_queue.Contains(user))
                    return false;

                _queue.Enqueue(user);
                return true;
            }
        }

        public User? TryDequeue()
        {
            lock (_lock)
            {
                if (_queue.Count > 0)
                    return _queue.Dequeue();
                return null;
            }
        }

        public bool HasOpponent()
        {
            lock (_lock)
            {
                return _queue.Count > 1;
            }
        }

        public bool RemoveUser(User user)
        {
            lock (_lock)
            {
                if (_queue.Contains(user))
                {
                    _queue = new Queue<User>(_queue.Where(u => u != user));
                    return true;
                }
                return false;
            }
        }
    }

}
