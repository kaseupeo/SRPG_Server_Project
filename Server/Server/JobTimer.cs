using ServerCore;

namespace Server;

public struct JobTimerElement : IComparable<JobTimerElement>
{
    public int executionTick; // 실행시간
    public Action action;
    
    public int CompareTo(JobTimerElement other)
    {
        return other.executionTick - executionTick;
    }
}

public class JobTimer
{
    private PriorityQueue<JobTimerElement> _priorityQueue = new PriorityQueue<JobTimerElement>();
    private object _lock = new object();

    public static JobTimer Instance { get; } = new JobTimer();

    public void Push(Action action, int tickAfter = 0)
    {
        JobTimerElement job;
        job.executionTick = Environment.TickCount + tickAfter;
        job.action = action;

        lock (_lock)
        {
            _priorityQueue.Push(job);
        }
    }

    public void Flush()
    {
        while (true)
        {
            int now = Environment.TickCount;
            JobTimerElement job;

            lock (_lock)
            {
                if (_priorityQueue.Count == 0)
                    break;

                job = _priorityQueue.Peek();
                if (job.executionTick > now)
                    break;

                _priorityQueue.Pop();
            }
            
            job.action.Invoke();
        }
    }
}