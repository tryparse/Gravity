using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Gravity.Components
{
    struct TrajectoryHistoryElement
    {
        public TrajectoryHistoryElement(Vector2 from, Vector2 to)
        {
            From = from;
            To = to;
        }

        public Vector2 From { get; }
        public Vector2 To { get; }
    }

    class TrajectoryComponent
    {
        private int _maxHistoryCount = 2000;
        private Queue<TrajectoryHistoryElement> _history;

        public IEnumerable<TrajectoryHistoryElement> History => _history;

        public TrajectoryComponent()
        {
            _history = new Queue<TrajectoryHistoryElement>(_maxHistoryCount);
        }

        public void AddHistoryEntry(Vector2 position)
        {
            if (_history.Count == 0)
            {
                _history.Enqueue(new TrajectoryHistoryElement(position, position));
            }
            else
            {
                var previous = _history.LastOrDefault();

                var distance = Vector2.Distance(previous.To, position);

                if (previous.From != position
                    && previous.To != position
                    && distance > 10f)
                {
                    _history.Enqueue(new TrajectoryHistoryElement(previous.To, position));
                }
            }

            if (_history.Count > _maxHistoryCount)
            {
                _history.Dequeue();
            }
        } 
    }
}
