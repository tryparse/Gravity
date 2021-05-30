using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
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
        private const float DEFAULT_DIFF_DISTANCE = 10f;
        private const int DEFAULT_HISTORY_COUNT = 500;

        private Queue<TrajectoryHistoryElement> _history;

        private int _historyPointCount;

        public IEnumerable<TrajectoryHistoryElement> History => _history;

        public TrajectoryComponent()
        {
            _historyPointCount = DEFAULT_HISTORY_COUNT;

            _history = new Queue<TrajectoryHistoryElement>(_historyPointCount);
        }

        public TrajectoryComponent(int historyPointCount)
        {
            if (historyPointCount < 0)
            {
                throw new ArgumentException($"{nameof(historyPointCount)} can not be less than zero");
            }

            _historyPointCount = historyPointCount;

            _history = new Queue<TrajectoryHistoryElement>(_historyPointCount);
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
                    && distance > DEFAULT_DIFF_DISTANCE)
                {
                    _history.Enqueue(new TrajectoryHistoryElement(previous.To, position));
                }
            }

            if (_history.Count > _historyPointCount)
            {
                _history.Dequeue();
            }
        } 
    }
}
